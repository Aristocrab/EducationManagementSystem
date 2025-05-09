using EducationManagementSystem.Application.Features.SubjectsSelection.Dtos;

namespace EducationManagementSystem.Application.Features.SubjectsSelection;

public interface ISubjectsSelectionService
{
    Task<IReadOnlyList<SelectedSubjectGroupDto>> GetAllAsync();
    Task<SelectedSubjectGroupDto> GetByIdAsync(Guid groupId);
    Task AddAsync(CreateSelectedSubjectGroupDto dto);
    Task AddStudentToGroupAsync(Guid groupId, Guid studentId);
    Task RemoveStudentFromGroupAsync(Guid groupId, Guid studentId);
    Task DeleteAsync(Guid groupId);
}
