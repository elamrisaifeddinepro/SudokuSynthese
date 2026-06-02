using System.Text.Json;
using SudokuSynthese.Core.Models;

namespace SudokuSynthese.Core.Services;

/// <summary>
/// Service responsable de la sauvegarde et du chargement d'une grille de Sudoku.
/// 
/// Il permet de convertir une grille en JSON, de reconstruire une grille depuis du JSON,
/// et aussi de sauvegarder / charger depuis un fichier.
/// </summary>
public class SaveLoadService
{
    /// <summary>
    /// Options utilisées pour rendre le JSON lisible.
    /// </summary>
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };

    /// <summary>
    /// Convertit une grille Sudoku en texte JSON.
    /// 
    /// Cette méthode est pratique pour les tests unitaires,
    /// car elle ne dépend pas du disque.
    /// </summary>
    /// <param name="grid">Grille à sérialiser.</param>
    /// <returns>Chaîne JSON représentant la grille.</returns>
    public string Serialize(SudokuGrid grid)
    {
        ArgumentNullException.ThrowIfNull(grid);

        SudokuGridDto dto = SudokuGridDto.FromGrid(grid);

        return JsonSerializer.Serialize(dto, _jsonOptions);
    }

    /// <summary>
    /// Reconstruit une grille Sudoku à partir d'un texte JSON.
    /// 
    /// Cette méthode est pratique pour les tests unitaires,
    /// car elle ne dépend pas du disque.
    /// </summary>
    /// <param name="json">Texte JSON représentant une grille.</param>
    /// <returns>Grille Sudoku reconstruite.</returns>
    public SudokuGrid Deserialize(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException("Le JSON ne peut pas être vide.", nameof(json));
        }

        SudokuGridDto? dto = JsonSerializer.Deserialize<SudokuGridDto>(json, _jsonOptions);

        if (dto is null)
        {
            throw new InvalidOperationException("Impossible de désérialiser la grille.");
        }

        return dto.ToGrid();
    }

    /// <summary>
    /// Sauvegarde une grille Sudoku dans un fichier JSON.
    /// 
    /// Cette méthode sera surtout utilisée par l'application WPF.
    /// </summary>
    /// <param name="grid">Grille à sauvegarder.</param>
    /// <param name="path">Chemin du fichier de sauvegarde.</param>
    public void SaveToFile(SudokuGrid grid, string path)
    {
        ArgumentNullException.ThrowIfNull(grid);

        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Le chemin du fichier ne peut pas être vide.", nameof(path));
        }

        string json = Serialize(grid);

        File.WriteAllText(path, json);
    }

    /// <summary>
    /// Charge une grille Sudoku depuis un fichier JSON.
    /// 
    /// Cette méthode sera surtout utilisée par l'application WPF.
    /// </summary>
    /// <param name="path">Chemin du fichier à charger.</param>
    /// <returns>Grille Sudoku chargée.</returns>
    public SudokuGrid LoadFromFile(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Le chemin du fichier ne peut pas être vide.", nameof(path));
        }

        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Le fichier de sauvegarde est introuvable.", path);
        }

        string json = File.ReadAllText(path);

        return Deserialize(json);
    }

    /// <summary>
    /// DTO représentant une grille complète pour la sauvegarde JSON.
    /// 
    /// DTO signifie Data Transfer Object.
    /// Il sert ici à avoir une structure simple et propre à sérialiser.
    /// </summary>
    private class SudokuGridDto
    {
        public List<SudokuCellDto> Cells { get; set; } = new();

        public static SudokuGridDto FromGrid(SudokuGrid grid)
        {
            SudokuGridDto dto = new SudokuGridDto();

            foreach (SudokuCell cell in grid.GetAllCells())
            {
                dto.Cells.Add(SudokuCellDto.FromCell(cell));
            }

            return dto;
        }

        public SudokuGrid ToGrid()
        {
            SudokuGrid grid = new SudokuGrid();
            grid.Reset();

            foreach (SudokuCellDto cellDto in Cells)
            {
                SudokuCell cell = grid.GetCell(cellDto.Row, cellDto.Column);

                cell.Value = cellDto.Value;
                cell.IsGiven = cellDto.IsGiven;

                cell.CornerNotes.Clear();
                foreach (int note in cellDto.CornerNotes)
                {
                    cell.CornerNotes.Add(note);
                }

                cell.CenterNotes.Clear();
                foreach (int note in cellDto.CenterNotes)
                {
                    cell.CenterNotes.Add(note);
                }

                cell.Color = cellDto.Color;
                cell.IsSelected = cellDto.IsSelected;
                cell.HasError = cellDto.HasError;
            }

            return grid;
        }
    }

    /// <summary>
    /// DTO représentant une cellule pour la sauvegarde JSON.
    /// </summary>
    private class SudokuCellDto
    {
        public int Row { get; set; }

        public int Column { get; set; }

        public int? Value { get; set; }

        public bool IsGiven { get; set; }

        public List<int> CornerNotes { get; set; } = new();

        public List<int> CenterNotes { get; set; } = new();

        public CellColor Color { get; set; }

        public bool IsSelected { get; set; }

        public bool HasError { get; set; }

        public static SudokuCellDto FromCell(SudokuCell cell)
        {
            return new SudokuCellDto
            {
                Row = cell.Row,
                Column = cell.Column,
                Value = cell.Value,
                IsGiven = cell.IsGiven,
                CornerNotes = cell.CornerNotes.OrderBy(note => note).ToList(),
                CenterNotes = cell.CenterNotes.OrderBy(note => note).ToList(),
                Color = cell.Color,
                IsSelected = cell.IsSelected,
                HasError = cell.HasError
            };
        }
    }
}