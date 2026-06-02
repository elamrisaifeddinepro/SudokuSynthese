using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Win32;
using SudokuSynthese.Core.Commands;
using SudokuSynthese.Core.Managers;
using SudokuSynthese.Core.Models;
using SudokuSynthese.Core.Services;
using SudokuSynthese.Core.Strategies;

namespace SudokuSynthese.Wpf.ViewModels;

/// <summary>
/// ViewModel principal de l'application.
/// 
/// Il fait le lien entre l'interface WPF et la logique métier du projet Core.
/// </summary>
public class MainViewModel : INotifyPropertyChanged
{
    private SudokuGrid _grid;

    private readonly SelectionService _selectionService;
    private readonly SudokuValidator _validator;
    private readonly UndoRedoManager _undoRedoManager;
    private readonly SaveLoadService _saveLoadService;

    private NotationMode _currentMode;
    private CellColor _selectedColor;

    public ObservableCollection<CellViewModel> Cells { get; }

    public NotationMode CurrentMode
    {
        get => _currentMode;
        set
        {
            if (_currentMode != value)
            {
                _currentMode = value;
                OnPropertyChanged();
            }
        }
    }

    public CellColor SelectedColor
    {
        get => _selectedColor;
        set
        {
            if (_selectedColor != value)
            {
                _selectedColor = value;
                OnPropertyChanged();
            }
        }
    }

    public ICommand SelectCellCommand { get; }
    public ICommand InputNumberCommand { get; }
    public ICommand SetModeCommand { get; }
    public ICommand SetColorCommand { get; }
    public ICommand UndoCommand { get; }
    public ICommand RedoCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand LoadCommand { get; }
    public ICommand NewGridCommand { get; }

    public MainViewModel()
    {
        _grid = new SudokuGrid();

        _selectionService = new SelectionService();
        _validator = new SudokuValidator();
        _undoRedoManager = new UndoRedoManager();
        _saveLoadService = new SaveLoadService();

        _currentMode = NotationMode.FinalValue;
        _selectedColor = CellColor.None;

        Cells = new ObservableCollection<CellViewModel>(
            _grid.GetAllCells().Select(cell => new CellViewModel(cell))
        );

        SelectCellCommand = new RelayCommand(SelectCell);
        InputNumberCommand = new RelayCommand(InputNumber);
        SetModeCommand = new RelayCommand(SetMode);
        SetColorCommand = new RelayCommand(SetColor);
        UndoCommand = new RelayCommand(_ => Undo());
        RedoCommand = new RelayCommand(_ => Redo());
        SaveCommand = new RelayCommand(_ => Save());
        LoadCommand = new RelayCommand(_ => Load());
        NewGridCommand = new RelayCommand(_ => NewGrid());

        RefreshValidation();
        RefreshCells();
    }

    /// <summary>
    /// Sélectionne une cellule.
    /// 
    /// Clic simple : sélection unique.
    /// Ctrl + clic : sélection multiple.
    /// </summary>
    private void SelectCell(object? parameter)
    {
        if (parameter is not CellViewModel cellViewModel)
        {
            return;
        }

        CellPosition position = new CellPosition(cellViewModel.Row, cellViewModel.Column);

        bool isCtrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

        if (isCtrlPressed)
        {
            _selectionService.ToggleSelection(position);
        }
        else
        {
            _selectionService.SelectSingle(position);
        }

        ApplySelectionToCells();
        RefreshCells();
    }

    /// <summary>
    /// Gère la saisie d'un chiffre selon le mode actif.
    /// 
    /// Mode valeur finale : place le chiffre comme grande valeur.
    /// Mode note en coin : ajoute ou retire le chiffre dans les notes en coin.
    /// Mode note centrale : ajoute ou retire le chiffre dans les notes centrales.
    /// Mode couleur : ignoré ici, car les couleurs sont appliquées par SetColor().
    /// </summary>
    private void InputNumber(object? parameter)
    {
        if (!TryConvertToInt(parameter, out int input))
        {
            return;
        }

        IReadOnlyCollection<CellPosition> selectedCells = _selectionService.GetSelectedCells();

        if (selectedCells.Count == 0)
        {
            return;
        }

        if (CurrentMode == NotationMode.Color)
        {
            return;
        }

        IInputStrategy strategy = CreateCurrentStrategy();

        ISudokuCommand command = strategy.CreateCommand(_grid, selectedCells, input);

        _undoRedoManager.ExecuteCommand(command);

        RefreshValidation();
        RefreshCells();
    }

    /// <summary>
    /// Change le mode actif : valeur finale, note coin, note centre ou couleur.
    /// </summary>
    private void SetMode(object? parameter)
    {
        if (parameter is NotationMode notationMode)
        {
            CurrentMode = notationMode;
            return;
        }

        if (parameter is string text && Enum.TryParse(text, out NotationMode parsedMode))
        {
            CurrentMode = parsedMode;
        }
    }

