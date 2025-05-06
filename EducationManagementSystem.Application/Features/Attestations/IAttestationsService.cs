using EducationManagementSystem.Application.Features.Attestations.Dtos;
using EducationManagementSystem.Application.Shared.Auth.Models;

namespace EducationManagementSystem.Application.Features.Attestations;

public interface IAttestationsService
{
    Task<IReadOnlyList<AttestationDto>> GetAll(User currentUser);
    Task<AttestationDto> GetById(Guid id);
    Task Add(NewAttestationDto dto, User currentUser);
    Task Edit(Guid id, NewAttestationDto dto, User currentUser);
    Task Delete(Guid id, User currentUser);
}