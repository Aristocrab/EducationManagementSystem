using EducationManagementSystem.Application.Features.Students;
using EducationManagementSystem.Application.Features.Students.Dtos;
using EducationManagementSystem.WebApi.Controllers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EducationManagementSystem.WebApi.Controllers;

[Authorize]
[Route("api/students")]
public class StudentsController : BaseController
{
    private readonly IStudentsService _studentsService;

    public StudentsController(IStudentsService studentsService)
    {
        _studentsService = studentsService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get all students")]
    public async Task<IReadOnlyList<StudentDto>> GetAllStudents()
    {
        return await _studentsService.GetAllStudents(CurrentUser);
    }
    
    [HttpGet("{studentId}")]
    [SwaggerOperation(Summary = "Get a student by id")]
    [AllowAnonymous]
    public async Task<StudentDto> GetStudentById(Guid studentId)
    {
        return await _studentsService.GetStudentById(studentId);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Add a new student")]
    public async Task AddStudent([FromBody] NewStudentDto newStudent)
    {
        await _studentsService.AddStudent(newStudent, CurrentUser);
    }

    [HttpPut("{studentId}")]
    [SwaggerOperation(Summary = "Edit a student")]
    public async Task EditStudent(Guid studentId, [FromBody] NewStudentDto student)
    {
        await _studentsService.EditStudent(studentId, student, CurrentUser);
    }

    [HttpDelete("{studentId}")]
    [SwaggerOperation(Summary = "Delete a student")]
    public async Task DeleteStudent(Guid studentId)
    {
        await _studentsService.DeleteStudent(studentId, CurrentUser);
    }
}