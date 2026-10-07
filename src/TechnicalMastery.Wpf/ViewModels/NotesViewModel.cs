using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TechnicalMastery.Wpf.Models;
using TechnicalMastery.Wpf.Services;

namespace TechnicalMastery.Wpf.ViewModels;

/// <summary>
/// Notes overview screen (§17): every personal note in one call, newest first,
/// with inline edit, delete, and jump-to-question. New notes are created from
/// the question reader, where the question context already exists.
/// </summary>
public partial class NotesViewModel : ViewModelBase
{
    private readonly INotesApiClient notes;
    private readonly NavigationService navigation;

    [ObservableProperty]
    private string editText = string.Empty;

    [ObservableProperty]
    private QuestionNoteModel? editingNote;

    public ObservableCollection<QuestionNoteModel> Notes { get; } = new ObservableCollection<QuestionNoteModel>();

    public NotesViewModel(INotesApiClient notes, NavigationService navigation)
    {
        this.notes = notes;
        this.navigation = navigation;
    }

    public override async Task InitializeAsync()
    {
        await LoadAsync();
    }

    [RelayCommand]
    private void StartEdit(QuestionNoteModel? note)
    {
        if (note is null)
        {
            return;
        }

        EditingNote = note;
        EditText = note.NoteText;
    }

    [RelayCommand]
    private async Task SaveEditAsync()
    {
        if (EditingNote is null || string.IsNullOrWhiteSpace(EditText))
        {
            return;
        }

        try
        {
            await this.notes.UpdateAsync(EditingNote.Id, EditText.Trim(), CancellationToken.None);
            EditingNote = null;
            EditText = string.Empty;
            await LoadAsync();
        }
        catch (Exception ex)
        {
            ShowError(ApiException.UserMessage(ex));
        }
    }

    [RelayCommand]
    private void CancelEdit()
    {
        EditingNote = null;
        EditText = string.Empty;
    }

    [RelayCommand]
    private async Task DeleteNoteAsync(QuestionNoteModel? note)
    {
        if (note is null)
        {
            return;
        }

        try
        {
            await this.notes.DeleteAsync(note.Id, CancellationToken.None);
            await LoadAsync();
        }
        catch (Exception ex)
        {
            ShowError(ApiException.UserMessage(ex));
        }
    }

    [RelayCommand]
    private async Task OpenQuestionAsync(QuestionNoteModel? note)
    {
        if (note is null)
        {
            return;
        }

        await this.navigation.NavigateToAsync<QuestionDetailViewModel>(detail => detail.QuestionId = note.QuestionId);
    }

    private async Task LoadAsync()
    {
        IsBusy = true;
        ClearError();

        try
        {
            IReadOnlyList<QuestionNoteModel> result = await this.notes.GetAllNotesAsync(CancellationToken.None);
            Notes.Clear();

            foreach (QuestionNoteModel note in result)
            {
                Notes.Add(note);
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
}
