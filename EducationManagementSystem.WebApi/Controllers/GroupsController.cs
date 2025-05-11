using EducationManagementSystem.Application.Features.Groups;
using EducationManagementSystem.Application.Features.Groups.Dtos;
using EducationManagementSystem.WebApi.Controllers.Shared;
using Microsoft.AspNetCore.Mvc;

namespace EducationManagementSystem.WebApi.Controllers;

[Route("groups")]
public class GroupsController : BaseController
{
    private readonly IGroupsService _groupsService;
    public GroupsController(IGroupsService groupsService) => _groupsService = groupsService;
    
    [HttpGet]
    public async Task<IReadOnlyList<GroupDto>> GetAllGroups() => await _groupsService.GetAllGroups(CurrentUser);
    
    [HttpGet("{id}")]
    public async Task<GroupDto> GetGroupById(Guid id) => await _groupsService.GetGroupById(id);
    
    [HttpPost]
    public async Task AddGroup(NewGroupDto dto) => await _groupsService.AddGroup(dto, CurrentUser);
    
    [HttpPut("{id}")]
    public async Task EditGroup(Guid id, NewGroupDto dto) => await _groupsService.EditGroup(id, dto, CurrentUser);
    
    [HttpDelete("{id}")]
    public async Task DeleteGroup(Guid id) => await _groupsService.DeleteGroup(id, CurrentUser);
}
