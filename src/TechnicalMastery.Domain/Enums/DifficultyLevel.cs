namespace TechnicalMastery.Domain.Enums;

/// <summary>
/// Learning progression of a question, from fundamentals to system design.
/// Stored in the database as an integer.
/// </summary>
public enum DifficultyLevel
{
    Beginner = 1,
    Intermediate = 2,
    Advanced = 3,
    Production = 4,
    Architecture = 5,
    SystemDesign = 6
}
