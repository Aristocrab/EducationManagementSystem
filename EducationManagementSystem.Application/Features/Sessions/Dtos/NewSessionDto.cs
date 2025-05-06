namespace EducationManagementSystem.Application.Features.Sessions.Dtos;

public class NewSessionDto
{
    public Guid StudentId { get; set; }
    public Guid SubjectId { get; set; }
    public decimal Grade { get; set; }
    public DateTime Date { get; set; }
}