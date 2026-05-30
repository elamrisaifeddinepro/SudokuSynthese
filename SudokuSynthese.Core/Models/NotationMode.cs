namespace SudokuSynthese.Core.Models;

/// <summary>
/// Représente le mode de saisie actuellement utilisé par le joueur.
/// 
/// Le mode choisi détermine comment une action sur une cellule sera interprétée :
/// valeur finale, note en coin, note au centre ou couleur.
/// </summary>
public enum NotationMode
{
    /// <summary>
    /// Mode de saisie d'une valeur définitive dans la cellule.
    /// Exemple : placer le chiffre 5 comme réponse finale.
    /// </summary>
    FinalValue,

    /// <summary>
    /// Mode de saisie des petites notes placées dans les coins de la cellule.
    /// Exemple : candidats possibles 1, 3, 7.
    /// </summary>
    CornerNote,

    /// <summary>
    /// Mode de saisie des notes placées au centre de la cellule.
    /// Exemple : hypothèses principales du joueur.
    /// </summary>
    CenterNote,

    /// <summary>
    /// Mode de coloration de la cellule.
    /// Exemple : marquer une cellule en bleu, vert, rouge, etc.
    /// </summary>
    Color
}