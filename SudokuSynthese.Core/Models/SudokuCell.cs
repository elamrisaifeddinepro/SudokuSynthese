namespace SudokuSynthese.Core.Models;

/// <summary>
/// Représente une cellule individuelle dans une grille de Sudoku.
/// 
/// Une cellule contient sa position, sa valeur finale, ses notes,
/// sa couleur et certains états utilisés par le jeu.
/// </summary>
public class SudokuCell
{
    /// <summary>
    /// Ligne de la cellule dans la grille.
    /// Les valeurs possibles vont de 0 à 8.
    /// </summary>
    public int Row { get; }

    /// <summary>
    /// Colonne de la cellule dans la grille.
    /// Les valeurs possibles vont de 0 à 8.
    /// </summary>
    public int Column { get; }

    /// <summary>
    /// Valeur finale de la cellule.
    /// Null signifie que la cellule est vide.
    /// Sinon, la valeur doit être comprise entre 1 et 9.
    /// </summary>
    public int? Value { get; set; }

    /// <summary>
    /// Indique si la valeur de cette cellule est donnée au départ.
    /// Une cellule donnée ne doit normalement pas être modifiée par le joueur.
    /// </summary>
    public bool IsGiven { get; set; }

    /// <summary>
    /// Notes en coin de la cellule.
    /// Elles servent à mémoriser des petits candidats possibles.
    /// Exemple : 1, 3, 7.
    /// </summary>
    public HashSet<int> CornerNotes { get; } = new();

    /// <summary>
    /// Notes au centre de la cellule.
    /// Elles servent à mémoriser des hypothèses ou candidats principaux.
    /// </summary>
    public HashSet<int> CenterNotes { get; } = new();

    /// <summary>
    /// Couleur appliquée à la cellule.
    /// Par défaut, aucune couleur n'est appliquée.
    /// </summary>
    public CellColor Color { get; set; } = CellColor.None;

    /// <summary>
    /// Indique si la cellule est actuellement sélectionnée dans l'interface.
    /// </summary>
    public bool IsSelected { get; set; }

    /// <summary>
    /// Indique si la cellule contient une erreur selon les règles du Sudoku.
    /// </summary>
    public bool HasError { get; set; }

    /// <summary>
    /// Crée une cellule à partir d'une ligne et d'une colonne.
    /// </summary>
    /// <param name="row">Indice de ligne entre 0 et 8.</param>
    /// <param name="column">Indice de colonne entre 0 et 8.</param>
    public SudokuCell(int row, int column)
    {
        Row = row;
        Column = column;
    }

    /// <summary>
    /// Crée une cellule à partir d'une position.
    /// </summary>
    /// <param name="position">Position de la cellule dans la grille.</param>
    public SudokuCell(CellPosition position)
        : this(position.Row, position.Column)
    {
    }

    /// <summary>
    /// Indique si la cellule est vide.
    /// </summary>
    public bool IsEmpty => Value is null;

    /// <summary>
    /// Efface la valeur finale et les notes de la cellule.
    /// Cette méthode ne modifie pas la position de la cellule.
    /// </summary>
    public void Clear()
    {
        if (IsGiven)
        {
            return;
        }

        Value = null;
        CornerNotes.Clear();
        CenterNotes.Clear();
        Color = CellColor.None;
        HasError = false;
    }
}