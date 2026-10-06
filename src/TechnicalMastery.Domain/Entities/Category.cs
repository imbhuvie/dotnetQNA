using TechnicalMastery.Domain.Common;

namespace TechnicalMastery.Domain.Entities;

/// <summary>
/// A top-level grouping such as "C# Fundamentals", "ASP.NET Core" or "SQL".
/// </summary>
public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Topic> Topics { get; set; } = new List<Topic>();
}
