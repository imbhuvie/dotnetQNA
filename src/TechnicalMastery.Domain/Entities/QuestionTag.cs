namespace TechnicalMastery.Domain.Entities;

/// <summary>
/// A free-text label attached to a question (for example "di", "jwt", "n-plus-1").
/// </summary>
public class QuestionTag
{
    public int Id { get; set; }

    public int QuestionId { get; set; }

    public string Tag { get; set; } = string.Empty;

    public Question Question { get; set; } = null!;
}
