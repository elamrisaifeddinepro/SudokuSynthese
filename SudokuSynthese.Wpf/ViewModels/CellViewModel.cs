using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using SudokuSynthese.Core.Models;

namespace SudokuSynthese.Wpf.ViewModels;

/// <summary>
/// ViewModel représentant une cellule affichée dans l'interface WPF.
/// 
/// Il enveloppe une SudokuCell venant du projet Core.
/// Son rôle est d'exposer des propriétés faciles à utiliser dans le XAML.
/// </summary>
public class CellViewModel : INotifyPropertyChanged
{
    private Thickness _selectionBorderThickness = new Thickness(0);
    private bool _isPeer;
    private bool _isSameValue;

    /// <summary>
    /// Cellule métier associée à ce ViewModel.
    /// </summary>
    public SudokuCell Cell { get; }

    /// <summary>
    /// Ligne de la cellule dans la grille.
    /// </summary>
    public int Row => Cell.Row;

    /// <summary>
    /// Colonne de la cellule dans la grille.
    /// </summary>
    public int Column => Cell.Column;

    /// <summary>
    /// Valeur finale de la cellule.
    /// Null signifie que la cellule est vide.
    /// </summary>
    public int? Value => Cell.Value;

    /// <summary>
    /// Texte affiché pour la valeur principale.
    /// Si la cellule est vide, on affiche une chaîne vide.
    /// </summary>
    public string ValueText => Cell.Value?.ToString() ?? string.Empty;

    /// <summary>
    /// Texte complet des notes en coin.
    /// Exemple : "2 5 8 9".
    /// </summary>
    public string CornerNotesText
    {
        get
        {
            return string.Join(" ", Cell.CornerNotes.OrderBy(note => note));
        }
    }

    /// <summary>
    /// Texte complet des notes centrales.
    /// Exemple : "1 2 3 4 5".
    /// </summary>
    public string CenterNotesText
    {
        get
        {
            return string.Join(" ", Cell.CenterNotes.OrderBy(note => note));
        }
    }

    /// <summary>
    /// Texte des notes centrales affiché sur une ou deux lignes.
    /// </summary>
    public string CenterNotesDisplayText
    {
        get
        {
            List<int> notes = Cell.CenterNotes
                .OrderBy(note => note)
                .ToList();

            if (notes.Count == 0)
            {
                return string.Empty;
            }

            if (notes.Count <= 5)
            {
                return string.Join(" ", notes);
            }

            string firstLine = string.Join(" ", notes.Take(5));
            string secondLine = string.Join(" ", notes.Skip(5));

            return firstLine + Environment.NewLine + secondLine;
        }
    }

    /// <summary>
    /// Première note affichée dans le coin haut gauche.
    /// Ce n'est pas forcément le chiffre 1.
    /// </summary>
    public string CornerNote1 => GetCornerNoteByIndex(0);

    /// <summary>
    /// Deuxième note affichée dans le coin haut droit.
    /// Ce n'est pas forcément le chiffre 2.
    /// </summary>
    public string CornerNote2 => GetCornerNoteByIndex(1);

    /// <summary>
    /// Troisième note affichée dans le coin bas gauche.
    /// Ce n'est pas forcément le chiffre 3.
    /// </summary>
    public string CornerNote3 => GetCornerNoteByIndex(2);

    /// <summary>
    /// Quatrième note affichée dans le coin bas droit.
    /// Ce n'est pas forcément le chiffre 4.
    /// </summary>
    public string CornerNote4 => GetCornerNoteByIndex(3);

    /// <summary>
    /// Couleur métier de la cellule.
    /// La conversion vers une Brush WPF est faite par CellColorToBrushConverter.
    /// </summary>
    public CellColor Color => Cell.Color;

