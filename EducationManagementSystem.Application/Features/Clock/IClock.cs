namespace EducationManagementSystem.Application.Features.Clock;

public interface IClock
{
    DateTime Now { get; }
    DateTime Today { get; }
}