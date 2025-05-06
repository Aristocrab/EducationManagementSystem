namespace EducationManagementSystem.Application.Shared.Clock;

public interface IClock
{
    DateTime Now { get; }
    DateTime Today { get; }
}