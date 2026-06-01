using SudokuSynthese.Core.Models;

namespace SudokuSynthese.Core.Commands;

/// <summary>
/// Commande permettant de changer la couleur d'une cellule.
/// 
/// Cette commande utilise le patron Memento pour sauvegarder l'ancien état
/// de la cellule avant modification, afin de permettre l'annulation.
/// </summary>
public class SetColorCommand : ISudokuCommand
{
    /// <summary>
    /// Cellule concernée par la commande.
    /// </summary>
    private readonly SudokuCell _cell;

    /// <summary>
    /// Nouvelle couleur à appliquer à la cellule.
    /// </summary>
    private readonly CellColor _newColor;

    /// <summary>
    /// Ancien état de la cellule avant l'exécution de la commande.
    /// </summary>
    private CellMemento? _previousState;

    /// <summary>
    /// Crée une commande de changement de couleur.
    /// </summary>
    /// <param name="cell">Cellule à modifier.</param>
    /// <param name="newColor">Nouvelle couleur à appliquer.</param>
    public SetColorCommand(SudokuCell cell, CellColor newColor)
    {
        ArgumentNullException.ThrowIfNull(cell);

        _cell = cell;
        _newColor = newColor;
    }

    /// <summary>
    /// Exécute la commande.
    /// 
    /// Étapes :
    /// 1. Sauvegarder l'ancien état de la cellule.
    /// 2. Appliquer la nouvelle couleur.
    /// </summary>
    public void Execute()
    {
        if (_cell.IsGiven)
        {
            return;
        }

        _previousState = _cell.CreateMemento();

        _cell.Color = _newColor;
        _cell.HasError = false;
    }

    /// <summary>
    /// Annule la commande en restaurant l'ancien état de la cellule.
    /// </summary>
    public void Undo()
    {
        if (_previousState is null)
        {
            return;
        }

        _cell.RestoreMemento(_previousState);
    }
}