using Avalonia.Controls;
using Avalonia.Interactivity;

namespace SuzerainSaveEditor.UI.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        // Android forces edge-to-edge at the OS level when targeting API 35+, with no opt-out
        // available at API 36+. Avalonia's own InsetsManager doesn't know that unless told -
        // sync it here so its automatic SafeAreaPadding behavior (default on since 11.1) actually
        // engages and paints the app's background across the full window instead of leaving the
        // OS's raw window background exposed in the inset strip.
        var topLevel = TopLevel.GetTopLevel(this);
        var insetsManager = topLevel?.InsetsManager;
        if (insetsManager is not null)
        {
            insetsManager.DisplayEdgeToEdge = true;
        }
    }
}
