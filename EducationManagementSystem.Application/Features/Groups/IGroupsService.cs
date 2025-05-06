using EducationManagementSystem.Application.Features.Groups.Dtos;
using EducationManagementSystem.Application.Shared.Auth.Models;

namespace EducationManagementSystem.Application.Features.Groups;

public interface IGroupsService
{
    Task<IReadOnlyList<GroupDto>> GetAllGroups(User currentUser);
    Task<GroupDto> GetGroupById(Guid id);
    Task AddGroup(NewGroupDto newGroupDto, User currentUser);
    Task EditGroup(Guid groupId, NewGroupDto groupDto, User currentUser);
    Task DeleteGroup(Guid groupId, User currentUser);
}