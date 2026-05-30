using SudokuSynthese.Core.Models;

namespace SudokuSynthese.Core.Services;

/// <summary>
/// Service responsable de la gestion des cellules sélectionnées dans une grille de Sudoku.
/// 
/// Ce service ne dépend pas de WPF.
/// Il mémorise uniquement les positions des cellules sélectionnées.
/// </summary>
public class SelectionService
{
    /// <summary>
    /// Ensemble des positions actuellement sélectionnées.
    /// 
    /// HashSet permet d'éviter les doublons :
    /// une même cellule ne peut pas être sélectionnée deux fois.
    /// </summary>
    private readonly HashSet<CellPosition> _selectedCells = new();

    /// <summary>
    /// Sélectionne une seule cellule.
    /// 
    /// Toutes les anciennes sélections sont supprimées,
    /// puis la nouvelle cellule devient la seule cellule sélectionnée.
    /// </summary>
    /// <param name="position">Position de la cellule à sélectionner.</param>
    public void SelectSingle(CellPosition position)
    {
        ValidatePosition(position);

        _selectedCells.Clear();
        _selectedCells.Add(position);
    }

    /// <summary>
    /// Ajoute ou retire une cellule de la sélection.
    /// 
    /// Si la cellule est déjà sélectionnée, elle est retirée.
    /// Si elle n'est pas sélectionnée, elle est ajoutée.
    /// 
    /// Cette méthode sera utilisée pour le Ctrl + clic dans l'interface.
    /// </summary>
    /// <param name="position">Position de la cellule à ajouter ou retirer.</param>
    public void ToggleSelection(CellPosition position)
    {
        ValidatePosition(position);

        if (_selectedCells.Contains(position))
        {
            _selectedCells.Remove(position);
        }
        else
        {
            _selectedCells.Add(position);
        }
    }

    /// <summary>
    /// Vide complètement la sélection actuelle.
    /// </summary>
    public void ClearSelection()
    {
        _selectedCells.Clear();
    }

    /// <summary>
    /// Retourne toutes les cellules actuellement sélectionnées.
    /// 
    /// Une copie est retournée pour protéger la collection interne du service.
    /// </summary>
    /// <returns>Collection en lecture seule des positions sélectionnées.</returns>
    public IReadOnlyCollection<CellPosition> GetSelectedCells()
    {
        return _selectedCells.ToList().AsReadOnly();
    }

    /// <summary>
    /// Indique si une cellule donnée est actuellement sélectionnée.
    /// </summary>
    /// <param name="position">Position de la cellule à vérifier.</param>
    /// <returns>True si la cellule est sélectionnée, sinon False.</returns>
    public bool IsSelected(CellPosition position)
    {
        ValidatePosition(position);

        return _selectedCells.Contains(position);
    }

    /// <summary>
    /// Vérifie qu'une position est valide dans une grille de Sudoku.
    /// Les indices doivent être compris entre 0 et 8.
    /// </summary>
    /// <param name="position">Position à vérifier.</param>
    private static void ValidatePosition(CellPosition position)
    {
        ArgumentNullException.ThrowIfNull(position);

        if (position.Row < 0 || position.Row >= SudokuGrid.Size)
        {
            throw new ArgumentOutOfRangeException(
                nameof(position),
                "La ligne doit être comprise entre 0 et 8.");
        }

        if (position.Column < 0 || position.Column >= SudokuGrid.Size)
        {
            throw new ArgumentOutOfRangeException(
                nameof(position),
                "La colonne doit être comprise entre 0 et 8.");
        }
    }
}