using EducationManagementSystem.Application.Features.Lessons.Dtos;
using EducationManagementSystem.Application.Shared.Auth.Models;
using EducationManagementSystem.Core.Enums;

namespace EducationManagementSystem.Application.Features.Lessons;

public interface ILessonsService
{
    Task CopyLessonsToNextWeek();
    Task ChangeLessonStatus(Guid teacherId, Guid lessonId, Status newStatus, User currentUser);
    Task<Guid> AddLesson(Guid teacherId, NewLessonDto newLesson, User currentUser);
    Task EditLesson(Guid teacherId, Guid lessonId, NewLessonDto newLesson, User currentUser);
    Task DeleteLesson(Guid teacherId, Guid lessonId, User currentUser);
}