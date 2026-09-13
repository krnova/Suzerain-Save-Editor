using Avalonia.Controls;
using Avalonia.Interactivity;
using SuzerainSaveEditor.UI;

namespace SuzerainSaveEditor.Android.Views;

public partial class AndroidUnsavedChangesView : UserControl
{
    public event Action<UnsavedChangesResult>? Completed;

    public AndroidUnsavedChangesView()
    {
        InitializeComponent();
    }

    private void OnCancel(object? sender, RoutedEventArgs e) => Completed?.Invoke(UnsavedChangesResult.Cancel);

    private void OnDiscard(object? sender, RoutedEventArgs e) => Completed?.Invoke(UnsavedChangesResult.Discard);

    private void OnSave(object? sender, RoutedEventArgs e) => Completed?.Invoke(UnsavedChangesResult.Save);
}
