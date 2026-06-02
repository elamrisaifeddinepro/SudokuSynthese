using SudokuSynthese.Core.Commands;
using SudokuSynthese.Core.Models;

namespace SudokuSynthese.Core.Strategies;

/// <summary>
/// Stratégie de saisie pour ajouter ou supprimer une note en coin.
/// </summary>
public class CornerNoteInputStrategy : IInputStrategy
{
    /// <summary>
    /// Crée une commande qui ajoute ou supprime une note en coin
    /// dans chaque cellule sélectionnée.
    /// </summary>
    public ISudokuCommand CreateCommand(
        SudokuGrid grid,
        IEnumerable<CellPosition> selectedCells,
        int input)
    {
        ArgumentNullException.ThrowIfNull(grid);
        ArgumentNullException.ThrowIfNull(selectedCells);

        if (input < 1 || input > 9)
        {
            throw new ArgumentOutOfRangeException(
                nameof(input),
                "La note doit être comprise entre 1 et 9.");
        }

        CompositeCommand compositeCommand = new CompositeCommand();

        foreach (CellPosition position in selectedCells)
        {
            SudokuCell cell = grid.GetCell(position);
            compositeCommand.AddCommand(new ToggleCornerNoteCommand(cell, input));
        }

        return compositeCommand;
    }
}