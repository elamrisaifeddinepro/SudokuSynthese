using SudokuSynthese.Core.Models;

namespace SudokuSynthese.Core.Services;

/// <summary>
/// Service responsable de la résolution d'une grille de Sudoku.
/// 
/// Il utilise l'algorithme classique de backtracking.
/// Ce service travaille avec une représentation simple int[9,9] :
/// - 0 signifie cellule vide ;
/// - 1 à 9 signifie valeur placée.
/// </summary>
public class SudokuSolver
{
    /// <summary>
    /// Vérifie si une valeur peut être placée à une position donnée
    /// sans violer les règles du Sudoku.
    /// </summary>
    public bool IsMoveValid(int[,] grid, int row, int column, int value)
    {
        if (grid is null)
        {
            throw new ArgumentNullException(nameof(grid));
        }

        if (value < 1 || value > 9)
        {
            return false;
        }

        // Vérification de la ligne et de la colonne
        for (int i = 0; i < 9; i++)
        {
            if (i != column && grid[row, i] == value)
            {
                return false;
            }

            if (i != row && grid[i, column] == value)
            {
                return false;
            }
        }

        // Vérification du bloc 3x3
        int blockStartRow = (row / 3) * 3;
        int blockStartColumn = (column / 3) * 3;

        for (int r = 0; r < 3; r++)
        {
            for (int c = 0; c < 3; c++)
            {
                int currentRow = blockStartRow + r;
                int currentColumn = blockStartColumn + c;

                if (currentRow == row && currentColumn == column)
                {
                    continue;
                }

                if (grid[currentRow, currentColumn] == value)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Résout une grille de Sudoku.
    /// 
    /// La grille reçue est modifiée directement.
    /// Retourne true si une solution est trouvée.
    /// </summary>
    public bool Solve(int[,] grid)
    {
        if (grid is null)
        {
            throw new ArgumentNullException(nameof(grid));
        }

        for (int row = 0; row < 9; row++)
        {
            for (int column = 0; column < 9; column++)
            {
                if (grid[row, column] == 0)
                {
                    for (int value = 1; value <= 9; value++)
                    {
                        if (IsMoveValid(grid, row, column, value))
                        {
                            grid[row, column] = value;

                            if (Solve(grid))
                            {
                                return true;
                            }

                            grid[row, column] = 0;
                        }
                    }

                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Compte le nombre de solutions possibles d'une grille.
    /// 
    /// On s'arrête dès qu'on atteint maxSolutions.
    /// Cela permet de vérifier rapidement si une grille a plus d'une solution.
    /// </summary>
    public int CountSolutions(int[,] grid, int maxSolutions = 2)
    {
        if (grid is null)
        {
            throw new ArgumentNullException(nameof(grid));
        }

        int[,] copy = CloneGrid(grid);

        int count = 0;
        CountSolutionsInternal(copy, ref count, maxSolutions);

        return count;
    }

    /// <summary>
    /// Convertit une SudokuGrid métier vers un tableau int[9,9].
    /// </summary>
    public int[,] ToArray(SudokuGrid grid)
    {
        if (grid is null)
        {
            throw new ArgumentNullException(nameof(grid));
        }

        int[,] result = new int[9, 9];

        foreach (SudokuCell cell in grid.GetAllCells())
        {
            result[cell.Row, cell.Column] = cell.Value ?? 0;
        }

        return result;
    }

    private bool CountSolutionsInternal(int[,] grid, ref int count, int maxSolutions)
    {
        if (count >= maxSolutions)
        {
            return true;
        }

        for (int row = 0; row < 9; row++)
        {
            for (int column = 0; column < 9; column++)
            {
                if (grid[row, column] == 0)
                {
                    for (int value = 1; value <= 9; value++)
                    {
                        if (IsMoveValid(grid, row, column, value))
                        {
                            grid[row, column] = value;

                            CountSolutionsInternal(grid, ref count, maxSolutions);

                            grid[row, column] = 0;

                            if (count >= maxSolutions)
                            {
                                return true;
                            }
                        }
                    }

                    return false;
                }
            }
        }

        count++;
        return count >= maxSolutions;
    }

    private static int[,] CloneGrid(int[,] grid)
    {
        int[,] copy = new int[9, 9];

        for (int row = 0; row < 9; row++)
        {
            for (int column = 0; column < 9; column++)
            {
                copy[row, column] = grid[row, column];
            }
        }

        return copy;
    }
}