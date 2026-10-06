using TechnicalMastery.Domain.Enums;

namespace TechnicalMastery.Application.DTOs;

/// <summary>
/// Full reading payload for the question detail screen (§13):
/// every content section plus placement, tags and user state.
/// </summary>
public class QuestionDetailDto
{
    public int Id { get; set; }

    public string QuestionText { get; set; } = string.Empty;

    public int TopicId { get; set; }

    public string TopicName { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public DifficultyLevel DifficultyLevel { get; set; }

    public QuestionType QuestionType { get; set; }

    public string ShortAnswer { get; set; } = string.Empty;

    public string DetailedAnswer { get; set; } = string.Empty;

    public string CodeExample { get; set; } = string.Empty;

    public string InternalWorking { get; set; } = string.Empty;

    public string RealWorldUsage { get; set; } = string.Empty;

    public string CommonMistake { get; set; } = string.Empty;

    public string TechnicalConversation { get; set; } = string.Empty;

    public string InterviewFollowUp { get; set; } = string.Empty;

    public string KeyTakeaway { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = new List<string>();

    public bool IsBookmarked { get; set; }

    public StudyStatus Status { get; set; }
}
