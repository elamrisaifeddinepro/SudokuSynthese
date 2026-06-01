using SudokuSynthese.Core.Commands;

namespace SudokuSynthese.Core.Managers;

/// <summary>
/// Gestionnaire responsable de l'historique Undo / Redo.
/// 
/// Il mémorise les commandes exécutées afin de permettre
/// l'annulation et la réexécution des actions du joueur.
/// </summary>
public class UndoRedoManager
{
    /// <summary>
    /// Pile des commandes pouvant être annulées.
    /// La dernière commande exécutée est au sommet de la pile.
    /// </summary>
    private readonly Stack<ISudokuCommand> _undoStack = new();

    /// <summary>
    /// Pile des commandes pouvant être réexécutées après un Undo.
    /// </summary>
    private readonly Stack<ISudokuCommand> _redoStack = new();

    /// <summary>
    /// Indique s'il existe au moins une action à annuler.
    /// </summary>
    public bool CanUndo => _undoStack.Count > 0;

    /// <summary>
    /// Indique s'il existe au moins une action à refaire.
    /// </summary>
    public bool CanRedo => _redoStack.Count > 0;

    /// <summary>
    /// Nombre d'actions actuellement disponibles pour Undo.
    /// </summary>
    public int UndoCount => _undoStack.Count;

    /// <summary>
    /// Nombre d'actions actuellement disponibles pour Redo.
    /// </summary>
    public int RedoCount => _redoStack.Count;

    /// <summary>
    /// Exécute une nouvelle commande normale.
    /// 
    /// Comportement :
    /// 1. Exécuter la commande.
    /// 2. Ajouter la commande dans la pile Undo.
    /// 3. Vider la pile Redo.
    /// 
    /// La pile Redo est vidée parce qu'une nouvelle action après un Undo
    /// remplace l'ancien chemin d'historique.
    /// </summary>
    /// <param name="command">Commande à exécuter.</param>
    public void ExecuteCommand(ISudokuCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        command.Execute();

        _undoStack.Push(command);
        _redoStack.Clear();
    }

    /// <summary>
    /// Annule la dernière commande exécutée.
    /// 
    /// Comportement :
    /// 1. Retirer la dernière commande de la pile Undo.
    /// 2. Appeler Undo() sur cette commande.
    /// 3. Ajouter cette commande dans la pile Redo.
    /// </summary>
    public void Undo()
    {
        if (!CanUndo)
        {
            return;
        }

        ISudokuCommand command = _undoStack.Pop();

        command.Undo();

        _redoStack.Push(command);
    }

    /// <summary>
    /// Réexécute la dernière commande annulée.
    /// 
    /// Comportement :
    /// 1. Retirer la dernière commande de la pile Redo.
    /// 2. Appeler Execute() sur cette commande.
    /// 3. Ajouter cette commande dans la pile Undo.
    /// </summary>
    public void Redo()
    {
        if (!CanRedo)
        {
            return;
        }

        ISudokuCommand command = _redoStack.Pop();

        command.Execute();

        _undoStack.Push(command);
    }

    /// <summary>
    /// Vide complètement l'historique Undo et Redo.
    /// 
    /// Utile lors du chargement d'une nouvelle grille ou d'une nouvelle partie.
    /// </summary>
    public void Clear()
    {
        _undoStack.Clear();
        _redoStack.Clear();
    }
}