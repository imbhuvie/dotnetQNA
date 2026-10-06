using TechnicalMastery.Domain.Enums;

namespace TechnicalMastery.Application.DTOs;

/// <summary>
/// Binds the GET /api/questions query string. Defaults match §8
/// (<c>page=1&amp;pageSize=20</c>). Ranges are enforced by validation (Phase 10);
/// the repository clamps defensively as a second line of defence.
/// </summary>
public class QuestionsQuery
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public int? CategoryId { get; set; }

    public int? TopicId { get; set; }

    public DifficultyLevel? Difficulty { get; set; }

    public QuestionType? Type { get; set; }

    public string? Search { get; set; }

    public string? SortBy { get; set; }

    public bool Descending { get; set; }
}
