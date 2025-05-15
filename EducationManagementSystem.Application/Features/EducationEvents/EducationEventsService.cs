using EducationManagementSystem.Application.Database;
using EducationManagementSystem.Application.Extensions;
using EducationManagementSystem.Application.Features.EducationEvents.Dtos;
using EducationManagementSystem.Application.Shared.Auth.Models;
using EducationManagementSystem.Core.Exceptions;
using EducationManagementSystem.Core.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Throw;

namespace EducationManagementSystem.Application.Features.EducationEvents;

public sealed class EducationEventsService : IEducationEventsService
{
    private readonly AppDbContext _dbContext;

    public EducationEventsService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<EducationEventDto>> GetAllEducationEvents(User currentUser)
    {
        currentUser.Throw().IfNotAdminOrModerator();

        return await _dbContext.EducationEvents
            .AsNoTracking()
            .ProjectToType<EducationEventDto>()
            .ToListAsync();
    }

    public async Task<EducationEventDto> GetEducationEventById(Guid id)
    {
        var educationEvent = await _dbContext.EducationEvents
            .AsNoTracking()
            .ProjectToType<EducationEventDto>()
            .FirstOrDefaultAsync(e => e.Id == id);

        educationEvent.ThrowIfNull(_ => new NotFoundException("Education event not found"));

        return educationEvent;
    }

    public async Task EditEducationEvent(Guid educationEventId, EditEducationEventDto educationEventDto, User currentUser)
    {
        currentUser.Throw().IfNotAdminOrModerator();

        var educationEvent = await _dbContext.EducationEvents.FirstOrDefaultAsync(e => e.Id == educationEventId);

        educationEvent.ThrowIfNull(_ => new NotFoundException("Education event not found"));

        educationEvent.StartDate = educationEventDto.StartDate;
        educationEvent.EndDate = educationEventDto.EndDate;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteEducationEvent(Guid educationEventId, User currentUser)
    {
        currentUser.Throw().IfNotAdminOrModerator();

        var educationEvent = await _dbContext.EducationEvents.FirstOrDefaultAsync(e => e.Id == educationEventId);

        educationEvent.ThrowIfNull(_ => new NotFoundException("Education event not found"));

        _dbContext.EducationEvents.Remove(educationEvent);
        await _dbContext.SaveChangesAsync();
    }
}