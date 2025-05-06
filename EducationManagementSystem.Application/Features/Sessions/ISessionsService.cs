using EducationManagementSystem.Application.Features.Sessions.Dtos;
using EducationManagementSystem.Application.Shared.Auth.Models;

namespace EducationManagementSystem.Application.Features.Sessions;

public interface ISessionsService
{
    Task<IReadOnlyList<SessionDto>> GetAll(User currentUser);
    Task<SessionDto> GetById(Guid id);
    Task Add(NewSessionDto dto, User currentUser);
    Task Edit(Guid id, NewSessionDto dto, User currentUser);
    Task Delete(Guid id, User currentUser);
}