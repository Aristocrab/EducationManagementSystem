using EducationManagementSystem.Application.Features.Lessons;
using EducationManagementSystem.Application.Features.Lessons.Dtos;
using EducationManagementSystem.WebApi.Controllers.Shared;
using EducationManagementSystem.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EducationManagementSystem.WebApi.Controllers;

public class LessonsController : BaseController
{
    private readonly ILessonsService _lessonsService;

    public LessonsController(ILessonsService lessonsService)
    {
        _lessonsService = lessonsService;
    }
    
    [HttpPost("{teacherId}/changeLessonStatus/{lessonId}")]
    [SwaggerOperation(Summary = "Change lesson status (0 - pending, 1 - completed, 2 - canceled)")]
    public async Task ChangeLessonStatus(Guid teacherId, Guid lessonId, Status newStatus)
    {
        await _lessonsService.ChangeLessonStatus(teacherId, lessonId, newStatus, CurrentUser);
    }
    
    [HttpPost("{teacherId}/addLesson")]
    [SwaggerOperation(Summary = "Add lesson")]
    public async Task<Guid> AddLesson(Guid teacherId, NewLessonDto newLesson)
    {
        return await _lessonsService.AddLesson(teacherId, newLesson, CurrentUser);
    }
    
    [HttpPut("{teacherId}/editLesson/{lessonId}")]
    [SwaggerOperation(Summary = "Edit lesson")]
    public async Task EditLesson(Guid teacherId, Guid lessonId, NewLessonDto newLesson)
    {
        await _lessonsService.EditLesson(teacherId, lessonId, newLesson, CurrentUser);
    }
    
    [HttpDelete("{teacherId}/deleteLesson/{lessonId}")]
    [SwaggerOperation(Summary = "Delete teacher's lesson")]
    public async Task DeleteLesson(Guid teacherId, Guid lessonId)
    {
        await _lessonsService.DeleteLesson(teacherId, lessonId, CurrentUser);
    }
}