using System.Text.Json.Serialization;

namespace TechnicalMastery.Wpf.Models;

/// <summary>
/// Client-side copies of the API contracts. Deliberately NOT shared with the
/// server project: this client must work against HTTP + JSON exactly like a
/// future mobile app would (Rule: replaceable frontend).
/// Enums deserialize from strings ("Intermediate") via the client's converter.
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

public enum QuestionType
{
    Conceptual = 1,
    CodeBased = 2,
    ScenarioBased = 3,
    Debugging = 4,
    Design = 5
}

public enum StudyStatus
{
    NotStarted = 0,
    Learning = 1,
    Completed = 2,
    NeedsReview = 3
}

public class ApiResponse<T>
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public T? Data { get; set; }

    public List<string> Errors { get; set; } = new List<string>();
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new List<T>();

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }
}

public class CategoryModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }
}

public class TopicModel
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }
}

public class QuestionSummaryModel
{
    public int Id { get; set; }

    public string QuestionText { get; set; } = string.Empty;

    public int TopicId { get; set; }

    public string TopicName { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DifficultyLevel DifficultyLevel { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public QuestionType QuestionType { get; set; }

    public List<string> Tags { get; set; } = new List<string>();

    public bool IsBookmarked { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public StudyStatus Status { get; set; }
}

public class QuestionDetailModel
{
    public int Id { get; set; }

    public string QuestionText { get; set; } = string.Empty;

    public int TopicId { get; set; }

    public string TopicName { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DifficultyLevel DifficultyLevel { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public QuestionType QuestionType { get; set; }

    public string ShortAnswer { get; set; } = string.Empty;

    public string DetailedAnswer { get; set; } = string.Empty;

    public string CodeExample { get; set; } = string.Empty;

    public string InternalWorking { get; set; } = string.Empty;

    public string RealWorldUsage { get; set; } = string.Empty;

    public string CommonMistake { get; set; } = string.Empty;

    public string TechnicalConversation { get; set; } = string.Empty;

    public string InterviewFollowUp { get; set; } = string.Empty;

    public string KeyTakeaway { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = new List<string>();

    public bool IsBookmarked { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public StudyStatus Status { get; set; }
}

public class BookmarkModel
{
    public int Id { get; set; }

    public int QuestionId { get; set; }

    public string QuestionText { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}

public class StudyProgressModel
{
    public int Id { get; set; }

    public int QuestionId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public StudyStatus Status { get; set; }

    public DateTime? LastViewedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public int ReviewCount { get; set; }
}

public class QuestionNoteModel
{
    public int Id { get; set; }

    public int QuestionId { get; set; }

    public string NoteText { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}

public class DashboardSummaryModel
{
    public int TotalQuestions { get; set; }

    public int CompletedCount { get; set; }

    public int RemainingCount { get; set; }

    public int BookmarkCount { get; set; }

    public int NeedsReviewCount { get; set; }

    public List<CategoryProgressModel> Categories { get; set; } = new List<CategoryProgressModel>();
}

public class CategoryProgressModel
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public int Total { get; set; }

    public int Completed { get; set; }

    public int PercentComplete { get; set; }
}
