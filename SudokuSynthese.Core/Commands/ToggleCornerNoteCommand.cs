using SudokuSynthese.Core.Models;

namespace SudokuSynthese.Core.Commands;

/// <summary>
/// Commande permettant d'ajouter, supprimer ou remplacer une note en coin dans une cellule.
/// 
/// Règles métier :
/// - Une note en coin peut être n'importe quel chiffre entre 1 et 9.
/// - Une cellule peut contenir au maximum 4 notes en coin.
/// - Si la note existe déjà, elle est supprimée.
/// - Si la note n'existe pas et qu'il y a moins de 4 notes, elle est ajoutée.
/// - Si la note n'existe pas et qu'il y a déjà 4 notes, une ancienne note est remplacée.
/// 
/// Cette commande utilise le patron Memento pour permettre l'annulation.
/// </summary>
public class ToggleCornerNoteCommand : ISudokuCommand
{
    /// <summary>
    /// Nombre maximum de notes en coin autorisées dans une cellule.
    /// </summary>
    private const int MaxCornerNotes = 4;

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
    /// Il permet de restaurer exactement l'état précédent lors d'un Undo.
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
    /// Si la cellule est donnée au départ ou si elle contient une valeur finale,
    /// aucune note n'est modifiée.
    /// 
    /// Logique :
    /// - si la note existe déjà, on la supprime ;
    /// - si elle n'existe pas et qu'il y a moins de 4 notes, on l'ajoute ;
    /// - si elle n'existe pas et qu'il y a déjà 4 notes, on remplace une ancienne note.
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
            if (_cell.CornerNotes.Count >= MaxCornerNotes)
            {
                int noteToRemove = _cell.CornerNotes
                    .OrderBy(note => note)
                    .First();

                _cell.CornerNotes.Remove(noteToRemove);
            }

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