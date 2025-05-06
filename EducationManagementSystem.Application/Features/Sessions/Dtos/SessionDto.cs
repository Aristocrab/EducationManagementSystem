namespace EducationManagementSystem.Application.Features.Sessions.Dtos;

public class SessionDto
{
    public Guid Id { get; set; }
    public string StudentName { get; set; } = default!;
    public string SubjectTitle { get; set; } = default!;
    public decimal Grade { get; set; }
    public DateTime Date { get; set; }
}