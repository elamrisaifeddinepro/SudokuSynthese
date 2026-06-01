namespace SudokuSynthese.Core.Commands;

/// <summary>
/// Représente une action exécutable et annulable dans le jeu de Sudoku.
/// 
/// Cette interface est la base du patron de conception Command.
/// Chaque action du joueur sera représentée par une commande.
/// </summary>
public interface ISudokuCommand
{
    /// <summary>
    /// Exécute l'action.
    /// Exemple : placer une valeur, ajouter une note ou colorer une cellule.
    /// </summary>
    void Execute();

    /// <summary>
    /// Annule l'action et restaure l'état précédent.
    /// </summary>
    void Undo();
}