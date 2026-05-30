namespace SudokuSynthese.Core.Models;

/// <summary>
/// Représente une grille complète de Sudoku composée de 81 cellules.
/// 
/// La grille contient 9 lignes et 9 colonnes.
/// Chaque case est représentée par un objet SudokuCell.
/// </summary>
public class SudokuGrid
{
    /// <summary>
    /// Nombre total de lignes dans une grille de Sudoku.
    /// </summary>
    public const int Size = 9;

    /// <summary>
    /// Tableau interne contenant les 81 cellules de la grille.
    /// </summary>
    private readonly SudokuCell[,] _cells = new SudokuCell[Size, Size];

    /// <summary>
    /// Crée une nouvelle grille vide de 9 x 9 cellules.
    /// </summary>
    public SudokuGrid()
    {
        for (int row = 0; row < Size; row++)
        {
            for (int column = 0; column < Size; column++)
            {
                _cells[row, column] = new SudokuCell(row, column);
            }
        }
    }

    /// <summary>
    /// Récupère une cellule à partir de sa ligne et de sa colonne.
    /// </summary>
    /// <param name="row">Indice de ligne entre 0 et 8.</param>
    /// <param name="column">Indice de colonne entre 0 et 8.</param>
    /// <returns>La cellule située à la position demandée.</returns>
    public SudokuCell GetCell(int row, int column)
    {
        ValidatePosition(row, column);

        return _cells[row, column];
    }

    /// <summary>
    /// Récupère une cellule à partir d'une position.
    /// </summary>
    /// <param name="position">Position de la cellule.</param>
    /// <returns>La cellule située à cette position.</returns>
    public SudokuCell GetCell(CellPosition position)
    {
        return GetCell(position.Row, position.Column);
    }

    /// <summary>
    /// Retourne toutes les cellules de la grille.
    /// Cette méthode permet de parcourir facilement les 81 cellules.
    /// </summary>
    /// <returns>Une collection contenant toutes les cellules.</returns>
    public IEnumerable<SudokuCell> GetAllCells()
    {
        for (int row = 0; row < Size; row++)
        {
            for (int column = 0; column < Size; column++)
            {
                yield return _cells[row, column];
            }
        }
    }

    /// <summary>
    /// Réinitialise toute la grille.
    /// 
    /// Les cellules données au départ ne sont pas supprimées par défaut,
    /// car leur méthode Clear() protège les cellules IsGiven.
    /// </summary>
    public void Clear()
    {
        foreach (SudokuCell cell in GetAllCells())
        {
            cell.Clear();
            cell.IsSelected = false;
            cell.HasError = false;
        }
    }

    /// <summary>
    /// Vide complètement la grille, y compris les cellules données au départ.
    /// 
    /// Cette méthode est utile pour charger une nouvelle partie
    /// ou remplacer entièrement la grille actuelle.
    /// </summary>
    public void Reset()
    {
        foreach (SudokuCell cell in GetAllCells())
        {
            cell.Value = null;
            cell.IsGiven = false;
            cell.CornerNotes.Clear();
            cell.CenterNotes.Clear();
            cell.Color = CellColor.None;
            cell.IsSelected = false;
            cell.HasError = false;
        }
    }

    /// <summary>
    /// Charge une grille à partir d'un tableau 9 x 9.
    /// 
    /// La valeur 0 représente une cellule vide.
    /// Les valeurs de 1 à 9 représentent des valeurs données au départ.
    /// </summary>
    /// <param name="values">Tableau de valeurs représentant la grille.</param>
    public void LoadFromArray(int[,] values)
    {
        if (values.GetLength(0) != Size || values.GetLength(1) != Size)
        {
            throw new ArgumentException("La grille doit contenir exactement 9 lignes et 9 colonnes.");
        }

        Reset();

        for (int row = 0; row < Size; row++)
        {
            for (int column = 0; column < Size; column++)
            {
                int value = values[row, column];

                if (value < 0 || value > 9)
                {
                    throw new ArgumentException("Les valeurs de la grille doivent être comprises entre 0 et 9.");
                }

                SudokuCell cell = GetCell(row, column);

                if (value != 0)
                {
                    cell.Value = value;
                    cell.IsGiven = true;
                }
            }
        }
    }

    /// <summary>
    /// Convertit la grille actuelle en tableau 9 x 9.
    /// 
    /// Les cellules vides sont représentées par 0.
    /// </summary>
    /// <returns>Un tableau contenant les valeurs actuelles de la grille.</returns>
    public int[,] ToArray()
    {
        int[,] values = new int[Size, Size];

        for (int row = 0; row < Size; row++)
        {
            for (int column = 0; column < Size; column++)
            {
                values[row, column] = _cells[row, column].Value ?? 0;
            }
        }

        return values;
    }

    /// <summary>
    /// Vérifie qu'une position est valide dans la grille.
    /// </summary>
    /// <param name="row">Indice de ligne.</param>
    /// <param name="column">Indice de colonne.</param>
    private static void ValidatePosition(int row, int column)
    {
        if (row < 0 || row >= Size)
        {
            throw new ArgumentOutOfRangeException(nameof(row), "La ligne doit être comprise entre 0 et 8.");
        }

        if (column < 0 || column >= Size)
        {
            throw new ArgumentOutOfRangeException(nameof(column), "La colonne doit être comprise entre 0 et 8.");
        }
    }
}