using System.Windows;
using SudokuSynthese.Wpf.ViewModels;

namespace SudokuSynthese.Wpf;

/// <summary>
/// Fenêtre principale de l'application SudokuSynthese.
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DataContext = new MainViewModel();
    }
}