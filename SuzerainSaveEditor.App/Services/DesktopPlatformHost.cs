using SuzerainSaveEditor.App.Views;
using SuzerainSaveEditor.Core.Services;
using SuzerainSaveEditor.UI;
using SuzerainSaveEditor.UI.Services;

namespace SuzerainSaveEditor.App.Services;

public sealed class DesktopPlatformHost : IPlatformHost
{
    public PlatformComposition CreateHost(ISavePathProvider savePathProvider)
    {
        var mainWindow = new MainWindow();
        var dialogService = new DesktopDialogService(mainWindow);
        mainWindow.DialogService = dialogService;

        var fileDialogService = new FileDialogService(mainWindow, savePathProvider);

        return new PlatformComposition(mainWindow, dialogService, fileDialogService);
    }
}
