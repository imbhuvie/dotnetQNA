namespace TechnicalMastery.Application.Common.Exceptions;

/// <summary>
/// The operation conflicts with current state (for example, bookmarking an
/// already-bookmarked question). The API layer translates this into HTTP 409.
/// </summary>
public class ConflictException : Exception
{
    public ConflictException(string message)
        : base(message)
    {
    }
}
