namespace TechnicalMastery.Application.DTOs;

public class BookmarkDto
{
    public int Id { get; set; }

    public int QuestionId { get; set; }

    public string QuestionText { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
