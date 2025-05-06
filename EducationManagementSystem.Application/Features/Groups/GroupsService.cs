using EducationManagementSystem.Application.Database;
using EducationManagementSystem.Application.Extensions;
using EducationManagementSystem.Application.Features.Groups.Dtos;
using EducationManagementSystem.Application.Shared.Auth.Models;
using EducationManagementSystem.Core.Exceptions;
using EducationManagementSystem.Core.Models;
using EducationManagementSystem.Core.ValueTypes;
using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Throw;

namespace EducationManagementSystem.Application.Features.Groups;

public sealed class GroupsService : IGroupsService
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<NewGroupDto> _groupValidator;

    public GroupsService(AppDbContext dbContext, IValidator<NewGroupDto> groupValidator)
    {
        _dbContext = dbContext;
        _groupValidator = groupValidator;
    }

    public async Task<IReadOnlyList<GroupDto>> GetAllGroups(User currentUser)
    {
        return await _dbContext.Groups
            .Include(g => g.Students)
            .ProjectToType<GroupDto>()
            .ToListAsync();
    }

    public async Task<GroupDto> GetGroupById(Guid id)
    {
        var group = await _dbContext.Groups
            .Include(g => g.Students)
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == id);

        group.ThrowIfNull(_ => new NotFoundException("Group not found"));

        return group.Adapt<GroupDto>();
    }

    public async Task AddGroup(NewGroupDto newGroupDto, User currentUser)
    {
        currentUser.Throw().IfNotAdminOrModerator();
        await _groupValidator.ValidateAndThrowAsync(newGroupDto);

        var group = newGroupDto.Adapt<Group>();

        await _dbContext.Groups.AddAsync(group);
        await _dbContext.SaveChangesAsync();
    }

    public async Task EditGroup(Guid groupId, NewGroupDto groupDto, User currentUser)
    {
        currentUser.Throw().IfNotAdminOrModerator();
        await _groupValidator.ValidateAndThrowAsync(groupDto);

        var group = await _dbContext.Groups
            .Include(g => g.Students)
            .FirstOrDefaultAsync(g => g.Id == groupId);

        group.ThrowIfNull(_ => new NotFoundException("Group not found"));

        group.GroupId = GroupId.From(groupDto.GroupId);

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteGroup(Guid groupId, User currentUser)
    {
        currentUser.Throw().IfNotAdminOrModerator();

        var group = await _dbContext.Groups
            .FirstOrDefaultAsync(g => g.Id == groupId);

        group.ThrowIfNull(_ => new NotFoundException("Group not found"));

        _dbContext.Groups.Remove(group);
        await _dbContext.SaveChangesAsync();
    }
}
