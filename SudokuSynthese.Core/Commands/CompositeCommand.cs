namespace SudokuSynthese.Core.Commands;

/// <summary>
/// Commande composée permettant de regrouper plusieurs commandes
/// et de les traiter comme une seule action.
/// 
/// Exemple : appliquer une couleur à plusieurs cellules sélectionnées.
/// Chaque cellule reçoit sa propre commande, mais l'ensemble est annulé
/// en une seule opération Undo.
/// </summary>
public class CompositeCommand : ISudokuCommand
{
    /// <summary>
    /// Liste des commandes à exécuter ensemble.
    /// </summary>
    private readonly List<ISudokuCommand> _commands = new();

    /// <summary>
    /// Crée une commande composée vide.
    /// Les commandes pourront être ajoutées ensuite avec Add().
    /// </summary>
    public CompositeCommand()
    {
    }

    /// <summary>
    /// Crée une commande composée à partir d'une liste de commandes.
    /// </summary>
    /// <param name="commands">Commandes à regrouper.</param>
    public CompositeCommand(IEnumerable<ISudokuCommand> commands)
    {
        ArgumentNullException.ThrowIfNull(commands);

        _commands.AddRange(commands);
    }

    /// <summary>
    /// Ajoute une commande dans la commande composée.
    /// </summary>
    /// <param name="command">Commande à ajouter.</param>
    public void Add(ISudokuCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        _commands.Add(command);
    }

    /// <summary>
    /// Indique si la commande composée ne contient aucune commande.
    /// </summary>
    public bool IsEmpty => _commands.Count == 0;

    /// <summary>
    /// Retourne le nombre de commandes contenues dans cette commande composée.
    /// </summary>
    public int Count => _commands.Count;

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
    /// 
    /// L'ordre inverse est important :
    /// si plusieurs commandes sont liées entre elles, on annule d'abord
    /// la dernière action exécutée.
    /// </summary>
    public void Undo()
    {
        for (int i = _commands.Count - 1; i >= 0; i--)
        {
            _commands[i].Undo();
        }
    }
}