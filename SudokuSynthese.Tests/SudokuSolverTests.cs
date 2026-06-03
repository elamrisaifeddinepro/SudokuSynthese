using Microsoft.VisualStudio.TestTools.UnitTesting;
using SudokuSynthese.Core.Services;

namespace SudokuSynthese.Tests;

[TestClass]
public class SudokuSolverTests
{
    [TestMethod]
    public void IsMoveValid_Retourne_True_Si_Mouvement_Valide()
    {
        int[,] grid = new int[9, 9];

        SudokuSolver solver = new SudokuSolver();

        bool result = solver.IsMoveValid(grid, 0, 0, 5);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsMoveValid_Retourne_False_Si_Doublon_Dans_Ligne()
    {
        int[,] grid = new int[9, 9];

        grid[0, 3] = 5;

        SudokuSolver solver = new SudokuSolver();

        bool result = solver.IsMoveValid(grid, 0, 0, 5);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsMoveValid_Retourne_False_Si_Doublon_Dans_Colonne()
    {
        int[,] grid = new int[9, 9];

        grid[4, 0] = 7;

        SudokuSolver solver = new SudokuSolver();

        bool result = solver.IsMoveValid(grid, 0, 0, 7);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsMoveValid_Retourne_False_Si_Doublon_Dans_Bloc()
    {
        int[,] grid = new int[9, 9];

        grid[1, 1] = 9;

        SudokuSolver solver = new SudokuSolver();

        bool result = solver.IsMoveValid(grid, 0, 0, 9);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Solve_Resout_Une_Grille_Valide()
    {
        int[,] grid =
        {
            { 5, 3, 0, 0, 7, 0, 0, 0, 0 },
            { 6, 0, 0, 1, 9, 5, 0, 0, 0 },
            { 0, 9, 8, 0, 0, 0, 0, 6, 0 },
            { 8, 0, 0, 0, 6, 0, 0, 0, 3 },
            { 4, 0, 0, 8, 0, 3, 0, 0, 1 },
            { 7, 0, 0, 0, 2, 0, 0, 0, 6 },
            { 0, 6, 0, 0, 0, 0, 2, 8, 0 },
            { 0, 0, 0, 4, 1, 9, 0, 0, 5 },
            { 0, 0, 0, 0, 8, 0, 0, 7, 9 }
        };

        SudokuSolver solver = new SudokuSolver();

        bool solved = solver.Solve(grid);

        Assert.IsTrue(solved);

        for (int row = 0; row < 9; row++)
        {
            for (int column = 0; column < 9; column++)
            {
                Assert.IsTrue(grid[row, column] >= 1 && grid[row, column] <= 9);
            }
        }
    }

    [TestMethod]
    public void CountSolutions_Retourne_1_Pour_Grille_A_Solution_Unique()
    {
        int[,] grid =
        {
            { 5, 3, 0, 0, 7, 0, 0, 0, 0 },
            { 6, 0, 0, 1, 9, 5, 0, 0, 0 },
            { 0, 9, 8, 0, 0, 0, 0, 6, 0 },
            { 8, 0, 0, 0, 6, 0, 0, 0, 3 },
            { 4, 0, 0, 8, 0, 3, 0, 0, 1 },
            { 7, 0, 0, 0, 2, 0, 0, 0, 6 },
            { 0, 6, 0, 0, 0, 0, 2, 8, 0 },
            { 0, 0, 0, 4, 1, 9, 0, 0, 5 },
            { 0, 0, 0, 0, 8, 0, 0, 7, 9 }
        };

        SudokuSolver solver = new SudokuSolver();

        int count = solver.CountSolutions(grid);

        Assert.AreEqual(1, count);
    }
}