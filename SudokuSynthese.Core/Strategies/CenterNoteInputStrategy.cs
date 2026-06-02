using SudokuSynthese.Core.Commands;
using SudokuSynthese.Core.Models;

namespace SudokuSynthese.Core.Strategies;

/// <summary>
/// Stratégie de saisie pour ajouter ou supprimer une note centrale.
/// </summary>
public class CenterNoteInputStrategy : IInputStrategy
{
    /// <summary>
    /// Crée une commande qui ajoute ou supprime une note centrale
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
            compositeCommand.AddCommand(new ToggleCenterNoteCommand(cell, input));
        }

        return compositeCommand;
    }
}