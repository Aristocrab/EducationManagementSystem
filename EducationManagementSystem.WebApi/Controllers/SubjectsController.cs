using EducationManagementSystem.Application.Features.Subjects;
using EducationManagementSystem.Application.Features.Subjects.Dtos;
using EducationManagementSystem.WebApi.Controllers.Shared;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EducationManagementSystem.WebApi.Controllers;

public class SubjectsController : BaseController
{
    private readonly ISubjectsService _subjectsService;

    public SubjectsController(ISubjectsService subjectsService)
    {
        _subjectsService = subjectsService;
    }
    
    [HttpGet]
    [SwaggerOperation(Summary = "Get all subjects")]
    public async Task<IReadOnlyList<SubjectDto>> GetAll()
    {
        return await _subjectsService.GetAllSubjects(CurrentUser);
    }
    
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get subject by ID")]
    public async Task<SubjectDto> GetById(Guid id)
    {
        return await _subjectsService.GetSubjectById(id);
    }
    
    [HttpPost]
    [SwaggerOperation(Summary = "Add a new subject")]
    public async Task Add(NewSubjectDto dto)
    {
        await _subjectsService.AddSubject(dto, CurrentUser);
    }
    
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Edit subject")]
    public async Task Edit(Guid id, NewSubjectDto dto)
    {
        await _subjectsService.EditSubject(id, dto, CurrentUser);
    }
    
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete subject")]
    public async Task Delete(Guid id)
    {
        await _subjectsService.DeleteSubject(id, CurrentUser);
    }
}