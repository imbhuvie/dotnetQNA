using TechnicalMastery.Domain.Common;

namespace TechnicalMastery.Domain.Entities;

/// <summary>
/// A personal note the user attaches to a question.
/// </summary>
public class QuestionNote : BaseEntity
{
    public int QuestionId { get; set; }

    public string NoteText { get; set; } = string.Empty;

    public Question Question { get; set; } = null!;
}
