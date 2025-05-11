using EducationManagementSystem.Application.Features.Attestations;
using EducationManagementSystem.Application.Features.Attestations.Dtos;
using EducationManagementSystem.WebApi.Controllers.Shared;
using Microsoft.AspNetCore.Mvc;

namespace EducationManagementSystem.WebApi.Controllers;

[Route("attestations")]
public class AttestationsController : BaseController
{
    private readonly IAttestationsService _attestationsService;
    public AttestationsController(IAttestationsService attestationsService) => _attestationsService = attestationsService;
    
    [HttpGet]
    public async Task<IReadOnlyList<AttestationDto>> GetAll() => await _attestationsService.GetAll(CurrentUser);
    
    [HttpGet("{id}")]
    public async Task<AttestationDto> GetById(Guid id) => await _attestationsService.GetById(id);
    
    [HttpPost]
    public async Task Add(NewAttestationDto dto) => await _attestationsService.Add(dto, CurrentUser);
    
    [HttpPut("{id}")]
    public async Task Edit(Guid id, NewAttestationDto dto) => await _attestationsService.Edit(id, dto, CurrentUser);
    
    [HttpDelete("{id}")]
    public async Task Delete(Guid id) => await _attestationsService.Delete(id, CurrentUser);
}