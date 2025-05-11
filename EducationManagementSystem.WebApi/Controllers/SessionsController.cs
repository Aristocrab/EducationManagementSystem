using EducationManagementSystem.Application.Features.Sessions;
using EducationManagementSystem.Application.Features.Sessions.Dtos;
using EducationManagementSystem.WebApi.Controllers.Shared;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EducationManagementSystem.WebApi.Controllers;

[Route("sessions")]
public class SessionsController : BaseController
{
    private readonly ISessionsService _sessionsService;

    public SessionsController(ISessionsService sessionsService)
    {
        _sessionsService = sessionsService;
    }
    
    [HttpGet]
    [SwaggerOperation(Summary = "Get all sessions")]
    public async Task<IReadOnlyList<SessionDto>> GetAll()
    {
        return await _sessionsService.GetAll(CurrentUser);
    }
    
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get session by ID")]
    public async Task<SessionDto> GetById(Guid id)
    {
        return await _sessionsService.GetById(id);
    }
    
    [HttpPost]
    [SwaggerOperation(Summary = "Add a new session")]
    public async Task Add(NewSessionDto dto)
    {
        await _sessionsService.Add(dto, CurrentUser);
    }
    
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Edit session")]
    public async Task Edit(Guid id, NewSessionDto dto)
    {
        await _sessionsService.Edit(id, dto, CurrentUser);
    }
    
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete session")]
    public async Task Delete(Guid id)
    {
        await _sessionsService.Delete(id, CurrentUser);
    }
}