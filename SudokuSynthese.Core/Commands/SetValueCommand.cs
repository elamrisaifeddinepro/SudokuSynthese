using SudokuSynthese.Core.Models;

namespace SudokuSynthese.Core.Commands;

/// <summary>
/// Commande permettant de modifier la valeur finale d'une cellule.
/// 
/// Cette commande utilise le patron Memento pour sauvegarder l'ancien état
/// de la cellule avant modification, afin de permettre l'annulation.
/// </summary>
public class SetValueCommand : ISudokuCommand
{
    /// <summary>
    /// Cellule concernée par la commande.
    /// </summary>
    private readonly SudokuCell _cell;

    /// <summary>
    /// Nouvelle valeur à placer dans la cellule.
    /// Null signifie que l'on veut vider la cellule.
    /// </summary>
    private readonly int? _newValue;

    /// <summary>
    /// Ancien état de la cellule avant l'exécution de la commande.
    /// Il sera utilisé pour annuler l'action.
    /// </summary>
    private CellMemento? _previousState;

    /// <summary>
    /// Crée une commande de modification de valeur finale.
    /// </summary>
    /// <param name="cell">Cellule à modifier.</param>
    /// <param name="newValue">Nouvelle valeur finale entre 1 et 9, ou null pour vider la cellule.</param>
    public SetValueCommand(SudokuCell cell, int? newValue)
    {
        ArgumentNullException.ThrowIfNull(cell);

        if (newValue is not null && (newValue < 1 || newValue > 9))
        {
            throw new ArgumentOutOfRangeException(
                nameof(newValue),
                "La valeur doit être comprise entre 1 et 9, ou être null pour vider la cellule.");
        }

        _cell = cell;
        _newValue = newValue;
    }

    /// <summary>
    /// Exécute la commande.
    /// 
    /// Étapes :
    /// 1. Sauvegarder l'ancien état de la cellule.
    /// 2. Modifier la valeur finale.
    /// 3. Effacer les notes en coin.
    /// 4. Effacer les notes au centre.
    /// </summary>
    public void Execute()
    {
        if (_cell.IsGiven)
        {
            return;
        }

        _previousState = _cell.CreateMemento();

        _cell.Value = _newValue;

        _cell.CornerNotes.Clear();
        _cell.CenterNotes.Clear();

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