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

    public SudokuCell Cell { get; }

    public int Row => Cell.Row;

    public int Column => Cell.Column;

    public int? Value => Cell.Value;

    public string ValueText => Cell.Value?.ToString() ?? string.Empty;

    public string CornerNotesText
    {
        get
        {
            return string.Join(" ", Cell.CornerNotes.OrderBy(note => note));
        }
    }

    public string CenterNotesText
    {
        get
        {
            return string.Join(" ", Cell.CenterNotes.OrderBy(note => note));
        }
    }

    /// <summary>
    /// Affiche les notes centrales sur une ou deux lignes.
    /// Exemple :
    /// 1 2 3 4 5
    /// 6 7 8 9
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
    /// Première note en coin affichée dans le coin haut gauche.
    /// Ce n'est pas forcément le chiffre 1.
    /// </summary>
    public string CornerNote1 => GetCornerNoteByIndex(0);

    /// <summary>
    /// Deuxième note en coin affichée dans le coin haut droit.
    /// Ce n'est pas forcément le chiffre 2.
    /// </summary>
    public string CornerNote2 => GetCornerNoteByIndex(1);

    /// <summary>
    /// Troisième note en coin affichée dans le coin bas gauche.
    /// Ce n'est pas forcément le chiffre 3.
    /// </summary>
    public string CornerNote3 => GetCornerNoteByIndex(2);

    /// <summary>
    /// Quatrième note en coin affichée dans le coin bas droit.
    /// Ce n'est pas forcément le chiffre 4.
    /// </summary>
    public string CornerNote4 => GetCornerNoteByIndex(3);

    public CellColor Color => Cell.Color;

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

    public bool IsGiven => Cell.IsGiven;

    /// <summary>
    /// Bordure normale de la grille.
    /// Les lignes sont plus épaisses entre les blocs 3x3.
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
    /// Elle est calculée pour dessiner seulement le contour extérieur
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

    public CellViewModel(SudokuCell cell)
    {
        Cell = cell ?? throw new ArgumentNullException(nameof(cell));
    }

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