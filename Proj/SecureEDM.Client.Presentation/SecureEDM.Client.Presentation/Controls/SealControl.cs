using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace SecureEDM.Client.Presentation.Controls;

/// <summary>Круглая печать проверки подписи с текстом по дуге.</summary>
public sealed class SealControl : Control
{
    public static readonly StyledProperty<string?> RingTextProperty =
        AvaloniaProperty.Register<SealControl, string?>(nameof(RingText));

    public static readonly StyledProperty<string?> CenterTextProperty =
        AvaloniaProperty.Register<SealControl, string?>(nameof(CenterText));

    public static readonly StyledProperty<double> CenterFontSizeProperty =
        AvaloniaProperty.Register<SealControl, double>(nameof(CenterFontSize), 17);

    public static readonly StyledProperty<bool> IsValidProperty =
        AvaloniaProperty.Register<SealControl, bool>(nameof(IsValid), true);

    public static readonly StyledProperty<IBrush?> ValidBrushProperty =
        AvaloniaProperty.Register<SealControl, IBrush?>(nameof(ValidBrush));

    public static readonly StyledProperty<IBrush?> InvalidBrushProperty =
        AvaloniaProperty.Register<SealControl, IBrush?>(nameof(InvalidBrush));

    static SealControl()
    {
        AffectsRender<SealControl>(
            RingTextProperty,
            CenterTextProperty,
            CenterFontSizeProperty,
            IsValidProperty,
            ValidBrushProperty,
            InvalidBrushProperty);
    }

    public string? RingText
    {
        get => GetValue(RingTextProperty);
        set => SetValue(RingTextProperty, value);
    }

    public string? CenterText
    {
        get => GetValue(CenterTextProperty);
        set => SetValue(CenterTextProperty, value);
    }

    public double CenterFontSize
    {
        get => GetValue(CenterFontSizeProperty);
        set => SetValue(CenterFontSizeProperty, value);
    }

    public bool IsValid
    {
        get => GetValue(IsValidProperty);
        set => SetValue(IsValidProperty, value);
    }

    public IBrush? ValidBrush
    {
        get => GetValue(ValidBrushProperty);
        set => SetValue(ValidBrushProperty, value);
    }

    public IBrush? InvalidBrush
    {
        get => GetValue(InvalidBrushProperty);
        set => SetValue(InvalidBrushProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var size = Math.Min(Bounds.Width, Bounds.Height);
        if (size <= 0) return;

        var brush = IsValid
            ? ValidBrush ?? Brushes.SeaGreen
            : InvalidBrush ?? Brushes.IndianRed;

        var scale = size / 160.0;
        var offsetX = (Bounds.Width - size) / 2;
        var offsetY = (Bounds.Height - size) / 2;

        using var transform = context.PushTransform(
            Matrix.CreateScale(scale, scale) * Matrix.CreateTranslation(offsetX, offsetY));

        var center = new Point(80, 80);
        context.DrawEllipse(null, new Pen(brush, 3), center, 75, 75);
        context.DrawEllipse(null, new Pen(brush, 0.8), center, 70, 70);
        context.DrawEllipse(
            null,
            new Pen(brush, 1.5, new DashStyle([1.5, 2.5], 0)),
            center,
            43,
            43);

        DrawRingText(context, brush);
        DrawCenterText(context, brush);
    }

    private void DrawRingText(DrawingContext context, IBrush brush)
    {
        var text = RingText;
        if (string.IsNullOrEmpty(text)) return;

        var typeface = new Typeface(FontFamily.Default, FontStyle.Normal, FontWeight.SemiBold);
        var span = Math.PI * 2 - 0.65;
        var step = span / Math.Max(text.Length, 1);
        var start = -Math.PI / 2 - span / 2;
        const double radius = 58;

        for (var index = 0; index < text.Length; index++)
        {
            var character = text[index];
            if (char.IsWhiteSpace(character)) continue;

            var angle = start + (index + 0.5) * step;
            var x = 80 + radius * Math.Cos(angle);
            var y = 80 + radius * Math.Sin(angle);

            var formatted = new FormattedText(
                character.ToString(),
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                typeface,
                11,
                brush);

            using var characterTransform = context.PushTransform(
                Matrix.CreateRotation(angle + Math.PI / 2) *
                Matrix.CreateTranslation(x, y));

            context.DrawText(
                formatted,
                new Point(-formatted.Width / 2, -formatted.Height / 2));
        }
    }

    private void DrawCenterText(DrawingContext context, IBrush brush)
    {
        var text = CenterText;
        if (string.IsNullOrEmpty(text)) return;

        var typeface = new Typeface(FontFamily.Default, FontStyle.Normal, FontWeight.Bold);
        var formatted = new FormattedText(
            text,
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            typeface,
            CenterFontSize,
            brush);

        context.DrawText(
            formatted,
            new Point(80 - formatted.Width / 2, 80 - formatted.Height / 2));
    }
}
