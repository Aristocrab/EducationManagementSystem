namespace EducationManagementSystem.Application.Features.Subjects.Dtos;

public class SubjectDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public List<string> Teachers { get; set; } = [];
    public List<string> Students { get; set; } = [];
}