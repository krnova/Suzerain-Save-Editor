using Avalonia.Controls;
using SuzerainSaveEditor.Core.Services;
using SuzerainSaveEditor.UI.Services;

namespace SuzerainSaveEditor.UI;

public sealed record PlatformComposition(
    Control RootView,
    IDialogService DialogService,
    IFileDialogService FileDialogService);

public interface IPlatformHost
{
    PlatformComposition CreateHost(ISavePathProvider savePathProvider);
}
