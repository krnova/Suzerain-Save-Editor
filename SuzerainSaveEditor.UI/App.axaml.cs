using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using SuzerainSaveEditor.UI.ViewModels;
using SuzerainSaveEditor.Core.Parsing;
using SuzerainSaveEditor.Core.Schema;
using SuzerainSaveEditor.Core.Services;

namespace SuzerainSaveEditor.UI;

public partial class App : Application
{
    /// <summary>Must be assigned by the platform head before Avalonia startup completes.</summary>
    public static IPlatformHost? Host { get; set; }

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (Host is null)
            throw new InvalidOperationException(
                $"{nameof(App)}.{nameof(Host)} must be set before startup — the platform head didn't wire itself up.");

        DisableAvaloniaDataAnnotationValidation();

        var parser = new JsonSaveParser();
        var backupService = new BackupService();
        var saveFileService = new SaveFileService(parser, backupService);
        var schemaService = new SchemaService();
        var fieldResolver = new FieldResolver();
        var discoveryService = new FieldDiscoveryService(schemaService);
        var undoRedoService = new UndoRedoService();
        var appDataPathProvider = new AppDataPathProvider();
        var recentFilesService = new RecentFilesService(appDataPathProvider);
        var savePathProvider = new SavePathProvider();

        var composition = Host.CreateHost(savePathProvider);

        var viewModel = new MainWindowViewModel(
            saveFileService,
            schemaService,
            fieldResolver,
            composition.FileDialogService,
            discoveryService,
            undoRedoService,
            recentFilesService);

        viewModel.ShowChangeSummaryDialog = items => composition.DialogService.ShowChangeSummaryAsync(items);

        composition.RootView.DataContext = viewModel;
        _ = viewModel.LoadRecentFilesAsync();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = (Window)composition.RootView;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime mobile)
        {
            mobile.MainView = composition.RootView;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        foreach (var plugin in dataValidationPluginsToRemove)
            BindingPlugins.DataValidators.Remove(plugin);
    }
}
