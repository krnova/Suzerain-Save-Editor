using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using SuzerainSaveEditor.Android.Views;
using SuzerainSaveEditor.UI;
using SuzerainSaveEditor.UI.ViewModels;

namespace SuzerainSaveEditor.Android.Services;

public sealed class AndroidDialogService : IDialogService
{
    private readonly Visual _anchor;

    public AndroidDialogService(Visual anchor)
    {
        ArgumentNullException.ThrowIfNull(anchor);
        _anchor = anchor;
    }

    public Task<bool> ShowChangeSummaryAsync(IReadOnlyList<ChangeSummaryItemViewModel> items)
    {
        var tcs = new TaskCompletionSource<bool>();
        var view = new AndroidChangeSummaryView();
        view.SetChanges(items);
        view.Completed += result =>
        {
            Dismiss(view);
            tcs.TrySetResult(result);
        };
        Show(view);
        return tcs.Task;
    }

    public Task<UnsavedChangesResult> ShowUnsavedChangesAsync()
    {
        var tcs = new TaskCompletionSource<UnsavedChangesResult>();
        var view = new AndroidUnsavedChangesView();
        view.Completed += result =>
        {
            Dismiss(view);
            tcs.TrySetResult(result);
        };
        Show(view);
        return tcs.Task;
    }

    private void Show(Control content)
    {
        var overlay = OverlayLayer.GetOverlayLayer(_anchor)
            ?? throw new InvalidOperationException("No OverlayLayer found — is the view attached?");
        overlay.Children.Add(content);
    }

    private void Dismiss(Control content)
    {
        var overlay = OverlayLayer.GetOverlayLayer(_anchor);
        overlay?.Children.Remove(content);
    }
}
