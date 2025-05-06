using EducationManagementSystem.Application.Features.Students.Dtos;

namespace EducationManagementSystem.Application.Features.Groups.Dtos;

public sealed class GroupDto
{
    public Guid Id { get; set; }
    public string GroupId { get; set; } = null!;
    public List<StudentDto> Students { get; set; } = [];
}