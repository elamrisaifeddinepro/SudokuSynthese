using SudokuSynthese.Core.Commands;
using SudokuSynthese.Core.Models;

namespace SudokuSynthese.Core.Strategies;

/// <summary>
/// Interface représentant une stratégie de saisie.
/// 
/// Le rôle d'une stratégie est de transformer une action utilisateur
/// en commande exécutable et annulable.
/// </summary>
public interface IInputStrategy
{
    /// <summary>
    /// Crée une commande selon le mode de saisie actif.
    /// </summary>
    /// <param name="grid">Grille de Sudoku concernée.</param>
    /// <param name="selectedCells">Cellules actuellement sélectionnées.</param>
    /// <param name="input">Valeur saisie par l'utilisateur.</param>
    /// <returns>Une commande Sudoku exécutable et annulable.</returns>
    ISudokuCommand CreateCommand(
        SudokuGrid grid,
        IEnumerable<CellPosition> selectedCells,
        int input
    );
}