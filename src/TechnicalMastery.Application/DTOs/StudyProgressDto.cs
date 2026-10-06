using TechnicalMastery.Domain.Enums;

namespace TechnicalMastery.Application.DTOs;

public class StudyProgressDto
{
    public int Id { get; set; }

    public int QuestionId { get; set; }

    public StudyStatus Status { get; set; }

    public DateTime? LastViewedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public int ReviewCount { get; set; }
}
