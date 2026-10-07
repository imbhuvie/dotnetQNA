using TechnicalMastery.Wpf.Models;

namespace TechnicalMastery.Wpf.Services;

/// <summary>
/// Narrow client contracts — one per server area. View models depend on these,
/// never on <see cref="StudyApiClient"/> directly, so an offline cache can later
/// implement the same interfaces without touching any UI (§21: IDataSource seam).
/// </summary>
public interface ICatalogApiClient
{
    Task<IReadOnlyList<CategoryModel>> GetCategoriesAsync(CancellationToken ct);

    Task<IReadOnlyList<TopicModel>> GetTopicsAsync(int? categoryId, CancellationToken ct);
}

public interface IQuestionApiClient
{
    Task<PagedResult<QuestionSummaryModel>> GetPagedAsync(
        int page, int pageSize, int? categoryId, int? topicId,
        DifficultyLevel? difficulty, string? search, string? sortBy, bool descending,
        CancellationToken ct);

    Task<QuestionDetailModel> GetByIdAsync(int id, CancellationToken ct);

    Task<IReadOnlyList<QuestionSummaryModel>> GetRandomAsync(int count, CancellationToken ct);

    Task<IReadOnlyList<QuestionSummaryModel>> GetRelatedAsync(int questionId, int count, CancellationToken ct);
}

public interface IBookmarkApiClient
{
    Task<IReadOnlyList<BookmarkModel>> GetAllAsync(CancellationToken ct);

    Task AddAsync(int questionId, CancellationToken ct);

    Task RemoveAsync(int questionId, CancellationToken ct);
}

public interface IProgressApiClient
{
    Task<IReadOnlyList<StudyProgressModel>> GetAllProgressAsync(CancellationToken ct);

    Task SetStatusAsync(int questionId, StudyStatus status, CancellationToken ct);
}

public interface INotesApiClient
{
    Task<IReadOnlyList<QuestionNoteModel>> GetAllNotesAsync(CancellationToken ct);

    Task<IReadOnlyList<QuestionNoteModel>> GetByQuestionAsync(int questionId, CancellationToken ct);

    Task CreateAsync(int questionId, string noteText, CancellationToken ct);

    Task UpdateAsync(int noteId, string noteText, CancellationToken ct);

    Task DeleteAsync(int noteId, CancellationToken ct);
}

public interface IDashboardApiClient
{
    Task<DashboardSummaryModel> GetSummaryAsync(CancellationToken ct);
}
