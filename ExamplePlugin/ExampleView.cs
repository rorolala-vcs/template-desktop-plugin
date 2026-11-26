using System.Globalization;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using RorolalaDesktop.Contract;
using RorolalaDesktop.I18n;

namespace ExamplePlugin;

/// <summary>The example view: a message, and the moment the header command last read it.</summary>
/// <remarks>
/// A view is an Avalonia control of the plugin's own, which the host inserts into its own visual tree.
/// It must therefore be built from the host's copy of Avalonia, which the load context delegates to. What
/// is drawn follows the look: a colour is named as a resource so that it changes with the theme, and a
/// value is read from <see cref="IPluginHost.Palette"/>.
/// </remarks>
internal sealed class ExampleView : Border
{
    /// <summary>What the view says.</summary>
    private readonly TextBlock _message = new()
    {
        FontSize = 13,
        TextAlignment = TextAlignment.Center,
        TextWrapping = TextWrapping.Wrap,
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
    };

    /// <summary>Makes the view and reads the message once.</summary>
    public ExampleView()
    {
        _message[!TextBlock.ForegroundProperty] = new DynamicResourceExtension(PaletteKeys.Front);

        Child = _message;
        Padding = new Avalonia.Thickness(16);

        Refresh();
    }

    /// <summary>Says the message again, with the moment it was read.</summary>
    public void Refresh() =>
        _message.Text = RolaI18N.Get(
            "rorolala_example.view.message",
            DateTime.Now.ToString("T", CultureInfo.CurrentCulture)
        );
}
