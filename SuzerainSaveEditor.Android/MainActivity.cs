using Android.App;
using Android.Content.PM;
using Avalonia;
using Avalonia.Android;
using SuzerainSaveEditor.Android.Services;
using SuzerainSaveEditor.Core.Services;
using SuzerainSaveEditor.UI;
using SuzerainSaveEditor.UI.Services;
using SuzerainSaveEditor.UI.ViewModels;
using SuzerainSaveEditor.UI.Views;

namespace SuzerainSaveEditor.Android;

[Activity(
    Label = "Suzerain Save Editor",
    Theme = "@style/MyTheme",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.KeyboardHidden)]
public class MainActivity : AvaloniaMainActivity<App>, IPlatformHost
{
    private MainView? _mainView;
    private AndroidDialogService? _dialogService;

    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        App.Host = this;
        BackRequested += OnBackRequested;
        return base.CustomizeAppBuilder(builder).WithInterFont();
    }

    public PlatformComposition CreateHost(ISavePathProvider savePathProvider)
    {
        _mainView = new MainView();
        _dialogService = new AndroidDialogService(_mainView);
        var fileDialogService = new FileDialogService(_mainView, savePathProvider);

        return new PlatformComposition(_mainView, _dialogService, fileDialogService);
    }

    // BackRequested is Avalonia's own wrapper over the modern OnBackPressedDispatcher/
    // OnBackInvoked APIs (Android 13+ predictive-back-safe) - this replaces overriding
    // the deprecated Activity.OnBackPressed() directly.
    private void OnBackRequested(object? sender, AndroidBackRequestedEventArgs e)
    {
        if (_mainView?.DataContext is MainWindowViewModel vm && _dialogService is not null)
        {
            e.Handled = true; // suppress the default back action until we've resolved
            _ = HandleBackPressAsync(vm);
        }
    }

    private async Task HandleBackPressAsync(MainWindowViewModel vm)
    {
        if (await UnsavedChangesFlow.ResolveAsync(vm, _dialogService!))
            Finish();
    }
}
