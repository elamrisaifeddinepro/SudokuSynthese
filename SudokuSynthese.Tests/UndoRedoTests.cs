using Microsoft.VisualStudio.TestTools.UnitTesting;
using SudokuSynthese.Core.Commands;
using SudokuSynthese.Core.Managers;
using SudokuSynthese.Core.Models;

namespace SudokuSynthese.Tests;

[TestClass]
public class UndoRedoTests
{
    [TestMethod]
    public void SetValue_Undo_Restaure_Ancienne_Valeur()
    {
        SudokuCell cell = new SudokuCell(0, 0);
        cell.Value = 3;

        UndoRedoManager manager = new UndoRedoManager();

        manager.ExecuteCommand(new SetValueCommand(cell, 8));

        Assert.AreEqual(8, cell.Value);

        manager.Undo();

        Assert.AreEqual(3, cell.Value);
    }

    [TestMethod]
    public void SetValue_Redo_Restaure_Nouvelle_Valeur()
    {
        SudokuCell cell = new SudokuCell(0, 0);
        cell.Value = 3;

        UndoRedoManager manager = new UndoRedoManager();

        manager.ExecuteCommand(new SetValueCommand(cell, 8));
        manager.Undo();
        manager.Redo();

        Assert.AreEqual(8, cell.Value);
    }

    [TestMethod]
    public void SetColor_Undo_Restaure_Ancienne_Couleur()
    {
        SudokuCell cell = new SudokuCell(0, 0);
        cell.Color = CellColor.Green;

        UndoRedoManager manager = new UndoRedoManager();

        manager.ExecuteCommand(new SetColorCommand(cell, CellColor.Blue));

        Assert.AreEqual(CellColor.Blue, cell.Color);

        manager.Undo();

        Assert.AreEqual(CellColor.Green, cell.Color);
    }

    [TestMethod]
    public void ToggleCornerNote_Undo_Restaure_Ancien_Etat()
    {
        SudokuCell cell = new SudokuCell(0, 0);
        cell.CornerNotes.Add(2);

        UndoRedoManager manager = new UndoRedoManager();

        manager.ExecuteCommand(new ToggleCornerNoteCommand(cell, 5));

        Assert.IsTrue(cell.CornerNotes.Contains(2));
        Assert.IsTrue(cell.CornerNotes.Contains(5));

        manager.Undo();

        Assert.IsTrue(cell.CornerNotes.Contains(2));
        Assert.IsFalse(cell.CornerNotes.Contains(5));
        Assert.AreEqual(1, cell.CornerNotes.Count);
    }

    [TestMethod]
    public void CompositeCommand_Undo_Restaure_Toutes_Les_Cellules()
    {
        SudokuCell cell1 = new SudokuCell(0, 0);
        SudokuCell cell2 = new SudokuCell(0, 1);
        SudokuCell cell3 = new SudokuCell(0, 2);

        cell1.Color = CellColor.None;
        cell2.Color = CellColor.Green;
        cell3.Color = CellColor.Yellow;

        CompositeCommand compositeCommand = new CompositeCommand();

        compositeCommand.AddCommand(new SetColorCommand(cell1, CellColor.Blue));
        compositeCommand.AddCommand(new SetColorCommand(cell2, CellColor.Blue));
        compositeCommand.AddCommand(new SetColorCommand(cell3, CellColor.Blue));

        UndoRedoManager manager = new UndoRedoManager();

        manager.ExecuteCommand(compositeCommand);

        Assert.AreEqual(CellColor.Blue, cell1.Color);
        Assert.AreEqual(CellColor.Blue, cell2.Color);
        Assert.AreEqual(CellColor.Blue, cell3.Color);

        manager.Undo();

        Assert.AreEqual(CellColor.None, cell1.Color);
        Assert.AreEqual(CellColor.Green, cell2.Color);
        Assert.AreEqual(CellColor.Yellow, cell3.Color);
    }

    [TestMethod]
    public void Nouvelle_Action_Apres_Undo_Vide_Redo()
    {
        SudokuCell cell = new SudokuCell(0, 0);

        UndoRedoManager manager = new UndoRedoManager();

        manager.ExecuteCommand(new SetValueCommand(cell, 5));

        Assert.AreEqual(5, cell.Value);
        Assert.IsTrue(manager.CanUndo);
        Assert.IsFalse(manager.CanRedo);

        manager.Undo();

        Assert.IsNull(cell.Value);
        Assert.IsTrue(manager.CanRedo);

        manager.ExecuteCommand(new SetValueCommand(cell, 7));

        Assert.AreEqual(7, cell.Value);
        Assert.IsFalse(manager.CanRedo);
        Assert.AreEqual(0, manager.RedoCount);
    }
}