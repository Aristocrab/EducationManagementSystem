namespace EducationManagementSystem.Application.Features.Teachers.Dtos;

public class TeacherDtoShort
{
    public Guid Id { get; set; }
    public required string Role { get; set; }
    
    public required string FullName { get; set; }
    public required string Username { get; set; }
    public required string WorkingHours { get; set; }
}