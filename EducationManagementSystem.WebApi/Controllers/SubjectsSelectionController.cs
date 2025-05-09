using EducationManagementSystem.Application.Features.SubjectsSelection;
using EducationManagementSystem.Application.Features.SubjectsSelection.Dtos;
using EducationManagementSystem.WebApi.Controllers.Shared;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EducationManagementSystem.WebApi.Controllers;

public class SubjectsSelectionController : BaseController
{
    private readonly ISubjectsSelectionService _subjectsSelectionService;

    public SubjectsSelectionController(ISubjectsSelectionService subjectsSelectionService)
    {
        _subjectsSelectionService = subjectsSelectionService;
    }
    
    [HttpGet]
    [SwaggerOperation(Summary = "Get all selected subject groups")]
    public async Task<IReadOnlyList<SelectedSubjectGroupDto>> GetAll()
    {
        return await _subjectsSelectionService.GetAllAsync();
    }
    
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get selected subject group by ID")]
    public async Task<SelectedSubjectGroupDto> GetById(Guid id)
    {
        return await _subjectsSelectionService.GetByIdAsync(id);
    }
    
    [HttpPost]
    [SwaggerOperation(Summary = "Add a new selected subject group")]
    public async Task Add(CreateSelectedSubjectGroupDto dto)
    {
        await _subjectsSelectionService.AddAsync(dto);
    }
    
    [HttpPost("{groupId}/students/{studentId}")]
    [SwaggerOperation(Summary = "Add a student to a selected subject group")]
    public async Task AddStudent(Guid groupId, Guid studentId)
    {
        await _subjectsSelectionService.AddStudentToGroupAsync(groupId, studentId);
    }
    
    [HttpDelete("{groupId}/students/{studentId}")]
    [SwaggerOperation(Summary = "Remove a student from a selected subject group")]
    public async Task RemoveStudent(Guid groupId, Guid studentId)
    {
        await _subjectsSelectionService.RemoveStudentFromGroupAsync(groupId, studentId);
    }
    
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete selected subject group")]
    public async Task Delete(Guid id)
    {
        await _subjectsSelectionService.DeleteAsync(id);
    }
}
