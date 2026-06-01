using SudokuSynthese.Core.Models;

namespace SudokuSynthese.Core.Commands;

/// <summary>
/// Commande permettant d'ajouter ou de supprimer une note en coin dans une cellule.
/// 
/// Si la note existe déjà, elle est supprimée.
/// Si la note n'existe pas, elle est ajoutée.
/// 
/// Cette commande utilise le patron Memento pour permettre l'annulation.
/// </summary>
public class ToggleCornerNoteCommand : ISudokuCommand
{
    /// <summary>
    /// Cellule concernée par la commande.
    /// </summary>
    private readonly SudokuCell _cell;

    /// <summary>
    /// Note à ajouter ou supprimer.
    /// La valeur doit être comprise entre 1 et 9.
    /// </summary>
    private readonly int _note;

    /// <summary>
    /// Ancien état de la cellule avant l'exécution de la commande.
    /// </summary>
    private CellMemento? _previousState;

    /// <summary>
    /// Crée une commande de modification d'une note en coin.
    /// </summary>
    /// <param name="cell">Cellule à modifier.</param>
    /// <param name="note">Note à ajouter ou supprimer, entre 1 et 9.</param>
    public ToggleCornerNoteCommand(SudokuCell cell, int note)
    {
        ArgumentNullException.ThrowIfNull(cell);

        if (note < 1 || note > 9)
        {
            throw new ArgumentOutOfRangeException(
                nameof(note),
                "La note doit être comprise entre 1 et 9.");
        }

        _cell = cell;
        _note = note;
    }

    /// <summary>
    /// Exécute la commande.
    /// 
    /// Si la cellule est donnée au départ, aucune modification n'est faite.
    /// Si la cellule contient déjà une valeur finale, on ne modifie pas les notes.
    /// Sinon :
    /// - si la note existe déjà, on la supprime ;
    /// - si elle n'existe pas, on l'ajoute.
    /// </summary>
    public void Execute()
    {
        if (_cell.IsGiven || _cell.Value is not null)
        {
            return;
        }

        _previousState = _cell.CreateMemento();

        if (_cell.CornerNotes.Contains(_note))
        {
            _cell.CornerNotes.Remove(_note);
        }
        else
        {
            _cell.CornerNotes.Add(_note);
        }

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