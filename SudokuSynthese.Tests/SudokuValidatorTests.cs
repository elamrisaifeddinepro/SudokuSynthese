using Microsoft.VisualStudio.TestTools.UnitTesting;
using SudokuSynthese.Core.Models;
using SudokuSynthese.Core.Services;

namespace SudokuSynthese.Tests;

[TestClass]
public class SudokuValidatorTests
{
    [TestMethod]
    public void Detecte_Doublon_Dans_Ligne()
    {
        SudokuGrid grid = new SudokuGrid();

        grid.GetCell(0, 0).Value = 5;
        grid.GetCell(0, 4).Value = 5;

        SudokuValidator validator = new SudokuValidator();

        IReadOnlyCollection<CellPosition> invalidCells = validator.GetInvalidCells(grid);

        Assert.IsFalse(validator.IsValid(grid));
        Assert.IsTrue(invalidCells.Contains(new CellPosition(0, 0)));
        Assert.IsTrue(invalidCells.Contains(new CellPosition(0, 4)));
    }

    [TestMethod]
    public void Detecte_Doublon_Dans_Colonne()
    {
        SudokuGrid grid = new SudokuGrid();

        grid.GetCell(0, 2).Value = 7;
        grid.GetCell(6, 2).Value = 7;

        SudokuValidator validator = new SudokuValidator();

        IReadOnlyCollection<CellPosition> invalidCells = validator.GetInvalidCells(grid);

        Assert.IsFalse(validator.IsValid(grid));
        Assert.IsTrue(invalidCells.Contains(new CellPosition(0, 2)));
        Assert.IsTrue(invalidCells.Contains(new CellPosition(6, 2)));
    }

    [TestMethod]
    public void Detecte_Doublon_Dans_Bloc()
    {
        SudokuGrid grid = new SudokuGrid();

        grid.GetCell(0, 0).Value = 9;
        grid.GetCell(2, 2).Value = 9;

        SudokuValidator validator = new SudokuValidator();

        IReadOnlyCollection<CellPosition> invalidCells = validator.GetInvalidCells(grid);

        Assert.IsFalse(validator.IsValid(grid));
        Assert.IsTrue(invalidCells.Contains(new CellPosition(0, 0)));
        Assert.IsTrue(invalidCells.Contains(new CellPosition(2, 2)));
    }

    [TestMethod]
    public void Ignore_Les_Cases_Vides()
    {
        SudokuGrid grid = new SudokuGrid();

        SudokuValidator validator = new SudokuValidator();

        IReadOnlyCollection<CellPosition> invalidCells = validator.GetInvalidCells(grid);

        Assert.IsTrue(validator.IsValid(grid));
        Assert.AreEqual(0, invalidCells.Count);
    }

    [TestMethod]
    public void Ignore_Les_Notes()
    {
        SudokuGrid grid = new SudokuGrid();

        grid.GetCell(0, 0).CornerNotes.Add(5);
        grid.GetCell(0, 1).CornerNotes.Add(5);

        grid.GetCell(1, 0).CenterNotes.Add(7);
        grid.GetCell(1, 1).CenterNotes.Add(7);

        SudokuValidator validator = new SudokuValidator();

        IReadOnlyCollection<CellPosition> invalidCells = validator.GetInvalidCells(grid);

        Assert.IsTrue(validator.IsValid(grid));
        Assert.AreEqual(0, invalidCells.Count);
    }

    [TestMethod]
    public void Retourne_Aucune_Erreur_Si_Grille_Valide()
    {
        SudokuGrid grid = new SudokuGrid();

        grid.GetCell(0, 0).Value = 1;
        grid.GetCell(0, 1).Value = 2;
        grid.GetCell(0, 2).Value = 3;

        grid.GetCell(1, 0).Value = 4;
        grid.GetCell(1, 1).Value = 5;
        grid.GetCell(1, 2).Value = 6;

        grid.GetCell(2, 0).Value = 7;
        grid.GetCell(2, 1).Value = 8;
        grid.GetCell(2, 2).Value = 9;

        SudokuValidator validator = new SudokuValidator();

        IReadOnlyCollection<CellPosition> invalidCells = validator.GetInvalidCells(grid);

        Assert.IsTrue(validator.IsValid(grid));
        Assert.AreEqual(0, invalidCells.Count);
    }
}