using System.Collections.ObjectModel;
using System.Windows.Input;
using TechnicalMastery.Wpf.Commands;
using TechnicalMastery.Wpf.Models;
using TechnicalMastery.Wpf.Services;

namespace TechnicalMastery.Wpf.ViewModels;

/// <summary>
/// Bookmarks screen (§15): list of bookmarked questions with navigation to
/// detail and remove action. Loads on navigation via InitializeAsync.
/// </summary>
public class BookmarksViewModel : ViewModelBase
{
    private readonly IBookmarkApiClient bookmarks;
    private readonly NavigationService navigation;

    public ObservableCollection<BookmarkModel> Bookmarks { get; } = new ObservableCollection<BookmarkModel>();

    public BookmarksViewModel(IBookmarkApiClient bookmarks, NavigationService navigation)
    {
        this.bookmarks = bookmarks;
        this.navigation = navigation;

        OpenBookmarkCommand = new RelayCommand<BookmarkModel>(OpenBookmarkAsync);
        RemoveBookmarkCommand = new RelayCommand<BookmarkModel>(RemoveBookmarkAsync);
    }

    public ICommand OpenBookmarkCommand { get; }

    public ICommand RemoveBookmarkCommand { get; }

    public override async Task InitializeAsync()
    {
        IsBusy = true;
        ClearError();

        try
        {
            IReadOnlyList<BookmarkModel> result = await this.bookmarks.GetAllAsync(CancellationToken.None);
            Bookmarks.Clear();

            foreach (BookmarkModel bookmark in result)
            {
                Bookmarks.Add(bookmark);
            }
        }
        catch (Exception ex)
        {
            ShowError(ApiException.UserMessage(ex));
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task OpenBookmarkAsync(BookmarkModel? bookmark)
    {
        if (bookmark is null)
        {
            return;
        }

        await this.navigation.NavigateToAsync<QuestionDetailViewModel>(detail => detail.QuestionId = bookmark.QuestionId);
    }

    private async Task RemoveBookmarkAsync(BookmarkModel? bookmark)
    {
        if (bookmark is null)
        {
            return;
        }

        try
        {
            await this.bookmarks.RemoveAsync(bookmark.QuestionId, CancellationToken.None);
            Bookmarks.Remove(bookmark);
        }
        catch (Exception ex)
        {
            ShowError(ApiException.UserMessage(ex));
        }
    }
}
