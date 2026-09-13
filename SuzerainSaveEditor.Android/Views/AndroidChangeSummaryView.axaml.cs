using Avalonia.Controls;
using Avalonia.Interactivity;
using SuzerainSaveEditor.UI.ViewModels;

namespace SuzerainSaveEditor.Android.Views;

public partial class AndroidChangeSummaryView : UserControl
{
    /// <summary>Raised with true for Save, false for Cancel.</summary>
    public event Action<bool>? Completed;

    public AndroidChangeSummaryView()
    {
        InitializeComponent();
    }

    public void SetChanges(IReadOnlyList<ChangeSummaryItemViewModel> items)
    {
        var count = items.Count;
        SummaryTextBlock.Text = count == 1
            ? "1 unsaved change"
            : $"{count} unsaved changes";

        ChangesList.ItemsSource = items;
    }

    private void OnCancel(object? sender, RoutedEventArgs e) => Completed?.Invoke(false);

    private void OnSave(object? sender, RoutedEventArgs e) => Completed?.Invoke(true);
}
