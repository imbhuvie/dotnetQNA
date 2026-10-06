namespace TechnicalMastery.Application.DTOs;

public class QuestionNoteDto
{
    public int Id { get; set; }

    public int QuestionId { get; set; }

    public string NoteText { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
