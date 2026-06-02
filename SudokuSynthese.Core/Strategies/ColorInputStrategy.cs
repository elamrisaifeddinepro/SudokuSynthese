using SudokuSynthese.Core.Commands;
using SudokuSynthese.Core.Models;

namespace SudokuSynthese.Core.Strategies;

/// <summary>
/// Stratégie de saisie pour appliquer une couleur aux cellules sélectionnées.
/// 
/// Ici, le paramètre input est converti en CellColor.
/// Exemple :
/// input = 1 → CellColor.Blue
/// input = 2 → CellColor.Green
/// input = 0 → CellColor.None
/// </summary>
public class ColorInputStrategy : IInputStrategy
{
    /// <summary>
    /// Crée une commande qui applique une couleur à chaque cellule sélectionnée.
    /// </summary>
    public ISudokuCommand CreateCommand(
        SudokuGrid grid,
        IEnumerable<CellPosition> selectedCells,
        int input)
    {
        ArgumentNullException.ThrowIfNull(grid);
        ArgumentNullException.ThrowIfNull(selectedCells);

        if (!Enum.IsDefined(typeof(CellColor), input))
        {
            throw new ArgumentOutOfRangeException(
                nameof(input),
                "La couleur demandée n'existe pas.");
        }

        CellColor color = (CellColor)input;

        CompositeCommand compositeCommand = new CompositeCommand();

        foreach (CellPosition position in selectedCells)
        {
            SudokuCell cell = grid.GetCell(position);
            compositeCommand.AddCommand(new SetColorCommand(cell, color));
        }

        return compositeCommand;
    }
}