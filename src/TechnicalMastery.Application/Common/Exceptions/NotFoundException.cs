namespace TechnicalMastery.Application.Common.Exceptions;

/// <summary>
/// A requested entity does not exist. The API layer translates this
/// into HTTP 404 (global exception handling arrives in Phase 11).
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string entityName, object key)
        : base(entityName + " with id " + key + " was not found.")
    {
    }
}
