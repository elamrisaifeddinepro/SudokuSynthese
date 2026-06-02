using Microsoft.VisualStudio.TestTools.UnitTesting;
using SudokuSynthese.Core.Models;
using SudokuSynthese.Core.Services;

namespace SudokuSynthese.Tests;

[TestClass]
public class SaveLoadServiceTests
{
    [TestMethod]
    public void SerializeDeserialize_Conserve_Les_Valeurs()
    {
        SudokuGrid grid = new SudokuGrid();

        grid.GetCell(0, 0).Value = 5;
        grid.GetCell(0, 0).IsGiven = true;

        SaveLoadService service = new SaveLoadService();

        string json = service.Serialize(grid);
        SudokuGrid loadedGrid = service.Deserialize(json);

        Assert.AreEqual(5, loadedGrid.GetCell(0, 0).Value);
        Assert.IsTrue(loadedGrid.GetCell(0, 0).IsGiven);
    }

    [TestMethod]
    public void SerializeDeserialize_Conserve_Les_Notes_En_Coin()
    {
        SudokuGrid grid = new SudokuGrid();

        grid.GetCell(1, 1).CornerNotes.Add(2);
        grid.GetCell(1, 1).CornerNotes.Add(8);

        SaveLoadService service = new SaveLoadService();

        string json = service.Serialize(grid);
        SudokuGrid loadedGrid = service.Deserialize(json);

        SudokuCell loadedCell = loadedGrid.GetCell(1, 1);

        Assert.IsTrue(loadedCell.CornerNotes.Contains(2));
        Assert.IsTrue(loadedCell.CornerNotes.Contains(8));
        Assert.AreEqual(2, loadedCell.CornerNotes.Count);
    }

    [TestMethod]
    public void SerializeDeserialize_Conserve_Les_Notes_Centrales()
    {
        SudokuGrid grid = new SudokuGrid();

        grid.GetCell(2, 2).CenterNotes.Add(3);
        grid.GetCell(2, 2).CenterNotes.Add(6);

        SaveLoadService service = new SaveLoadService();

        string json = service.Serialize(grid);
        SudokuGrid loadedGrid = service.Deserialize(json);

        SudokuCell loadedCell = loadedGrid.GetCell(2, 2);

        Assert.IsTrue(loadedCell.CenterNotes.Contains(3));
        Assert.IsTrue(loadedCell.CenterNotes.Contains(6));
        Assert.AreEqual(2, loadedCell.CenterNotes.Count);
    }

    [TestMethod]
    public void SerializeDeserialize_Conserve_Les_Couleurs()
    {
        SudokuGrid grid = new SudokuGrid();

        grid.GetCell(3, 3).Color = CellColor.Blue;
        grid.GetCell(4, 4).Color = CellColor.Green;

        SaveLoadService service = new SaveLoadService();

        string json = service.Serialize(grid);
        SudokuGrid loadedGrid = service.Deserialize(json);

        Assert.AreEqual(CellColor.Blue, loadedGrid.GetCell(3, 3).Color);
        Assert.AreEqual(CellColor.Green, loadedGrid.GetCell(4, 4).Color);
    }
}