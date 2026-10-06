namespace TechnicalMastery.Domain.Common;

/// <summary>
/// Base class for entities that track creation and modification time.
/// Entities without audit timestamps (Topic, QuestionTag, Bookmark,
/// StudyProgress) define their own fields exactly as specified and do
/// not inherit from this class.
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
