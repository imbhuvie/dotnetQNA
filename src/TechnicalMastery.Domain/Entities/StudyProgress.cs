using TechnicalMastery.Domain.Enums;

namespace TechnicalMastery.Domain.Entities;

/// <summary>
/// Tracks the user's study journey for one question.
/// There is exactly one row per question.
/// </summary>
public class StudyProgress
{
    public int Id { get; set; }

    public int QuestionId { get; set; }

    public StudyStatus Status { get; set; } = StudyStatus.NotStarted;

    public DateTime? LastViewedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public int ReviewCount { get; set; }

    public Question Question { get; set; } = null!;
}
