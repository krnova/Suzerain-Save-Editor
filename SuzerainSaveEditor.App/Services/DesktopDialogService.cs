using Avalonia.Controls;
using SuzerainSaveEditor.App.Views;
using SuzerainSaveEditor.UI;
using SuzerainSaveEditor.UI.ViewModels;

namespace SuzerainSaveEditor.App.Services;

public sealed class DesktopDialogService : IDialogService
{
    private readonly Window _owner;

    public DesktopDialogService(Window owner)
    {
        ArgumentNullException.ThrowIfNull(owner);
        _owner = owner;
    }

    public async Task<bool> ShowChangeSummaryAsync(IReadOnlyList<ChangeSummaryItemViewModel> items)
    {
        var dialog = new ChangeSummaryDialog();
        dialog.SetChanges(items);
        await dialog.ShowDialog(_owner);
        return dialog.Result == ChangeSummaryResult.Save;
    }

    public async Task<UnsavedChangesResult> ShowUnsavedChangesAsync()
    {
        var dialog = new UnsavedChangesDialog();
        await dialog.ShowDialog(_owner);
        return dialog.Result;
    }
}
