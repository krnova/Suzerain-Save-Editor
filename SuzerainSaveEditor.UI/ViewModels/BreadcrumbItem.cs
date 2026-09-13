namespace SuzerainSaveEditor.UI.ViewModels;

// represents a single clickable segment in the breadcrumb trail
public sealed record BreadcrumbItem(string Label, CategoryNodeViewModel Node, bool IsLast);
