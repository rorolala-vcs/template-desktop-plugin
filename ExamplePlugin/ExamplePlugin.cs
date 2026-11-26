using RorolalaDesktop.Contract;

namespace ExamplePlugin;

/// <summary>
/// The example plugin: one dock, and the context-menu item that opens it.
/// </summary>
/// <remarks>
/// A plugin is the one type in its assembly implementing <see cref="IRolaPlugin"/>. The host makes it,
/// reads <see cref="Manifest"/>, and calls <see cref="Initialize"/> once the plugins it depends on are
/// loaded. Everything the plugin contributes is registered in that call: a registration made after the
/// window is shown is not drawn.
/// <para>
/// The identity and the dock name id are written into the user's configuration and dock layout, so they
/// must not change once the plugin has been shipped.
/// </para>
/// </remarks>
public sealed class ExamplePlugin : IRolaPlugin
{
    /// <summary>This plugin's identity, which another plugin names in its manifest to depend on it.</summary>
    public const string Identity = "rorolala.example";

    /// <summary>The stable name of the example dock, by convention the identity and the dock's own name.</summary>
    public const string DockNameId = "rorolala.example.dock";

    /// <inheritdoc />
    public PluginManifest Manifest { get; } =
        new(
            new PluginId(Identity),
            "rorolala_example.name",
            typeof(IRolaPlugin).Assembly.GetName().Version ?? new Version(0, 0),
            []
        );

    /// <inheritdoc />
    public void Initialize(IPluginHost host)
    {
        ArgumentNullException.ThrowIfNull(host);

        host.I18n.RegisterDirectory(Translations());
        host.Log.Info("the example plugin started");

        // A toggle: activating it shows or hides the one instance, which is what a view of one thing
        // wants. A factory is `DockOpenMode.New`, for a dock a user may want several of.
        host.Docks.Register(
            new DockRegistration(
                Manifest.Id,
                DockNameId,
                "rorolala_example.dock",
                DockOpenMode.Toggle,
                DockSide.Right,
                () => new ExampleDock()
            )
        );

        // The context menu is how the dock is reached without the Window menu. `Applies` keeps the item
        // out of menus it has nothing to say about.
        host.ContextMenus.Add(
            ContextMenuTarget.File,
            new ContextMenuItem(
                "rorolala_example.menu.open",
                50,
                _ => host.Windows.Show(DockNameId, null),
                Applies: target => target.Entries.Count > 0
            )
        );
    }

    /// <summary>The directory this plugin's translations sit in, beside its assembly.</summary>
    /// <returns>The directory.</returns>
    private static string Translations() =>
        Path.Combine(
            Path.GetDirectoryName(typeof(ExamplePlugin).Assembly.Location)!,
            "i18n",
            "rorolala_example"
        );
}
