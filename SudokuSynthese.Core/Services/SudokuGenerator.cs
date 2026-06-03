using SudokuSynthese.Core.Models;

namespace SudokuSynthese.Core.Services;

/// <summary>
/// Service responsable de la génération d'une vraie grille de Sudoku jouable.
/// 
/// Logique :
/// 1. Générer une grille complète valide.
/// 2. Retirer progressivement des valeurs.
/// 3. Vérifier que la grille garde une solution unique.
/// 4. Retourner une SudokuGrid avec des cellules données IsGiven = true.
/// </summary>
public class SudokuGenerator
{
    private readonly Random _random = new();
    private readonly SudokuSolver _solver = new();

    /// <summary>
    /// Génère une nouvelle grille Sudoku jouable.
    /// 
    /// Le paramètre givensCount représente environ le nombre de chiffres visibles au départ.
    /// Plus il est bas, plus la grille est difficile.
    /// </summary>
    public SudokuGrid GenerateNewPuzzle(int givensCount = 35)
    {
        if (givensCount < 17 || givensCount > 81)
        {
            throw new ArgumentOutOfRangeException(
                nameof(givensCount),
                "Le nombre de chiffres donnés doit être entre 17 et 81.");
        }

        int[,] fullGrid = new int[9, 9];

        FillGrid(fullGrid);

        int[,] puzzle = CloneGrid(fullGrid);

        RemoveValuesWhileKeepingUniqueSolution(puzzle, givensCount);

        return ConvertToSudokuGrid(puzzle);
    }

    /// <summary>
    /// Remplit entièrement une grille valide.
    /// </summary>
    private bool FillGrid(int[,] grid)
    {
        for (int row = 0; row < 9; row++)
        {
            for (int column = 0; column < 9; column++)
            {
                if (grid[row, column] == 0)
                {
                    List<int> numbers = Enumerable.Range(1, 9)
                        .OrderBy(_ => _random.Next())
                        .ToList();

                    foreach (int value in numbers)
                    {
                        if (_solver.IsMoveValid(grid, row, column, value))
                        {
                            grid[row, column] = value;

                            if (FillGrid(grid))
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
    /// Retire des valeurs tout en gardant une solution unique.
    /// </summary>
    private void RemoveValuesWhileKeepingUniqueSolution(int[,] puzzle, int givensCount)
    {
        List<CellPosition> positions = new();

        for (int row = 0; row < 9; row++)
        {
            for (int column = 0; column < 9; column++)
            {
                positions.Add(new CellPosition(row, column));
            }
        }

        positions = positions
            .OrderBy(_ => _random.Next())
            .ToList();

        int currentGivens = 81;

        foreach (CellPosition position in positions)
        {
            if (currentGivens <= givensCount)
            {
                break;
            }

            int oldValue = puzzle[position.Row, position.Column];

            if (oldValue == 0)
            {
                continue;
            }

            puzzle[position.Row, position.Column] = 0;

            int solutionCount = _solver.CountSolutions(puzzle, maxSolutions: 2);

            if (solutionCount != 1)
            {
                puzzle[position.Row, position.Column] = oldValue;
            }
            else
            {
                currentGivens--;
            }
        }
    }

    /// <summary>
    /// Convertit un tableau int[9,9] vers une SudokuGrid métier.
    /// Les valeurs présentes deviennent des cellules données au départ.
    /// </summary>
    private static SudokuGrid ConvertToSudokuGrid(int[,] puzzle)
    {
        SudokuGrid grid = new SudokuGrid();

        for (int row = 0; row < 9; row++)
        {
            for (int column = 0; column < 9; column++)
            {
                int value = puzzle[row, column];

                SudokuCell cell = grid.GetCell(row, column);

                if (value != 0)
                {
                    cell.Value = value;
                    cell.IsGiven = true;
                }
                else
                {
                    cell.Value = null;
                    cell.IsGiven = false;
                }

                cell.CornerNotes.Clear();
                cell.CenterNotes.Clear();
                cell.Color = CellColor.None;
                cell.IsSelected = false;
                cell.HasError = false;
            }
        }

        return grid;
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