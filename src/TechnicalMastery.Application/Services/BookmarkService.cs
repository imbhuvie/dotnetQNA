using TechnicalMastery.Application.Common.Exceptions;
using TechnicalMastery.Application.DTOs;
using TechnicalMastery.Application.Interfaces;
using TechnicalMastery.Application.Mappings;
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

    public async Task<IReadOnlyList<BookmarkDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Bookmark> result = await this.bookmarks.GetAllAsync(cancellationToken);

        return result.Select(DtoMapper.ToDto).ToList();
    }

    public async Task<BookmarkDto> AddAsync(int questionId, CancellationToken cancellationToken)
    {
        Question? question = await this.questions.GetByIdAsync(questionId, cancellationToken);

        if (question is null)
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

        // Build the DTO from the already-loaded question instead of assigning
        // the navigation: attaching a detached graph would conflict with any
        // instance the context already tracks (same key, different object).
        return new BookmarkDto
        {
            Id = bookmark.Id,
            QuestionId = questionId,
            QuestionText = question.QuestionText,
            CreatedAt = bookmark.CreatedAt
        };
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
