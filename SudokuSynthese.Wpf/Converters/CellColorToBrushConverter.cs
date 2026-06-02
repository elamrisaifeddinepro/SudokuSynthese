using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using SudokuSynthese.Core.Models;

namespace SudokuSynthese.Wpf.Converters;

/// <summary>
/// Convertit une couleur métier CellColor en Brush WPF.
/// </summary>
public class CellColorToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not CellColor color)
        {
            return Brushes.White;
        }

        return color switch
        {
            CellColor.None => Brushes.White,
            CellColor.Blue => Brushes.LightBlue,
            CellColor.Green => Brushes.LightGreen,
            CellColor.Yellow => Brushes.LightYellow,
            CellColor.Red => Brushes.LightCoral,
            CellColor.Purple => Brushes.Plum,
            CellColor.Orange => Brushes.Orange,
            CellColor.Pink => Brushes.LightPink,
            CellColor.Gray => Brushes.LightGray,
            CellColor.Cyan => Brushes.LightCyan,
            _ => Brushes.White
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}