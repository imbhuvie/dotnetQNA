using TechnicalMastery.Application.Common.Exceptions;
using TechnicalMastery.Application.Interfaces;
using TechnicalMastery.Domain.Entities;

namespace TechnicalMastery.Application.Services;

/// <summary>
/// Bookmark use cases.
/// </summary>
public class BookmarkService : IBookmarkService
{
    private readonly IBookmarkRepository bookmarks;
    private readonly IQuestionRepository questions;

    public BookmarkService(IBookmarkRepository bookmarks, IQuestionRepository questions)
    {
        this.bookmarks = bookmarks;
        this.questions = questions;
    }

    public Task<IReadOnlyList<Bookmark>> GetAllAsync(CancellationToken cancellationToken)
    {
        return this.bookmarks.GetAllAsync(cancellationToken);
    }

    public async Task<Bookmark> AddAsync(int questionId, CancellationToken cancellationToken)
    {
        bool questionExists = await this.questions.ExistsAsync(questionId, cancellationToken);

        if (!questionExists)
        {
            throw new NotFoundException("Question", questionId);
        }

        bool alreadyBookmarked = await this.bookmarks.ExistsAsync(questionId, cancellationToken);

        if (alreadyBookmarked)
        {
            throw new ConflictException("Question " + questionId + " is already bookmarked.");
        }

        Bookmark bookmark = new Bookmark
        {
            QuestionId = questionId,
            CreatedAt = DateTime.UtcNow
        };

        await this.bookmarks.AddAsync(bookmark, cancellationToken);

        return bookmark;
    }

    public async Task RemoveAsync(int questionId, CancellationToken cancellationToken)
    {
        bool removed = await this.bookmarks.RemoveAsync(questionId, cancellationToken);

        if (!removed)
        {
            throw new NotFoundException("Bookmark", questionId);
        }
    }
}
