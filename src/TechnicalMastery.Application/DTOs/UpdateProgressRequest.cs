using TechnicalMastery.Domain.Enums;

namespace TechnicalMastery.Application.DTOs;

public class UpdateProgressRequest
{
    public StudyStatus Status { get; set; }
}
