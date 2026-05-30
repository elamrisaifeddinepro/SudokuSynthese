namespace SudokuSynthese.Core.Models;

/// <summary>
/// Représente les couleurs possibles qu'un joueur peut appliquer à une cellule.
/// 
/// Important : ce modèle ne contient aucune dépendance WPF.
/// On ne met donc pas de Brush, SolidColorBrush ou autre classe graphique ici.
/// La conversion vers une couleur visuelle sera faite dans le projet WPF.
/// </summary>
public enum CellColor
{
    /// <summary>
    /// Aucune couleur appliquée à la cellule.
    /// </summary>
    None,

    /// <summary>
    /// Couleur bleue.
    /// </summary>
    Blue,

    /// <summary>
    /// Couleur verte.
    /// </summary>
    Green,

    /// <summary>
    /// Couleur jaune.
    /// </summary>
    Yellow,

    /// <summary>
    /// Couleur rouge.
    /// </summary>
    Red,

    /// <summary>
    /// Couleur violette.
    /// </summary>
    Purple,

    /// <summary>
    /// Couleur orange.
    /// </summary>
    Orange,

    /// <summary>
    /// Couleur rose.
    /// </summary>
    Pink,

    /// <summary>
    /// Couleur grise.
    /// </summary>
    Gray,

    /// <summary>
    /// Couleur cyan.
    /// </summary>
    Cyan
}