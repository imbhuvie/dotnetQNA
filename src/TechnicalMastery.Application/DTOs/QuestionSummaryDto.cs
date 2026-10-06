using TechnicalMastery.Domain.Enums;

namespace TechnicalMastery.Application.DTOs;

/// <summary>
/// One row of the question list: identity, placement, classification
/// and the current user's state — without the heavy answer body.
/// </summary>
public class QuestionSummaryDto
{
    public int Id { get; set; }

    public string QuestionText { get; set; } = string.Empty;

    public int TopicId { get; set; }

    public string TopicName { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public DifficultyLevel DifficultyLevel { get; set; }

    public QuestionType QuestionType { get; set; }

    public List<string> Tags { get; set; } = new List<string>();

    public bool IsBookmarked { get; set; }

    public StudyStatus Status { get; set; }
}
