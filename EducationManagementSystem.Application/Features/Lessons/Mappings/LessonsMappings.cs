using EducationManagementSystem.Application.Features.Lessons.Dtos;
using EducationManagementSystem.Core.Models;
using Mapster;

namespace EducationManagementSystem.Application.Features.Lessons.Mappings;

public class LessonsMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Lesson, LessonDto>()
            .Map(dest => dest.DurationMinutes,
                src => src.Duration.TotalMinutes);
    }
}