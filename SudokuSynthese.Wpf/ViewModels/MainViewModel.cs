using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
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
    private readonly SudokuGenerator _sudokuGenerator;

    private readonly DispatcherTimer _timer;
    private TimeSpan _elapsedTime;
    private bool _isPuzzleSolved;

    private NotationMode _currentMode;
    private CellColor _selectedColor;
    private DifficultyLevel _selectedDifficulty;
    private string _statusMessage;

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

    public DifficultyLevel SelectedDifficulty
    {
        get => _selectedDifficulty;
        set
        {
            if (_selectedDifficulty != value)
            {
                _selectedDifficulty = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Temps écoulé depuis le début de la grille actuelle.
    /// </summary>
    public string ElapsedTimeText
    {
        get
        {
            return _elapsedTime.ToString(@"mm\:ss");
        }
    }

    /// <summary>
    /// Message affiché dans la barre inférieure.
    /// </summary>
    public string StatusMessage
    {
        get => _statusMessage;
        set
        {
            if (_statusMessage != value)
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }
    }

    public ICommand SelectCellCommand { get; }
    public ICommand InputNumberCommand { get; }
    public ICommand SetModeCommand { get; }
    public ICommand SetColorCommand { get; }
    public ICommand SetDifficultyCommand { get; }
    public ICommand UndoCommand { get; }
    public ICommand RedoCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand LoadCommand { get; }
    public ICommand NewGridCommand { get; }

    public MainViewModel()
    {
        _selectionService = new SelectionService();
        _validator = new SudokuValidator();
        _undoRedoManager = new UndoRedoManager();
        _saveLoadService = new SaveLoadService();
        _sudokuGenerator = new SudokuGenerator();

        _currentMode = NotationMode.FinalValue;
        _selectedColor = CellColor.None;
        _selectedDifficulty = DifficultyLevel.Moyen;
        _elapsedTime = TimeSpan.Zero;
        _isPuzzleSolved = false;
        _statusMessage = "Prêt — Clique sur une cellule, puis utilise le clavier 1 à 9 ou les boutons.";

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        _timer.Tick += (_, _) =>
        {
            _elapsedTime = _elapsedTime.Add(TimeSpan.FromSeconds(1));
            OnPropertyChanged(nameof(ElapsedTimeText));
        };

        _grid = _sudokuGenerator.GenerateNewPuzzle(_selectedDifficulty);

        Cells = new ObservableCollection<CellViewModel>(
            _grid.GetAllCells().Select(cell => new CellViewModel(cell))
        );

        SelectCellCommand = new RelayCommand(SelectCell);
        InputNumberCommand = new RelayCommand(InputNumber);
        SetModeCommand = new RelayCommand(SetMode);
        SetColorCommand = new RelayCommand(SetColor);
        SetDifficultyCommand = new RelayCommand(SetDifficulty);
        UndoCommand = new RelayCommand(_ => Undo());
        RedoCommand = new RelayCommand(_ => Redo());
        SaveCommand = new RelayCommand(_ => Save());
        LoadCommand = new RelayCommand(_ => Load());
        NewGridCommand = new RelayCommand(_ => NewGrid());

        ApplySelectionToCells();
        RefreshValidation();
        RefreshCells();
        StartTimer();
    }

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
        CheckIfPuzzleSolved();
    }

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
        CheckIfPuzzleSolved();
    }

    private void SetDifficulty(object? parameter)
    {
        if (parameter is DifficultyLevel difficulty)
        {
            SelectedDifficulty = difficulty;
            return;
        }

        if (parameter is string text && Enum.TryParse(text, out DifficultyLevel parsedDifficulty))
        {
            SelectedDifficulty = parsedDifficulty;
        }
    }

    private void Undo()
    {
        _undoRedoManager.Undo();

        RefreshValidation();
        RefreshCells();

        if (_isPuzzleSolved && !IsPuzzleSolved())
        {
            _isPuzzleSolved = false;
            StatusMessage = "Modification effectuée — le compteur reprend.";
            StartTimer();
        }
    }

    private void Redo()
    {
        _undoRedoManager.Redo();

        RefreshValidation();
        RefreshCells();
        CheckIfPuzzleSolved();
    }

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

        RebuildCellsFromGrid();

        _selectionService.ClearSelection();
        _undoRedoManager.Clear();

        ApplySelectionToCells();
        RefreshValidation();
        RefreshCells();

        ResetTimer();
        StartTimer();

        StatusMessage = "Grille chargée — compteur démarré.";
        CheckIfPuzzleSolved();
    }

    /// <summary>
    /// Génère une nouvelle grille selon le niveau choisi par le joueur.
    /// </summary>
    private void NewGrid()
    {
        _grid = _sudokuGenerator.GenerateNewPuzzle(SelectedDifficulty);

        RebuildCellsFromGrid();

        _selectionService.ClearSelection();
        _undoRedoManager.Clear();

        CurrentMode = NotationMode.FinalValue;
        SelectedColor = CellColor.None;

        ApplySelectionToCells();
        RefreshValidation();
        RefreshCells();

        ResetTimer();
        StartTimer();

        StatusMessage = $"Nouvelle grille {SelectedDifficulty} générée — compteur démarré.";
    }

    private void RebuildCellsFromGrid()
    {
        Cells.Clear();

        foreach (SudokuCell cell in _grid.GetAllCells())
        {
            Cells.Add(new CellViewModel(cell));
        }
    }

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

    private void ApplySelectionToCells()
    {
        IReadOnlyCollection<CellPosition> selectedPositions = _selectionService.GetSelectedCells();

        HashSet<CellPosition> selectedSet = selectedPositions.ToHashSet();

        foreach (CellViewModel cellViewModel in Cells)
        {
            CellPosition position = new CellPosition(cellViewModel.Row, cellViewModel.Column);

            bool isSelected = selectedSet.Contains(position);

            cellViewModel.IsSelected = isSelected;

            if (!isSelected)
            {
                cellViewModel.SelectionBorderThickness = new Thickness(0);
                continue;
            }

            bool hasSelectedTop = selectedSet.Contains(
                new CellPosition(cellViewModel.Row - 1, cellViewModel.Column));

            bool hasSelectedBottom = selectedSet.Contains(
                new CellPosition(cellViewModel.Row + 1, cellViewModel.Column));

            bool hasSelectedLeft = selectedSet.Contains(
                new CellPosition(cellViewModel.Row, cellViewModel.Column - 1));

            bool hasSelectedRight = selectedSet.Contains(
                new CellPosition(cellViewModel.Row, cellViewModel.Column + 1));

            double thickness = 4;

            double left = hasSelectedLeft ? 0 : thickness;
            double top = hasSelectedTop ? 0 : thickness;
            double right = hasSelectedRight ? 0 : thickness;
            double bottom = hasSelectedBottom ? 0 : thickness;

            cellViewModel.SelectionBorderThickness = new Thickness(left, top, right, bottom);
        }
    }

    private void RefreshValidation()
    {
        IReadOnlyCollection<CellPosition> invalidPositions = _validator.GetInvalidCells(_grid);

        foreach (CellViewModel cellViewModel in Cells)
        {
            CellPosition position = new CellPosition(cellViewModel.Row, cellViewModel.Column);

            cellViewModel.HasError = invalidPositions.Contains(position);
        }
    }

    private void RefreshCells()
    {
        foreach (CellViewModel cellViewModel in Cells)
        {
            cellViewModel.Refresh();
        }
    }

    /// <summary>
    /// Vérifie si la grille est complètement résolue.
    /// Si oui, le compteur est arrêté.
    /// </summary>
    private void CheckIfPuzzleSolved()
    {
        if (_isPuzzleSolved)
        {
            return;
        }

        if (!IsPuzzleSolved())
        {
            return;
        }

        _isPuzzleSolved = true;
        StopTimer();

        StatusMessage = $"Grille résolue ! Temps final : {ElapsedTimeText}";
    }

    /// <summary>
    /// Une grille est résolue si :
    /// - toutes les cellules ont une valeur ;
    /// - aucune règle Sudoku n'est violée.
    /// </summary>
    private bool IsPuzzleSolved()
    {
        bool allCellsFilled = _grid.GetAllCells()
            .All(cell => cell.Value is not null);

        if (!allCellsFilled)
        {
            return false;
        }

        return _validator.IsValid(_grid);
    }

    private void StartTimer()
    {
        if (!_timer.IsEnabled)
        {
            _timer.Start();
        }
    }

    private void StopTimer()
    {
        if (_timer.IsEnabled)
        {
            _timer.Stop();
        }
    }

    private void ResetTimer()
    {
        StopTimer();

        _elapsedTime = TimeSpan.Zero;
        _isPuzzleSolved = false;

        OnPropertyChanged(nameof(ElapsedTimeText));
    }

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