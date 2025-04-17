using EducationManagementSystem.Application.Features.Clock;
using EducationManagementSystem.Application.Features.Lessons.Dtos;
using EducationManagementSystem.Application.Features.Lessons.Predicates;

namespace EducationManagementSystem.Application.Features.Students.Dtos;

public class StudentDto
{
    public required Guid Id { get; set; }
    public required string FullName { get; set; }

    public string? MessengerLink { get; set; }

    public int UnpaidLessons => Lessons.Count(LessonPredicates.UnpaidLessonDtoPredicate(new KyivTimeClock()));
    
    public decimal UnpaidLessonsPrice => Lessons
        .Where(LessonPredicates.UnpaidLessonDtoPredicate(new KyivTimeClock()))
        .Sum(x => x.Price);
    
    public List<string> Languages { get; init; } = [];
    public List<LessonDto> Lessons { get; set; } = [];
}