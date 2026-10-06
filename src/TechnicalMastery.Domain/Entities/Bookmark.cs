namespace TechnicalMastery.Domain.Entities;

/// <summary>
/// Marks a question as bookmarked by the user.
/// Future multi-user support: add a nullable UserId column and extend the
/// uniqueness rule to (UserId, QuestionId). No other change is required.
/// </summary>
public class Bookmark
{
    public int Id { get; set; }

    public int QuestionId { get; set; }

    public DateTime CreatedAt { get; set; }

    public Question Question { get; set; } = null!;
}
