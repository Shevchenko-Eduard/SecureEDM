using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace SecureEDM.Client.Presentation.Controls;

/// <summary>Визуализация хеша: 16 колонок цветных ячеек, по одной на каждый hex-символ.</summary>
public sealed class HashCellsControl : Control
{
    public static readonly StyledProperty<string?> HashProperty =
        AvaloniaProperty.Register<HashCellsControl, string?>(nameof(Hash));

    static HashCellsControl()
    {
        AffectsMeasure<HashCellsControl>(HashProperty);
        AffectsRender<HashCellsControl>(HashProperty);
    }

    public string? Hash
    {
        get => GetValue(HashProperty);
        set => SetValue(HashProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var hash = NormalizedHash;
        if (hash.Length == 0) return new Size(0, 0);

        var width = double.IsFinite(availableSize.Width) && availableSize.Width > 0
            ? availableSize.Width
            : 480;
        var cell = (width - 15 * 3) / 16;
        if (cell <= 0) return new Size(width, 0);

        var rows = (int)Math.Ceiling(hash.Length / 16.0);
        var height = rows * cell + (rows - 1) * 3;
        return new Size(width, height);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var hash = NormalizedHash;
        if (hash.Length == 0 || Bounds.Width <= 0) return;

        const double gap = 3;
        var cell = (Bounds.Width - 15 * gap) / 16;
        if (cell <= 0) return;

        for (var index = 0; index < hash.Length; index++)
        {
            var row = index / 16;
            var column = index % 16;
            var x = column * (cell + gap);
            var y = row * (cell + gap);
            var value = (int)hash[index];
            var brush = new SolidColorBrush(HslToRgb(
                165 + value % 55,
                0.30,
                (60 + value / 255.0 * 22) / 100.0));

            context.DrawRectangle(
                brush,
                null,
                new Rect(x, y, cell, cell),
                3,
                3);
        }
    }

    private string NormalizedHash
    {
        get
        {
            var value = Hash;
            return string.IsNullOrWhiteSpace(value) || value == "—" ? string.Empty : value;
        }
    }

    private static Color HslToRgb(double hue, double saturation, double lightness)
    {
        hue = (hue % 360 + 360) % 360 / 360.0;
        var q = lightness < 0.5
            ? lightness * (1 + saturation)
            : lightness + saturation - lightness * saturation;
        var p = 2 * lightness - q;

        static double Channel(double t, double pValue, double qValue)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1.0 / 6) return pValue + (qValue - pValue) * 6 * t;
            if (t < 0.5) return qValue;
            if (t < 2.0 / 3) return pValue + (qValue - pValue) * (2.0 / 3 - t) * 6;
            return pValue;
        }

        return Color.FromRgb(
            (byte)Math.Round(Channel(hue + 1.0 / 3, p, q) * 255),
            (byte)Math.Round(Channel(hue, p, q) * 255),
            (byte)Math.Round(Channel(hue - 1.0 / 3, p, q) * 255));
    }
}
