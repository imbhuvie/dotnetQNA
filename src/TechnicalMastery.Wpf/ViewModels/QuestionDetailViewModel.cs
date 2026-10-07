using System.Collections.ObjectModel;
using System.Windows.Input;
using TechnicalMastery.Wpf.Commands;
using TechnicalMastery.Wpf.Models;
using TechnicalMastery.Wpf.Services;

namespace TechnicalMastery.Wpf.ViewModels;

/// <summary>
/// Question reader (§13): full content sections, related questions, personal
/// notes, and study actions (bookmark, complete, needs-review, prev/next).
/// The id is set before <see cref="InitializeAsync"/> via navigation configure.
/// </summary>
public class QuestionDetailViewModel : ViewModelBase
{
    private readonly IQuestionApiClient questions;
    private readonly IBookmarkApiClient bookmarks;
    private readonly IProgressApiClient progress;
    private readonly INotesApiClient notes;
    private readonly NavigationService navigation;

    private int questionId;

    private QuestionDetailModel? detail;

    private string newNoteText = string.Empty;

    public ObservableCollection<QuestionSummaryModel> Related { get; } = new ObservableCollection<QuestionSummaryModel>();

    public ObservableCollection<QuestionNoteModel> Notes { get; } = new ObservableCollection<QuestionNoteModel>();

    public QuestionDetailViewModel(
        IQuestionApiClient questions,
        IBookmarkApiClient bookmarks,
        IProgressApiClient progress,
        INotesApiClient notes,
        NavigationService navigation)
    {
        this.questions = questions;
        this.bookmarks = bookmarks;
        this.progress = progress;
        this.notes = notes;
        this.navigation = navigation;

        PreviousCommand = new RelayCommand(PreviousAsync);
        NextCommand = new RelayCommand(NextAsync);
        ToggleBookmarkCommand = new RelayCommand(ToggleBookmarkAsync);
        MarkCompletedCommand = new RelayCommand(MarkCompletedAsync);
        MarkNeedsReviewCommand = new RelayCommand(MarkNeedsReviewAsync);
        OpenRelatedCommand = new RelayCommand<QuestionSummaryModel>(OpenRelatedAsync);
        AddNoteCommand = new RelayCommand(AddNoteAsync);
        DeleteNoteCommand = new RelayCommand<QuestionNoteModel>(DeleteNoteAsync);
        BackToBrowseCommand = new RelayCommand(BackToBrowseAsync);
    }

    public int QuestionId
    {
        get { return this.questionId; }
        set { SetProperty(ref this.questionId, value); }
    }

    public QuestionDetailModel? Detail
    {
        get { return this.detail; }
        set { SetProperty(ref this.detail, value); }
    }

    public string NewNoteText
    {
        get { return this.newNoteText; }
        set { SetProperty(ref this.newNoteText, value); }
    }

    public ICommand PreviousCommand { get; }

    public ICommand NextCommand { get; }

    public ICommand ToggleBookmarkCommand { get; }

    public ICommand MarkCompletedCommand { get; }

    public ICommand MarkNeedsReviewCommand { get; }

    public ICommand OpenRelatedCommand { get; }

    public ICommand AddNoteCommand { get; }

    public ICommand DeleteNoteCommand { get; }

    public ICommand BackToBrowseCommand { get; }

    public override async Task InitializeAsync()
    {
        await LoadAsync(QuestionId);
    }

    private async Task PreviousAsync()
    {
        if (QuestionId > 1)
        {
            await LoadAsync(QuestionId - 1);
        }
    }

    private async Task NextAsync()
    {
        await LoadAsync(QuestionId + 1);
    }

    private async Task ToggleBookmarkAsync()
    {
        if (Detail is null)
        {
            return;
        }

        try
        {
            if (Detail.IsBookmarked)
            {
                await this.bookmarks.RemoveAsync(Detail.Id, CancellationToken.None);
            }
            else
            {
                await this.bookmarks.AddAsync(Detail.Id, CancellationToken.None);
            }

            await LoadAsync(Detail.Id);
        }
        catch (Exception ex)
        {
            ShowError(ApiException.UserMessage(ex));
        }
    }

    private async Task MarkCompletedAsync()
    {
        await SetStatusAsync(StudyStatus.Completed);
    }

    private async Task MarkNeedsReviewAsync()
    {
        await SetStatusAsync(StudyStatus.NeedsReview);
    }

    private async Task OpenRelatedAsync(QuestionSummaryModel? related)
    {
        if (related is null)
        {
            return;
        }

        await LoadAsync(related.Id);
    }

    private async Task AddNoteAsync()
    {
        if (Detail is null || string.IsNullOrWhiteSpace(NewNoteText))
        {
            return;
        }

        try
        {
            await this.notes.CreateAsync(Detail.Id, NewNoteText.Trim(), CancellationToken.None);
            NewNoteText = string.Empty;
            await LoadNotesAsync(Detail.Id);
        }
        catch (Exception ex)
        {
            ShowError(ApiException.UserMessage(ex));
        }
    }

    private async Task DeleteNoteAsync(QuestionNoteModel? note)
    {
        if (note is null || Detail is null)
        {
            return;
        }

        try
        {
            await this.notes.DeleteAsync(note.Id, CancellationToken.None);
            await LoadNotesAsync(Detail.Id);
        }
        catch (Exception ex)
        {
            ShowError(ApiException.UserMessage(ex));
        }
    }

    private async Task BackToBrowseAsync()
    {
        await this.navigation.NavigateToAsync<BrowseViewModel>();
    }

    private async Task SetStatusAsync(StudyStatus status)
    {
        if (Detail is null)
        {
            return;
        }

        try
        {
            await this.progress.SetStatusAsync(Detail.Id, status, CancellationToken.None);
            await LoadAsync(Detail.Id);
        }
        catch (Exception ex)
        {
            ShowError(ApiException.UserMessage(ex));
        }
    }

    private async Task LoadAsync(int id)
    {
        IsBusy = true;
        ClearError();

        try
        {
            QuestionDetailModel detail = await this.questions.GetByIdAsync(id, CancellationToken.None);
            QuestionId = detail.Id;
            Detail = detail;

            IReadOnlyList<QuestionSummaryModel> related = await this.questions.GetRelatedAsync(id, 5, CancellationToken.None);
            Related.Clear();

            foreach (QuestionSummaryModel item in related)
            {
                Related.Add(item);
            }

            await LoadNotesAsync(id);
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

    private async Task LoadNotesAsync(int id)
    {
        IReadOnlyList<QuestionNoteModel> result = await this.notes.GetByQuestionAsync(id, CancellationToken.None);
        Notes.Clear();

        foreach (QuestionNoteModel note in result)
        {
            Notes.Add(note);
        }
    }
}
