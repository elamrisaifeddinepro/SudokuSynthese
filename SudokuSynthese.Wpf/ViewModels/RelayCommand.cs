using System.Windows.Input;

namespace SudokuSynthese.Wpf.ViewModels;

/// <summary>
/// Commande générique utilisée dans le pattern MVVM.
/// 
/// Elle permet de connecter les boutons, menus ou raccourcis WPF
/// aux méthodes du ViewModel.
/// </summary>
public class RelayCommand : ICommand
{
    /// <summary>
    /// Action à exécuter lorsque la commande est appelée.
    /// </summary>
    private readonly Action<object?> _execute;

    /// <summary>
    /// Condition optionnelle qui indique si la commande peut être exécutée.
    /// </summary>
    private readonly Predicate<object?>? _canExecute;

    /// <summary>
    /// Crée une nouvelle commande.
    /// </summary>
    /// <param name="execute">Action à exécuter.</param>
    /// <param name="canExecute">Condition optionnelle d'exécution.</param>
    public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    /// <summary>
    /// Indique si la commande peut être exécutée.
    /// </summary>
    /// <param name="parameter">Paramètre envoyé par le XAML.</param>
    /// <returns>True si la commande peut être exécutée, sinon false.</returns>
    public bool CanExecute(object? parameter)
    {
        return _canExecute is null || _canExecute(parameter);
    }

    /// <summary>
    /// Exécute la commande.
    /// </summary>
    /// <param name="parameter">Paramètre envoyé par le XAML.</param>
    public void Execute(object? parameter)
    {
        _execute(parameter);
    }

    /// <summary>
    /// Événement utilisé par WPF pour savoir si l'état de la commande a changé.
    /// </summary>
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Force WPF à réévaluer CanExecute().
    /// 
    /// Exemple : activer ou désactiver un bouton selon l'état du ViewModel.
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}