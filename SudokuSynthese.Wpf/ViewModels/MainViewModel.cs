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

    /// <summary>
    /// Mode de saisie actuellement utilisé.
    /// 
    /// Il détermine le comportement d'un chiffre saisi :
    /// valeur finale, note en coin, note centrale ou couleur.
    /// </summary>
    public NotationMode CurrentMode
    {
        get => _currentMode;
        set
        {
            if (_currentMode != value)
            {
                _currentMode = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(IsFinalValueModeSelected));
                OnPropertyChanged(nameof(IsCornerNoteModeSelected));
                OnPropertyChanged(nameof(IsCenterNoteModeSelected));
                OnPropertyChanged(nameof(IsColorModeSelected));

                StatusMessage = $"Mode actif : {GetModeDisplayName(CurrentMode)}.";
            }
        }
    }

    /// <summary>
    /// Couleur actuellement sélectionnée pour le mode Couleur.
    /// </summary>
    public CellColor SelectedColor
    {
        get => _selectedColor;
        set
        {
            if (_selectedColor != value)
            {
                _selectedColor = value;

                OnPropertyChanged();
                RefreshColorSelectionProperties();
            }
        }
    }

    /// <summary>
    /// Niveau de difficulté choisi par le joueur.
    /// 
    /// Il influence le nombre de chiffres donnés
    /// au moment de la génération d'une nouvelle grille.
    /// </summary>
    public DifficultyLevel SelectedDifficulty
    {
        get => _selectedDifficulty;
        set
        {
            if (_selectedDifficulty != value)
            {
                _selectedDifficulty = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(IsEasyDifficultySelected));
                OnPropertyChanged(nameof(IsMediumDifficultySelected));
                OnPropertyChanged(nameof(IsHardDifficultySelected));

                StatusMessage = $"Niveau sélectionné : {SelectedDifficulty}. Clique sur Générer une grille pour commencer une nouvelle partie.";
            }
        }
    }

    /// <summary>
    /// Indique si le niveau Facile est sélectionné.
    /// </summary>
    public bool IsEasyDifficultySelected => SelectedDifficulty == DifficultyLevel.Facile;

    /// <summary>
    /// Indique si le niveau Moyen est sélectionné.
    /// </summary>
    public bool IsMediumDifficultySelected => SelectedDifficulty == DifficultyLevel.Moyen;

    /// <summary>
    /// Indique si le niveau Difficile est sélectionné.
    /// </summary>
    public bool IsHardDifficultySelected => SelectedDifficulty == DifficultyLevel.Difficile;

    /// <summary>
    /// Indique si le mode Valeur finale est actif.
    /// </summary>
    public bool IsFinalValueModeSelected => CurrentMode == NotationMode.FinalValue;

    /// <summary>
    /// Indique si le mode Note en coin est actif.
    /// </summary>
    public bool IsCornerNoteModeSelected => CurrentMode == NotationMode.CornerNote;

    /// <summary>
    /// Indique si le mode Note centrale est actif.
    /// </summary>
    public bool IsCenterNoteModeSelected => CurrentMode == NotationMode.CenterNote;

    /// <summary>
    /// Indique si le mode Couleur est actif.
    /// </summary>
    public bool IsColorModeSelected => CurrentMode == NotationMode.Color;

    /// <summary>
    /// Indique si aucune couleur est sélectionnée.
    /// </summary>
    public bool IsNoColorSelected => SelectedColor == CellColor.None;

    /// <summary>
    /// Indique si la couleur Bleue est sélectionnée.
    /// </summary>
    public bool IsBlueColorSelected => SelectedColor == CellColor.Blue;

    /// <summary>
    /// Indique si la couleur Verte est sélectionnée.
    /// </summary>
    public bool IsGreenColorSelected => SelectedColor == CellColor.Green;

    /// <summary>
    /// Indique si la couleur Jaune est sélectionnée.
    /// </summary>
    public bool IsYellowColorSelected => SelectedColor == CellColor.Yellow;

    /// <summary>
    /// Indique si la couleur Rouge est sélectionnée.
    /// </summary>
    public bool IsRedColorSelected => SelectedColor == CellColor.Red;

    /// <summary>
    /// Indique si la couleur Violette est sélectionnée.
    /// </summary>
    public bool IsPurpleColorSelected => SelectedColor == CellColor.Purple;

    /// <summary>
    /// Indique si la couleur Orange est sélectionnée.
    /// </summary>
    public bool IsOrangeColorSelected => SelectedColor == CellColor.Orange;

    /// <summary>
    /// Indique si la couleur Rose est sélectionnée.
    /// </summary>
    public bool IsPinkColorSelected => SelectedColor == CellColor.Pink;

    /// <summary>
    /// Indique si la couleur Grise est sélectionnée.
    /// </summary>
    public bool IsGrayColorSelected => SelectedColor == CellColor.Gray;

    /// <summary>
    /// Indique si la couleur Cyan est sélectionnée.
    /// </summary>
    public bool IsCyanColorSelected => SelectedColor == CellColor.Cyan;

    /// <summary>
    /// Indique s'il existe au moins une action à annuler.
    /// </summary>
    public bool CanUndo => _undoRedoManager.CanUndo;

    /// <summary>
    /// Indique s'il existe au moins une action à refaire.
    /// </summary>
    public bool CanRedo => _undoRedoManager.CanRedo;

    /// <summary>
    /// Temps écoulé depuis le début de la grille actuelle.
    /// </summary>
    public string ElapsedTimeText => _elapsedTime.ToString(@"mm\:ss");

    /// <summary>
    /// Message affiché dans la barre de statut.
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
        NewGridCommand = new RelayCommand(_ => RequestNewGrid());

        ApplySelectionToCells();
        RefreshValidation();
        RefreshCells();
        RefreshUndoRedoState();
        StartTimer();
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
            StatusMessage = "Sélection multiple mise à jour.";
        }
        else
        {
            _selectionService.SelectSingle(position);
            StatusMessage = $"Cellule sélectionnée : ligne {cellViewModel.Row + 1}, colonne {cellViewModel.Column + 1}.";
        }

        ApplySelectionToCells();
        RefreshCells();
    }

    /// <summary>
    /// Traite la saisie d'un chiffre selon le mode actif.
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
            StatusMessage = "Aucune cellule sélectionnée.";
            return;
        }

        if (CurrentMode == NotationMode.Color)
        {
            StatusMessage = "Mode Couleur actif — choisis une couleur dans la palette.";
            return;
        }

        IInputStrategy strategy = CreateCurrentStrategy();

        ISudokuCommand command = strategy.CreateCommand(_grid, selectedCells, input);

        _undoRedoManager.ExecuteCommand(command);

        ApplySelectionToCells();
        RefreshValidation();
        RefreshCells();
        RefreshUndoRedoState();

        StatusMessage = selectedCells.Count == 1
            ? $"Chiffre {input} appliqué en mode {GetModeDisplayName(CurrentMode)}."
            : $"Chiffre {input} appliqué à {selectedCells.Count} cellules en mode {GetModeDisplayName(CurrentMode)}.";

        CheckIfPuzzleSolved();
    }

    /// <summary>
    /// Change le mode de saisie courant.
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
    /// La couleur choisie devient aussi la couleur active.
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
            StatusMessage = $"Couleur active : {GetColorDisplayName(SelectedColor)}. Sélectionne une cellule pour l'appliquer.";
            return;
        }

        IInputStrategy strategy = new ColorInputStrategy();

        ISudokuCommand command = strategy.CreateCommand(_grid, selectedCells, (int)SelectedColor);

        _undoRedoManager.ExecuteCommand(command);

        ApplySelectionToCells();
        RefreshValidation();
        RefreshCells();
        RefreshUndoRedoState();

        StatusMessage = selectedCells.Count == 1
            ? $"Couleur {GetColorDisplayName(SelectedColor)} appliquée."
            : $"Couleur {GetColorDisplayName(SelectedColor)} appliquée à {selectedCells.Count} cellules.";

        CheckIfPuzzleSolved();
    }

    /// <summary>
    /// Change le niveau de difficulté sélectionné.
    /// </summary>
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

    /// <summary>
    /// Annule la dernière action du joueur.
    /// </summary>
    public void Undo()
    {
        if (!CanUndo)
        {
            StatusMessage = "Aucune action à annuler.";
            return;
        }

        _undoRedoManager.Undo();

        ApplySelectionToCells();
        RefreshValidation();
        RefreshCells();
        RefreshUndoRedoState();

        StatusMessage = "Dernière action annulée.";

        if (_isPuzzleSolved && !IsPuzzleSolved())
        {
            _isPuzzleSolved = false;
            StatusMessage = "Modification effectuée — le compteur reprend.";
            StartTimer();
        }
    }

    /// <summary>
    /// Rétablit la dernière action annulée.
    /// </summary>
    public void Redo()
    {
        if (!CanRedo)
        {
            StatusMessage = "Aucune action à refaire.";
            return;
        }

        _undoRedoManager.Redo();

        ApplySelectionToCells();
        RefreshValidation();
        RefreshCells();
        RefreshUndoRedoState();

        StatusMessage = "Action rétablie.";

        CheckIfPuzzleSolved();
    }

    /// <summary>
    /// Sauvegarde la grille courante dans un fichier JSON.
    /// </summary>
    public void Save()
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
            StatusMessage = "Grille sauvegardée avec succès.";
        }
        else
        {
            StatusMessage = "Sauvegarde annulée.";
        }
    }

    /// <summary>
    /// Charge une grille depuis un fichier JSON.
    /// </summary>
    public void Load()
    {
        OpenFileDialog dialog = new OpenFileDialog
        {
            Filter = "Fichier Sudoku JSON (*.json)|*.json",
            DefaultExt = ".json"
        };

        bool? result = dialog.ShowDialog();

        if (result != true)
        {
            StatusMessage = "Chargement annulé.";
            return;
        }

        _grid = _saveLoadService.LoadFromFile(dialog.FileName);

        RebuildCellsFromGrid();

        _selectionService.ClearSelection();
        _undoRedoManager.Clear();

        ApplySelectionToCells();
        RefreshValidation();
        RefreshCells();
        RefreshUndoRedoState();

        ResetTimer();
        StartTimer();

        StatusMessage = "Grille chargée — compteur démarré.";
        CheckIfPuzzleSolved();
    }

    /// <summary>
    /// Demande une confirmation avant de créer une nouvelle grille.
    /// </summary>
    public void RequestNewGrid()
    {
        MessageBoxResult result = MessageBox.Show(
            "Voulez-vous vraiment commencer une nouvelle grille ?\n\nLa partie actuelle sera remplacée.",
            "Nouvelle grille",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes)
        {
            StatusMessage = "Nouvelle grille annulée.";
            return;
        }

        NewGrid();
    }

    /// <summary>
    /// Génère une nouvelle grille selon le niveau choisi.
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
        RefreshUndoRedoState();

        ResetTimer();
        StartTimer();

        StatusMessage = $"Nouvelle grille {SelectedDifficulty} générée — compteur démarré.";
    }

    /// <summary>
    /// Vide la sélection courante.
    /// </summary>
    public void ClearSelection()
    {
        _selectionService.ClearSelection();

        ApplySelectionToCells();
        RefreshCells();

        StatusMessage = "Sélection vidée.";
    }

    /// <summary>
    /// Reconstruit les CellViewModel à partir de la grille métier.
    /// </summary>
    private void RebuildCellsFromGrid()
    {
        Cells.Clear();

        foreach (SudokuCell cell in _grid.GetAllCells())
        {
            Cells.Add(new CellViewModel(cell));
        }
    }

    /// <summary>
    /// Crée la stratégie correspondant au mode courant.
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
    /// Applique visuellement la sélection aux cellules.
    /// 
    /// La méthode calcule aussi les cellules liées à la sélection :
    /// même ligne, même colonne, même bloc et même valeur.
    /// </summary>
    private void ApplySelectionToCells()
    {
        IReadOnlyCollection<CellPosition> selectedPositions = _selectionService.GetSelectedCells();
        HashSet<CellPosition> selectedSet = selectedPositions.ToHashSet();

        HashSet<int> selectedValues = Cells
            .Where(cellViewModel => selectedSet.Contains(new CellPosition(cellViewModel.Row, cellViewModel.Column)))
            .Where(cellViewModel => cellViewModel.Value is not null)
            .Select(cellViewModel => cellViewModel.Value!.Value)
            .ToHashSet();

        foreach (CellViewModel cellViewModel in Cells)
        {
            CellPosition position = new CellPosition(cellViewModel.Row, cellViewModel.Column);

            bool isSelected = selectedSet.Contains(position);

            cellViewModel.IsSelected = isSelected;

            cellViewModel.IsPeer = !isSelected && IsPeerOfSelection(cellViewModel, selectedSet);

            cellViewModel.IsSameValue =
                cellViewModel.Value is not null &&
                selectedValues.Contains(cellViewModel.Value.Value);

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

    /// <summary>
    /// Indique si une cellule est liée à la sélection courante.
    /// </summary>
    private static bool IsPeerOfSelection(
        CellViewModel cellViewModel,
        HashSet<CellPosition> selectedSet)
    {
        foreach (CellPosition selectedPosition in selectedSet)
        {
            bool sameRow = cellViewModel.Row == selectedPosition.Row;
            bool sameColumn = cellViewModel.Column == selectedPosition.Column;

            bool sameBlock =
                cellViewModel.Row / 3 == selectedPosition.Row / 3 &&
                cellViewModel.Column / 3 == selectedPosition.Column / 3;

            if (sameRow || sameColumn || sameBlock)
            {
                return true;
            }
        }

        return false;
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
    /// Vérifie si la grille est résolue.
    /// 
    /// Si la grille est complète et valide,
    /// le compteur de temps est arrêté.
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

        StatusMessage = $"Bravo ! Grille résolue en {ElapsedTimeText}.";

        MessageBox.Show(
            $"Bravo !\n\nTu as résolu la grille en {ElapsedTimeText}.",
            "Grille résolue",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    /// <summary>
    /// Indique si la grille est complètement résolue.
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

    /// <summary>
    /// Rafraîchit les propriétés liées à la couleur sélectionnée.
    /// </summary>
    private void RefreshColorSelectionProperties()
    {
        OnPropertyChanged(nameof(IsNoColorSelected));
        OnPropertyChanged(nameof(IsBlueColorSelected));
        OnPropertyChanged(nameof(IsGreenColorSelected));
        OnPropertyChanged(nameof(IsYellowColorSelected));
        OnPropertyChanged(nameof(IsRedColorSelected));
        OnPropertyChanged(nameof(IsPurpleColorSelected));
        OnPropertyChanged(nameof(IsOrangeColorSelected));
        OnPropertyChanged(nameof(IsPinkColorSelected));
        OnPropertyChanged(nameof(IsGrayColorSelected));
        OnPropertyChanged(nameof(IsCyanColorSelected));
    }

    /// <summary>
    /// Rafraîchit les propriétés indiquant l'état Undo / Redo.
    /// </summary>
    private void RefreshUndoRedoState()
    {
        OnPropertyChanged(nameof(CanUndo));
        OnPropertyChanged(nameof(CanRedo));
    }

    /// <summary>
    /// Démarre le compteur de temps.
    /// </summary>
    private void StartTimer()
    {
        if (!_timer.IsEnabled)
        {
            _timer.Start();
        }
    }

    /// <summary>
    /// Arrête le compteur de temps.
    /// </summary>
    private void StopTimer()
    {
        if (_timer.IsEnabled)
        {
            _timer.Stop();
        }
    }

    /// <summary>
    /// Réinitialise le compteur de temps.
    /// </summary>
    private void ResetTimer()
    {
        StopTimer();

        _elapsedTime = TimeSpan.Zero;
        _isPuzzleSolved = false;

        OnPropertyChanged(nameof(ElapsedTimeText));
    }

    /// <summary>
    /// Retourne le nom lisible d'un mode de saisie.
    /// </summary>
    private static string GetModeDisplayName(NotationMode mode)
    {
        return mode switch
        {
            NotationMode.FinalValue => "Valeur finale",
            NotationMode.CornerNote => "Note en coin",
            NotationMode.CenterNote => "Note centrale",
            NotationMode.Color => "Couleur",
            _ => mode.ToString()
        };
    }

    /// <summary>
    /// Retourne le nom lisible d'une couleur.
    /// </summary>
    private static string GetColorDisplayName(CellColor color)
    {
        return color switch
        {
            CellColor.None => "Aucune",
            CellColor.Blue => "Bleu",
            CellColor.Green => "Vert",
            CellColor.Yellow => "Jaune",
            CellColor.Red => "Rouge",
            CellColor.Purple => "Violet",
            CellColor.Orange => "Orange",
            CellColor.Pink => "Rose",
            CellColor.Gray => "Gris",
            CellColor.Cyan => "Cyan",
            _ => color.ToString()
        };
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