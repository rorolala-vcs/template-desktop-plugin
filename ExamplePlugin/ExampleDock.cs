using Avalonia.Controls;
using RorolalaDesktop.Contract;

namespace ExamplePlugin;

/// <summary>The example dock: the view, and one command in its header.</summary>
/// <remarks>
/// The header is what a user reaches for what the dock is showing. The host draws a
/// <see cref="DockHeaderCommand"/> as a button; a <see cref="DockHeaderView"/> is a control of the
/// plugin's own, inserted as it is, for a strip a command cannot express.
/// </remarks>
internal sealed class ExampleDock : IDockView
{
    /// <summary>The view the dock shows.</summary>
    private readonly ExampleView _view = new();

    /// <summary>What the header carries.</summary>
    private readonly DockHeaderItem[] _header;

    /// <summary>Makes the dock.</summary>
    public ExampleDock() =>
        _header =
        [
            new DockHeaderCommand("rorolala_example.command.refresh", _view.Refresh),
        ];

    /// <inheritdoc />
    public Control View => _view;

    /// <inheritdoc />
    public IReadOnlyList<DockHeaderItem> Header => _header;
}
