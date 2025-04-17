using System.Linq.Expressions;
using EducationManagementSystem.Application.Features.Clock;
using EducationManagementSystem.Application.Features.Lessons.Dtos;
using EducationManagementSystem.Core.Enums;
using EducationManagementSystem.Core.Models;

namespace EducationManagementSystem.Application.Features.Lessons.Predicates;

public class LessonPredicates
{
    public static Expression<Func<Lesson, bool>> UnpaidLessonPredicate(IClock clock) =>
        x => !x.Paid && x.Status == Status.Completed && x.DateTime < clock.Now;

    public static Func<LessonDto, bool> UnpaidLessonDtoPredicate(IClock clock) =>
        x => !x.Paid && x.Status == "Completed" && x.DateTime < clock.Now;
}
