using SuzerainSaveEditor.UI.ViewModels;

namespace SuzerainSaveEditor.UI;

public interface IDialogService
{
    /// <summary>Returns true if the user chose Save, false for Cancel.</summary>
    Task<bool> ShowChangeSummaryAsync(IReadOnlyList<ChangeSummaryItemViewModel> items);

    Task<UnsavedChangesResult> ShowUnsavedChangesAsync();
}
