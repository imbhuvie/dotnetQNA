namespace TechnicalMastery.Domain.Enums;

/// <summary>
/// The format of a question. Used for filtering (for example,
/// show only scenario-based production questions).
/// Stored in the database as an integer.
/// </summary>
public enum QuestionType
{
    Conceptual = 1,
    CodeBased = 2,
    ScenarioBased = 3,
    Debugging = 4,
    Design = 5
}
