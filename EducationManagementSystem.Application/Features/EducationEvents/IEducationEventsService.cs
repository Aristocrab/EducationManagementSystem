using EducationManagementSystem.Application.Features.EducationEvents.Dtos;
using EducationManagementSystem.Application.Shared.Auth.Models;

namespace EducationManagementSystem.Application.Features.EducationEvents;

public interface IEducationEventsService
{
    Task<IReadOnlyList<EducationEventDto>> GetAllEducationEvents(User currentUser);
    Task<EducationEventDto> GetEducationEventById(Guid id);
    Task EditEducationEvent(Guid educationEventId, EditEducationEventDto educationEventDto, User currentUser);
    Task DeleteEducationEvent(Guid educationEventId, User currentUser);
}