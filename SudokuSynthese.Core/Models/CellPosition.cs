namespace SudokuSynthese.Core.Models;

/// <summary>
/// Représente la position d'une cellule dans la grille de Sudoku.
/// Une position est définie par une ligne et une colonne.
/// </summary>
/// <param name="Row">Indice de la ligne, entre 0 et 8.</param>
/// <param name="Column">Indice de la colonne, entre 0 et 8.</param>
public record CellPosition(int Row, int Column);