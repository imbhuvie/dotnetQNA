using CommunityToolkit.Mvvm.ComponentModel;

namespace TechnicalMastery.Wpf.ViewModels;

/// <summary>Placeholder: categories/topics/questions browsing arrives in Phase 20.</summary>
public partial class BrowseViewModel : ViewModelBase
{
    [ObservableProperty]
    private string searchText = string.Empty;
}
