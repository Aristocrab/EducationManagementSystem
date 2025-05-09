using EducationManagementSystem.Application.Features.Grades;
using EducationManagementSystem.Application.Features.Grades.Dtos;
using EducationManagementSystem.WebApi.Controllers.Shared;
using Microsoft.AspNetCore.Mvc;

namespace EducationManagementSystem.WebApi.Controllers;

public class GradesController : BaseController
{
    private readonly IGradesService _gradesService;
    public GradesController(IGradesService gradesService) => _gradesService = gradesService;
    
    [HttpGet]
    public async Task<IReadOnlyList<SubjectGradeDto>> GetAllGrades() => await _gradesService.GetAllGrades(CurrentUser);
    
    [HttpGet("{id}")]
    public async Task<SubjectGradeDto> GetGradeById(Guid id) => await _gradesService.GetGradeById(id);
    
    [HttpPost]
    public async Task AddGrade(NewSubjectGradeDto dto) => await _gradesService.AddGrade(dto, CurrentUser);
    
    [HttpPut("{id}")]
    public async Task EditGrade(Guid id, NewSubjectGradeDto dto) => await _gradesService.EditGrade(id, dto, CurrentUser);
    
    [HttpDelete("{id}")]
    public async Task DeleteGrade(Guid id) => await _gradesService.DeleteGrade(id, CurrentUser);
}