    /// <summary>
    /// Applique une couleur aux cellules sélectionnées.
    /// 
    /// Si aucune cellule n'est sélectionnée, la couleur devient seulement la couleur active.
    /// Si une ou plusieurs cellules sont sélectionnées, la couleur est appliquée directement.
    /// </summary>
    private void SetColor(object? parameter)
    {
        CellColor color;

        if (parameter is CellColor cellColor)
        {
            color = cellColor;
        }
        else if (parameter is string text && Enum.TryParse(text, out CellColor parsedColor))
        {
            color = parsedColor;
        }
        else
        {
            return;
        }

        SelectedColor = color;
        CurrentMode = NotationMode.Color;

        IReadOnlyCollection<CellPosition> selectedCells = _selectionService.GetSelectedCells();

        if (selectedCells.Count == 0)
        {
            return;
        }

        IInputStrategy strategy = new ColorInputStrategy();

        ISudokuCommand command = strategy.CreateCommand(_grid, selectedCells, (int)SelectedColor);

        _undoRedoManager.ExecuteCommand(command);

        RefreshValidation();
        RefreshCells();
    }

    /// <summary>
    /// Annule la dernière action.
    /// </summary>
    private void Undo()
    {
        _undoRedoManager.Undo();

        RefreshValidation();
        RefreshCells();
    }

    /// <summary>
    /// Refait la dernière action annulée.
    /// </summary>
    private void Redo()
    {
        _undoRedoManager.Redo();

        RefreshValidation();
        RefreshCells();
    }

    /// <summary>
    /// Sauvegarde la grille dans un fichier JSON.
    /// </summary>
    private void Save()
    {
        SaveFileDialog dialog = new SaveFileDialog
        {
            Filter = "Fichier Sudoku JSON (*.json)|*.json",
            DefaultExt = ".json",
            FileName = "sudoku-save.json"
        };

        bool? result = dialog.ShowDialog();

        if (result == true)
        {
            _saveLoadService.SaveToFile(_grid, dialog.FileName);
        }
    }

    /// <summary>
    /// Charge une grille depuis un fichier JSON.
    /// </summary>
    private void Load()
    {
        OpenFileDialog dialog = new OpenFileDialog
        {
            Filter = "Fichier Sudoku JSON (*.json)|*.json",
            DefaultExt = ".json"
        };

        bool? result = dialog.ShowDialog();

        if (result != true)
        {
            return;
        }

        _grid = _saveLoadService.LoadFromFile(dialog.FileName);

        Cells.Clear();

        foreach (SudokuCell cell in _grid.GetAllCells())
        {
            Cells.Add(new CellViewModel(cell));
        }

        _selectionService.ClearSelection();
        _undoRedoManager.Clear();

        RefreshValidation();
        RefreshCells();
    }

    /// <summary>
    /// Réinitialise complètement la grille.
    /// </summary>
    private void NewGrid()
    {
        _grid.Reset();

        _selectionService.ClearSelection();
        _undoRedoManager.Clear();

        RefreshValidation();
        RefreshCells();
    }

    /// <summary>
    /// Crée la bonne stratégie selon le mode actif.
    /// </summary>
    private IInputStrategy CreateCurrentStrategy()
    {
        return CurrentMode switch
        {
            NotationMode.FinalValue => new FinalValueInputStrategy(),
            NotationMode.CornerNote => new CornerNoteInputStrategy(),
            NotationMode.CenterNote => new CenterNoteInputStrategy(),
            NotationMode.Color => new ColorInputStrategy(),
            _ => new FinalValueInputStrategy()
        };
    }

    /// <summary>
    /// Applique les positions sélectionnées du SelectionService aux CellViewModel.
    /// </summary>
    private void ApplySelectionToCells()
    {
        IReadOnlyCollection<CellPosition> selectedPositions = _selectionService.GetSelectedCells();

        foreach (CellViewModel cellViewModel in Cells)
        {
            CellPosition position = new CellPosition(cellViewModel.Row, cellViewModel.Column);

            cellViewModel.IsSelected = selectedPositions.Contains(position);
        }
    }

    /// <summary>
    /// Valide la grille et marque les cellules en erreur.
    /// </summary>
    private void RefreshValidation()
    {
        IReadOnlyCollection<CellPosition> invalidPositions = _validator.GetInvalidCells(_grid);

        foreach (CellViewModel cellViewModel in Cells)
        {
            CellPosition position = new CellPosition(cellViewModel.Row, cellViewModel.Column);

            cellViewModel.HasError = invalidPositions.Contains(position);
        }
    }

    /// <summary>
    /// Rafraîchit toutes les cellules affichées.
    /// </summary>
    private void RefreshCells()
    {
        foreach (CellViewModel cellViewModel in Cells)
        {
            cellViewModel.Refresh();
        }
    }

    /// <summary>
    /// Convertit un paramètre WPF en entier.
    /// </summary>
    private static bool TryConvertToInt(object? parameter, out int value)
    {
        if (parameter is int intValue)
        {
            value = intValue;
            return true;
        }

        if (parameter is string text && int.TryParse(text, out int parsedValue))
        {
            value = parsedValue;
            return true;
        }

        value = 0;
        return false;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}