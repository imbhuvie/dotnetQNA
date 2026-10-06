namespace TechnicalMastery.Domain.Entities;

/// <summary>
/// A sub-grouping inside a category, such as "Middleware" inside "ASP.NET Core".
/// </summary>
public class Topic
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public Category Category { get; set; } = null!;

    public ICollection<Question> Questions { get; set; } = new List<Question>();
}
