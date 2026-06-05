using Microsoft.VisualStudio.TestTools.UnitTesting;
using SudokuSynthese.Core.Models;
using SudokuSynthese.Core.Services;

namespace SudokuSynthese.Tests;

[TestClass]
public class SudokuGeneratorTests
{
    [TestMethod]
    public void GenerateNewPuzzle_Cree_Une_Grille_Avec_81_Cellules()
    {
        SudokuGenerator generator = new SudokuGenerator();

        SudokuGrid grid = generator.GenerateNewPuzzle(givensCount: 45);

        int cellCount = grid.GetAllCells().Count();

        Assert.AreEqual(81, cellCount);
    }

    [TestMethod]
    public void GenerateNewPuzzle_Cree_Des_Cellules_Donnees()
    {
        SudokuGenerator generator = new SudokuGenerator();

        SudokuGrid grid = generator.GenerateNewPuzzle(givensCount: 45);

        int givenCount = grid.GetAllCells().Count(cell => cell.IsGiven);

        Assert.AreEqual(45, givenCount);
    }

    [TestMethod]
    public void GenerateNewPuzzle_Les_Cellules_Donnees_Ont_Une_Valeur()
    {
        SudokuGenerator generator = new SudokuGenerator();

        SudokuGrid grid = generator.GenerateNewPuzzle(givensCount: 45);

        foreach (SudokuCell cell in grid.GetAllCells().Where(cell => cell.IsGiven))
        {
            Assert.IsNotNull(cell.Value);
            Assert.IsTrue(cell.Value >= 1 && cell.Value <= 9);
        }
    }

    [TestMethod]
    public void GenerateNewPuzzle_Les_Cellules_Non_Donnees_Sont_Vides()
    {
        SudokuGenerator generator = new SudokuGenerator();

        SudokuGrid grid = generator.GenerateNewPuzzle(givensCount: 45);

        foreach (SudokuCell cell in grid.GetAllCells().Where(cell => !cell.IsGiven))
        {
            Assert.IsNull(cell.Value);
        }
    }

    [TestMethod]
    public void GenerateNewPuzzle_Cree_Une_Grille_Valide()
    {
        SudokuGenerator generator = new SudokuGenerator();
        SudokuValidator validator = new SudokuValidator();

        SudokuGrid grid = generator.GenerateNewPuzzle(givensCount: 45);

        bool isValid = validator.IsValid(grid);

        Assert.IsTrue(isValid);
    }

    [TestMethod]
    public void GenerateNewPuzzle_Cree_Une_Grille_A_Solution_Unique()
    {
        SudokuGenerator generator = new SudokuGenerator();
        SudokuSolver solver = new SudokuSolver();

        SudokuGrid grid = generator.GenerateNewPuzzle(givensCount: 45);

        int[,] array = solver.ToArray(grid);

        int solutionCount = solver.CountSolutions(array, maxSolutions: 2);

        Assert.AreEqual(1, solutionCount);
    }
}