    /// <summary>
    /// Indique si la cellule est sélectionnée.
    /// </summary>
    public bool IsSelected
    {
        get => Cell.IsSelected;
        set
        {
            if (Cell.IsSelected != value)
            {
                Cell.IsSelected = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Indique si la cellule est liée à la sélection courante.
    /// 
    /// Une cellule est liée si elle se trouve dans la même ligne,
    /// la même colonne ou le même bloc 3x3 qu'une cellule sélectionnée.
    /// </summary>
    public bool IsPeer
    {
        get => _isPeer;
        set
        {
            if (_isPeer != value)
            {
                _isPeer = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Indique si la cellule contient la même valeur
    /// qu'une cellule sélectionnée.
    /// </summary>
    public bool IsSameValue
    {
        get => _isSameValue;
        set
        {
            if (_isSameValue != value)
            {
                _isSameValue = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Indique si la cellule contient une erreur Sudoku.
    /// </summary>
    public bool HasError
    {
        get => Cell.HasError;
        set
        {
            if (Cell.HasError != value)
            {
                Cell.HasError = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Indique si la valeur de la cellule est donnée au départ.
    /// </summary>
    public bool IsGiven => Cell.IsGiven;

    /// <summary>
    /// Bordure normale de la grille.
    /// Elle permet d'avoir des lignes plus épaisses entre les blocs 3x3.
    /// </summary>
    public Thickness BorderThickness
    {
        get
        {
            double left = Column % 3 == 0 ? 3 : 0.5;
            double top = Row % 3 == 0 ? 3 : 0.5;
            double right = Column == 8 ? 3 : 0.5;
            double bottom = Row == 8 ? 3 : 0.5;

            return new Thickness(left, top, right, bottom);
        }
    }

    /// <summary>
    /// Bordure bleue de sélection.
    /// 
    /// Elle est calculée pour afficher seulement le contour extérieur
    /// du groupe de cellules sélectionnées.
    /// </summary>
    public Thickness SelectionBorderThickness
    {
        get => _selectionBorderThickness;
        set
        {
            if (_selectionBorderThickness != value)
            {
                _selectionBorderThickness = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Crée un ViewModel à partir d'une cellule métier.
    /// </summary>
    /// <param name="cell">Cellule métier à envelopper.</param>
    public CellViewModel(SudokuCell cell)
    {
        Cell = cell ?? throw new ArgumentNullException(nameof(cell));
    }

    /// <summary>
    /// Retourne la note affichée dans un emplacement visuel.
    /// </summary>
    /// <param name="index">Index de la note dans la liste triée.</param>
    /// <returns>Le chiffre de la note ou une chaîne vide.</returns>
    private string GetCornerNoteByIndex(int index)
    {
        List<int> notes = Cell.CornerNotes
            .OrderBy(note => note)
            .Take(4)
            .ToList();

        if (index < 0 || index >= notes.Count)
        {
            return string.Empty;
        }

        return notes[index].ToString();
    }

    /// <summary>
    /// Rafraîchit toutes les propriétés visibles dans l'interface.
    /// </summary>
    public void Refresh()
    {
        OnPropertyChanged(nameof(Row));
        OnPropertyChanged(nameof(Column));

        OnPropertyChanged(nameof(Value));
        OnPropertyChanged(nameof(ValueText));

        OnPropertyChanged(nameof(CornerNotesText));
        OnPropertyChanged(nameof(CenterNotesText));
        OnPropertyChanged(nameof(CenterNotesDisplayText));

        OnPropertyChanged(nameof(CornerNote1));
        OnPropertyChanged(nameof(CornerNote2));
        OnPropertyChanged(nameof(CornerNote3));
        OnPropertyChanged(nameof(CornerNote4));

        OnPropertyChanged(nameof(Color));
        OnPropertyChanged(nameof(IsSelected));
        OnPropertyChanged(nameof(IsPeer));
        OnPropertyChanged(nameof(IsSameValue));
        OnPropertyChanged(nameof(HasError));
        OnPropertyChanged(nameof(IsGiven));
        OnPropertyChanged(nameof(BorderThickness));
        OnPropertyChanged(nameof(SelectionBorderThickness));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}