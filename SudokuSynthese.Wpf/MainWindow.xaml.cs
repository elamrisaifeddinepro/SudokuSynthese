using System.Windows;
using System.Windows.Input;
using SudokuSynthese.Wpf.ViewModels;

namespace SudokuSynthese.Wpf;

/// <summary>
/// Fenêtre principale de l'application SudokuSynthese.
/// 
/// Elle initialise le MainViewModel et gère aussi les entrées clavier.
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DataContext = new MainViewModel();

        Loaded += (_, _) =>
        {
            Focus();
        };
    }

    /// <summary>
    /// Gère les touches du clavier avant les contrôles internes.
    /// 
    /// Cela permet d'utiliser les chiffres, les raccourcis clavier
    /// et certaines actions rapides depuis la fenêtre principale.
    /// </summary>
    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (DataContext is not MainViewModel viewModel)
        {
            return;
        }

        if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
        {
            if (HandleControlShortcut(e.Key, viewModel))
            {
                e.Handled = true;
            }

            return;
        }

        if (e.Key == Key.Escape)
        {
            viewModel.ClearSelection();
            e.Handled = true;
            return;
        }

        int? number = ConvertKeyToNumber(e.Key);

        if (number is null)
        {
            return;
        }

        if (viewModel.InputNumberCommand.CanExecute(number.Value))
        {
            viewModel.InputNumberCommand.Execute(number.Value);
            e.Handled = true;
        }
    }

    /// <summary>
    /// Gère les raccourcis clavier utilisant la touche Ctrl.
    /// </summary>
    /// <param name="key">Touche pressée.</param>
    /// <param name="viewModel">ViewModel principal.</param>
    /// <returns>True si le raccourci a été traité.</returns>
    private static bool HandleControlShortcut(Key key, MainViewModel viewModel)
    {
        switch (key)
        {
            case Key.Z:
                if (viewModel.CanUndo)
                {
                    viewModel.Undo();
                }

                return true;

            case Key.Y:
                if (viewModel.CanRedo)
                {
                    viewModel.Redo();
                }

                return true;

            case Key.S:
                viewModel.Save();
                return true;

            case Key.N:
                viewModel.RequestNewGrid();
                return true;

            default:
                return false;
        }
    }

    /// <summary>
    /// Convertit une touche clavier en chiffre Sudoku.
    /// 
    /// Supporte :
    /// - les chiffres au-dessus des lettres : 1 à 9 ;
    /// - le pavé numérique : NumPad1 à NumPad9.
    /// </summary>
    private static int? ConvertKeyToNumber(Key key)
    {
        return key switch
        {
            Key.D1 or Key.NumPad1 => 1,
            Key.D2 or Key.NumPad2 => 2,
            Key.D3 or Key.NumPad3 => 3,
            Key.D4 or Key.NumPad4 => 4,
            Key.D5 or Key.NumPad5 => 5,
            Key.D6 or Key.NumPad6 => 6,
            Key.D7 or Key.NumPad7 => 7,
            Key.D8 or Key.NumPad8 => 8,
            Key.D9 or Key.NumPad9 => 9,
            _ => null
        };
    }
}