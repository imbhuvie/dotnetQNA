using CommunityToolkit.Mvvm.ComponentModel;
using TechnicalMastery.Wpf.Services;

namespace TechnicalMastery.Wpf.ViewModels;

/// <summary>
/// Navigation stub: the full reader arrives in Phase 21.
/// </summary>
public partial class QuestionDetailViewModel : ViewModelBase
{
    [ObservableProperty]
    private int questionId;
}
