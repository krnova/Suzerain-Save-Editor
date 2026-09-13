using System.IO;
using System.Security;
using SuzerainSaveEditor.UI.ViewModels;

namespace SuzerainSaveEditor.UI;

public static class UnsavedChangesFlow
{
    /// <returns>
    /// true if it's safe to proceed (nothing dirty, discarded, or saved successfully);
    /// false to abort (save failed or the user cancelled).
    /// </returns>
    public static async Task<bool> ResolveAsync(MainWindowViewModel vm, IDialogService dialogService)
    {
        if (!vm.IsDirty || vm.SaveCommittedToDisk)
            return true;

        var result = await dialogService.ShowUnsavedChangesAsync();

        switch (result)
        {
            case UnsavedChangesResult.Save:
                try
                {
                    await vm.SaveCommand.ExecuteAsync(null);
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or SecurityException)
                {
                    return false; // save failed — caller should not proceed
                }
                return !vm.IsDirty || vm.SaveCommittedToDisk;

            case UnsavedChangesResult.Discard:
                return true;

            case UnsavedChangesResult.Cancel:
            default:
                return false;
        }
    }
}
