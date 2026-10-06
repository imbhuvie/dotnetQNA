using TechnicalMastery.Application.DTOs;
using TechnicalMastery.Application.DTOs.Common;
using TechnicalMastery.Domain.Entities;
using TechnicalMastery.Domain.Enums;

namespace TechnicalMastery.Application.Mappings;

/// <summary>
/// Manual entity → DTO mapping. Deliberately handwritten instead of AutoMapper:
/// every mapping is one screen of explicit code, so a ~1-year developer can
/// trace exactly what the API returns (§37). No reflection, no profiles.
/// </summary>
public static class DtoMapper
{
    public static CategoryDto ToDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            DisplayOrder = category.DisplayOrder
        };
    }

    public static TopicDto ToDto(Topic topic)
    {
        return new TopicDto
        {
            Id = topic.Id,
            CategoryId = topic.CategoryId,
            Name = topic.Name,
            Description = topic.Description,
            DisplayOrder = topic.DisplayOrder
        };
    }

    public static QuestionSummaryDto ToSummaryDto(Question question, bool isBookmarked, StudyStatus status)
    {
        return new QuestionSummaryDto
        {
            Id = question.Id,
            QuestionText = question.QuestionText,
            TopicId = question.TopicId,
            TopicName = question.Topic?.Name ?? string.Empty,
            CategoryId = question.Topic?.CategoryId ?? 0,
            CategoryName = question.Topic?.Category?.Name ?? string.Empty,
            DifficultyLevel = question.DifficultyLevel,
            QuestionType = question.QuestionType,
            Tags = question.Tags.Select(tag => tag.Tag).ToList(),
            IsBookmarked = isBookmarked,
            Status = status
        };
    }

    public static QuestionDetailDto ToDetailDto(Question question, bool isBookmarked, StudyStatus status)
    {
        return new QuestionDetailDto
        {
            Id = question.Id,
            QuestionText = question.QuestionText,
            TopicId = question.TopicId,
            TopicName = question.Topic?.Name ?? string.Empty,
            CategoryId = question.Topic?.CategoryId ?? 0,
            CategoryName = question.Topic?.Category?.Name ?? string.Empty,
            DifficultyLevel = question.DifficultyLevel,
            QuestionType = question.QuestionType,
            ShortAnswer = question.ShortAnswer,
            DetailedAnswer = question.DetailedAnswer,
            CodeExample = question.CodeExample,
            InternalWorking = question.InternalWorking,
            RealWorldUsage = question.RealWorldUsage,
            CommonMistake = question.CommonMistake,
            TechnicalConversation = question.TechnicalConversation,
            InterviewFollowUp = question.InterviewFollowUp,
            KeyTakeaway = question.KeyTakeaway,
            Tags = question.Tags.Select(tag => tag.Tag).ToList(),
            IsBookmarked = isBookmarked,
            Status = status
        };
    }

    public static BookmarkDto ToDto(Bookmark bookmark)
    {
        return new BookmarkDto
        {
            Id = bookmark.Id,
            QuestionId = bookmark.QuestionId,
            QuestionText = bookmark.Question?.QuestionText ?? string.Empty,
            CreatedAt = bookmark.CreatedAt
        };
    }

    public static StudyProgressDto ToDto(StudyProgress progress)
    {
        return new StudyProgressDto
        {
            Id = progress.Id,
            QuestionId = progress.QuestionId,
            Status = progress.Status,
            LastViewedAt = progress.LastViewedAt,
            CompletedAt = progress.CompletedAt,
            ReviewCount = progress.ReviewCount
        };
    }

    public static QuestionNoteDto ToDto(QuestionNote note)
    {
        return new QuestionNoteDto
        {
            Id = note.Id,
            QuestionId = note.QuestionId,
            NoteText = note.NoteText,
            CreatedAt = note.CreatedAt,
            UpdatedAt = note.UpdatedAt
        };
    }

    public static PagedResult<T> ToPagedResult<T>(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
    {
        int safePage = Math.Max(page, 1);
        int safePageSize = Math.Clamp(pageSize, 1, 100);

        return new PagedResult<T>
        {
            Items = items,
            Page = safePage,
            PageSize = safePageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)safePageSize)
        };
    }
}
