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
    /// Bordure de la cellule.
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

    public CellViewModel(SudokuCell cell)
    {
        Cell = cell ?? throw new ArgumentNullException(nameof(cell));
    }

    public void Refresh()
    {
        OnPropertyChanged(nameof(Row));
        OnPropertyChanged(nameof(Column));
        OnPropertyChanged(nameof(Value));
        OnPropertyChanged(nameof(ValueText));
        OnPropertyChanged(nameof(CornerNotesText));
        OnPropertyChanged(nameof(CenterNotesText));
        OnPropertyChanged(nameof(Color));
        OnPropertyChanged(nameof(IsSelected));
        OnPropertyChanged(nameof(HasError));
        OnPropertyChanged(nameof(IsGiven));
        OnPropertyChanged(nameof(BorderThickness));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}