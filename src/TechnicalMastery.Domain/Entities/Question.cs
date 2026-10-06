using TechnicalMastery.Domain.Common;
using TechnicalMastery.Domain.Enums;

namespace TechnicalMastery.Domain.Entities;

/// <summary>
/// A single technical question with its full learning content:
/// short answer, detailed explanation, code, internals, real-world usage,
/// common mistakes, discussion points and key takeaway.
/// </summary>
public class Question : BaseEntity
{
    public int TopicId { get; set; }

    public string QuestionText { get; set; } = string.Empty;

    public string ShortAnswer { get; set; } = string.Empty;

    public string DetailedAnswer { get; set; } = string.Empty;

    public DifficultyLevel DifficultyLevel { get; set; }

    public QuestionType QuestionType { get; set; }

    public string CodeExample { get; set; } = string.Empty;

    public string InternalWorking { get; set; } = string.Empty;

    public string RealWorldUsage { get; set; } = string.Empty;

    public string CommonMistake { get; set; } = string.Empty;

    public string TechnicalConversation { get; set; } = string.Empty;

    public string InterviewFollowUp { get; set; } = string.Empty;

    public string KeyTakeaway { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public Topic Topic { get; set; } = null!;

    public ICollection<QuestionTag> Tags { get; set; } = new List<QuestionTag>();

    public ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();

    public StudyProgress? Progress { get; set; }

    public ICollection<QuestionNote> Notes { get; set; } = new List<QuestionNote>();
}
