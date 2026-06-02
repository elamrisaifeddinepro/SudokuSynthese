namespace SudokuSynthese.Core.Commands;

/// <summary>
/// Commande composée permettant de regrouper plusieurs commandes en une seule action.
/// 
/// Exemple : appliquer une couleur à plusieurs cellules sélectionnées.
/// Chaque cellule reçoit sa propre commande, mais l'ensemble est traité comme une seule action.
/// </summary>
public class CompositeCommand : ISudokuCommand
{
    /// <summary>
    /// Liste des commandes regroupées.
    /// </summary>
    private readonly List<ISudokuCommand> _commands = new();

    /// <summary>
    /// Crée une commande composée vide.
    /// </summary>
    public CompositeCommand()
    {
    }

    /// <summary>
    /// Crée une commande composée avec une liste de commandes existantes.
    /// </summary>
    /// <param name="commands">Commandes à ajouter.</param>
    public CompositeCommand(IEnumerable<ISudokuCommand> commands)
    {
        ArgumentNullException.ThrowIfNull(commands);

        _commands.AddRange(commands);
    }

    /// <summary>
    /// Ajoute une commande dans la commande composée.
    /// </summary>
    /// <param name="command">Commande à ajouter.</param>
    public void AddCommand(ISudokuCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        _commands.Add(command);
    }

    /// <summary>
    /// Exécute toutes les commandes dans l'ordre d'ajout.
    /// </summary>
    public void Execute()
    {
        foreach (ISudokuCommand command in _commands)
        {
            command.Execute();
        }
    }

    /// <summary>
    /// Annule toutes les commandes dans l'ordre inverse.
    /// </summary>
    public void Undo()
    {
        for (int i = _commands.Count - 1; i >= 0; i--)
        {
            _commands[i].Undo();
        }
    }

    /// <summary>
    /// Indique si la commande composée ne contient aucune commande.
    /// </summary>
    public bool IsEmpty => _commands.Count == 0;

    /// <summary>
    /// Nombre de commandes contenues dans la commande composée.
    /// </summary>
    public int Count => _commands.Count;
}