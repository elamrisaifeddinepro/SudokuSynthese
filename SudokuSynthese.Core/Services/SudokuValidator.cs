using SudokuSynthese.Core.Models;

namespace SudokuSynthese.Core.Services;

/// <summary>
/// Service responsable de la validation des règles du Sudoku.
/// 
/// Il détecte les conflits entre les valeurs finales des cellules.
/// Les notes en coin et les notes au centre ne sont pas prises en compte.
/// </summary>
public class SudokuValidator
{
    /// <summary>
    /// Retourne toutes les positions des cellules invalides dans la grille.
    /// 
    /// Une cellule est considérée comme invalide si sa valeur finale est en conflit
    /// avec une autre cellule de même valeur dans :
    /// - la même ligne ;
    /// - la même colonne ;
    /// - le même bloc 3x3.
    /// </summary>
    /// <param name="grid">Grille de Sudoku à valider.</param>
    /// <returns>Liste des positions des cellules en conflit.</returns>
    public IReadOnlyCollection<CellPosition> GetInvalidCells(SudokuGrid grid)
    {
        ArgumentNullException.ThrowIfNull(grid);

        HashSet<CellPosition> invalidPositions = new();

        CheckRows(grid, invalidPositions);
        CheckColumns(grid, invalidPositions);
        CheckBlocks(grid, invalidPositions);

        return invalidPositions.ToList().AsReadOnly();
    }

    /// <summary>
    /// Indique si la grille respecte les règles du Sudoku.
    /// 
    /// La grille est valide si aucune valeur finale n'est en conflit.
    /// Les cellules vides sont ignorées.
    /// Les notes sont ignorées.
    /// </summary>
    /// <param name="grid">Grille de Sudoku à valider.</param>
    /// <returns>True si la grille ne contient aucun conflit, sinon False.</returns>
    public bool IsValid(SudokuGrid grid)
    {
        return GetInvalidCells(grid).Count == 0;
    }

    /// <summary>
    /// Vérifie les doublons dans chaque ligne.
    /// </summary>
    private static void CheckRows(SudokuGrid grid, HashSet<CellPosition> invalidPositions)
    {
        for (int row = 0; row < SudokuGrid.Size; row++)
        {
            Dictionary<int, List<SudokuCell>> cellsByValue = new();

            for (int column = 0; column < SudokuGrid.Size; column++)
            {
                SudokuCell cell = grid.GetCell(row, column);
                AddCellIfItHasFinalValue(cell, cellsByValue);
            }

            AddConflictingCells(cellsByValue, invalidPositions);
        }
    }

    /// <summary>
    /// Vérifie les doublons dans chaque colonne.
    /// </summary>
    private static void CheckColumns(SudokuGrid grid, HashSet<CellPosition> invalidPositions)
    {
        for (int column = 0; column < SudokuGrid.Size; column++)
        {
            Dictionary<int, List<SudokuCell>> cellsByValue = new();

            for (int row = 0; row < SudokuGrid.Size; row++)
            {
                SudokuCell cell = grid.GetCell(row, column);
                AddCellIfItHasFinalValue(cell, cellsByValue);
            }

            AddConflictingCells(cellsByValue, invalidPositions);
        }
    }

    /// <summary>
    /// Vérifie les doublons dans chaque bloc 3x3.
    /// </summary>
    private static void CheckBlocks(SudokuGrid grid, HashSet<CellPosition> invalidPositions)
    {
        for (int startRow = 0; startRow < SudokuGrid.Size; startRow += 3)
        {
            for (int startColumn = 0; startColumn < SudokuGrid.Size; startColumn += 3)
            {
                Dictionary<int, List<SudokuCell>> cellsByValue = new();

                for (int row = startRow; row < startRow + 3; row++)
                {
                    for (int column = startColumn; column < startColumn + 3; column++)
                    {
                        SudokuCell cell = grid.GetCell(row, column);
                        AddCellIfItHasFinalValue(cell, cellsByValue);
                    }
                }

                AddConflictingCells(cellsByValue, invalidPositions);
            }
        }
    }

    /// <summary>
    /// Ajoute une cellule dans le dictionnaire seulement si elle contient une valeur finale.
    /// 
    /// Les cellules vides sont ignorées.
    /// </summary>
    private static void AddCellIfItHasFinalValue(
        SudokuCell cell,
        Dictionary<int, List<SudokuCell>> cellsByValue)
    {
        if (cell.Value is null)
        {
            return;
        }

        int value = cell.Value.Value;

        if (!cellsByValue.ContainsKey(value))
        {
            cellsByValue[value] = new List<SudokuCell>();
        }

        cellsByValue[value].Add(cell);
    }

    /// <summary>
    /// Ajoute aux positions invalides toutes les cellules ayant une valeur en doublon.
    /// </summary>
    private static void AddConflictingCells(
        Dictionary<int, List<SudokuCell>> cellsByValue,
        HashSet<CellPosition> invalidPositions)
    {
        foreach (List<SudokuCell> cellsWithSameValue in cellsByValue.Values)
        {
            if (cellsWithSameValue.Count <= 1)
            {
                continue;
            }

            foreach (SudokuCell cell in cellsWithSameValue)
            {
                invalidPositions.Add(new CellPosition(cell.Row, cell.Column));
            }
        }
    }
}