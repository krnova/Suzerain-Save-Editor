using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using SuzerainSaveEditor.Core.Services;

namespace SuzerainSaveEditor.UI.Services;

public sealed class FileDialogService : IFileDialogService
{
    private readonly Visual _anchor;
    private readonly ISavePathProvider _savePathProvider;

    public FileDialogService(Visual anchor, ISavePathProvider savePathProvider)
    {
        ArgumentNullException.ThrowIfNull(anchor);
        ArgumentNullException.ThrowIfNull(savePathProvider);
        _anchor = anchor;
        _savePathProvider = savePathProvider;
    }

    public async Task<string?> OpenFileAsync()
    {
        var topLevel = TopLevel.GetTopLevel(_anchor)
            ?? throw new InvalidOperationException("No TopLevel available for the file picker yet.");

        IStorageFolder? suggestedFolder = null;

        // try each candidate save directory in priority order
        foreach (var candidatePath in _savePathProvider.GetSaveDirectories())
        {
            if (Directory.Exists(candidatePath))
            {
                try
                {
                    suggestedFolder = await topLevel.StorageProvider
                        .TryGetFolderFromPathAsync(candidatePath);
                    if (suggestedFolder is not null)
                        break;
                }
                catch
                {
                    // permission or platform issue — skip this candidate
                }
            }
        }

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Suzerain Save File",
            SuggestedStartLocation = suggestedFolder,
            FileTypeFilter =
            [
                new FilePickerFileType("JSON Files") { Patterns = ["*.json"] },
                new FilePickerFileType("All Files") { Patterns = ["*.*"] }
            ],
            AllowMultiple = false
        });

        if (files.Count == 0)
            return null;

        return files[0].TryGetLocalPath();
    }
}
