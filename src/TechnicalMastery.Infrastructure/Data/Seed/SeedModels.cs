using System.Text.Json.Serialization;

namespace TechnicalMastery.Infrastructure.Data.Seed;

/// <summary>
/// Deserialization shapes for the embedded seed JSON files.
/// Questions reference categories/topics by NAME (stable, reviewable);
/// the seeder resolves them to ids. Enums parse from their names
/// (e.g. "Beginner", "Conceptual") and are validated on load.
/// </summary>
public class SeedCategory
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("displayOrder")]
    public int DisplayOrder { get; set; }
}

public class SeedTopic
{
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("displayOrder")]
    public int DisplayOrder { get; set; }
}

public class SeedQuestion
{
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("topic")]
    public string Topic { get; set; } = string.Empty;

    [JsonPropertyName("questionText")]
    public string QuestionText { get; set; } = string.Empty;

    [JsonPropertyName("shortAnswer")]
    public string ShortAnswer { get; set; } = string.Empty;

    [JsonPropertyName("detailedAnswer")]
    public string DetailedAnswer { get; set; } = string.Empty;

    [JsonPropertyName("difficultyLevel")]
    public string DifficultyLevel { get; set; } = "Beginner";

    [JsonPropertyName("questionType")]
    public string QuestionType { get; set; } = "Conceptual";

    [JsonPropertyName("codeExample")]
    public string CodeExample { get; set; } = string.Empty;

    [JsonPropertyName("internalWorking")]
    public string InternalWorking { get; set; } = string.Empty;

    [JsonPropertyName("realWorldUsage")]
    public string RealWorldUsage { get; set; } = string.Empty;

    [JsonPropertyName("commonMistake")]
    public string CommonMistake { get; set; } = string.Empty;

    [JsonPropertyName("technicalConversation")]
    public string TechnicalConversation { get; set; } = string.Empty;

    [JsonPropertyName("interviewFollowUp")]
    public string InterviewFollowUp { get; set; } = string.Empty;

    [JsonPropertyName("keyTakeaway")]
    public string KeyTakeaway { get; set; } = string.Empty;

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new List<string>();
}
