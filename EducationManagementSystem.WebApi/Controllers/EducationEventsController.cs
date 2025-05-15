using EducationManagementSystem.Application.Features.EducationEvents;
using EducationManagementSystem.Application.Features.EducationEvents.Dtos;
using EducationManagementSystem.Core.Models;
using EducationManagementSystem.WebApi.Controllers.Shared;
using Microsoft.AspNetCore.Mvc;

namespace EducationManagementSystem.WebApi.Controllers;

[Route("api/education-events")]
public class EducationEventsController : BaseController
{
    private readonly IEducationEventsService _educationEventsService;
    public EducationEventsController(IEducationEventsService educationEventsService) => _educationEventsService = educationEventsService;
    
    [HttpGet]
    public async Task<IReadOnlyList<EducationEventDto>> GetAllEducationEvents() => await _educationEventsService.GetAllEducationEvents(CurrentUser);
    
    [HttpGet("{id}")]
    public async Task<EducationEventDto> GetEducationEventById(Guid id) => await _educationEventsService.GetEducationEventById(id);
    
    [HttpPut("{id}")]
    public async Task EditEducationEvent(Guid id, EditEducationEventDto dto) => await _educationEventsService.EditEducationEvent(id, dto, CurrentUser);
    
    [HttpDelete("{id}")]
    public async Task DeleteEducationEvent(Guid id) => await _educationEventsService.DeleteEducationEvent(id, CurrentUser);
}