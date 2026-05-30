namespace SudokuSynthese.Core.Models;

/// <summary>
/// Représente une sauvegarde de l'état d'une cellule de Sudoku.
/// 
/// Ce modèle est utilisé par le patron de conception Memento.
/// Il permet de restaurer une cellule exactement comme elle était avant une modification.
/// </summary>
public class CellMemento
{
    /// <summary>
    /// Valeur finale de la cellule au moment de la sauvegarde.
    /// Null signifie que la cellule était vide.
    /// </summary>
    public int? Value { get; }

    /// <summary>
    /// Copie des notes en coin au moment de la sauvegarde.
    /// </summary>
    public HashSet<int> CornerNotes { get; }

    /// <summary>
    /// Copie des notes au centre au moment de la sauvegarde.
    /// </summary>
    public HashSet<int> CenterNotes { get; }

    /// <summary>
    /// Couleur de la cellule au moment de la sauvegarde.
    /// </summary>
    public CellColor Color { get; }

    /// <summary>
    /// Crée une sauvegarde complète de l'état modifiable d'une cellule.
    /// </summary>
    /// <param name="value">Valeur finale de la cellule.</param>
    /// <param name="cornerNotes">Notes en coin de la cellule.</param>
    /// <param name="centerNotes">Notes au centre de la cellule.</param>
    /// <param name="color">Couleur appliquée à la cellule.</param>
    public CellMemento(
        int? value,
        IEnumerable<int> cornerNotes,
        IEnumerable<int> centerNotes,
        CellColor color)
    {
        Value = value;
        CornerNotes = new HashSet<int>(cornerNotes);
        CenterNotes = new HashSet<int>(centerNotes);
        Color = color;
    }
}