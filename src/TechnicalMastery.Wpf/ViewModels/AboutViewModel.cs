using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TechnicalMastery.Wpf.ViewModels;

/// <summary>About screen: product info and architecture note.</summary>
public partial class AboutViewModel : ViewModelBase
{
    public string AppTitle
    {
        get { return ".NET Technical Mastery"; }
    }

    public string AppVersion
    {
        get { return Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0"; }
    }

    public string Description
    {
        get
        {
            return "A .NET Technical Q&A learning platform: browse, search, bookmark and " +
                "study 1000+ questions through a clean MVVM desktop client backed by a " +
                "platform-independent ASP.NET Core API — a future mobile app consumes " +
                "the same endpoints with no backend changes.";
        }
    }
}
