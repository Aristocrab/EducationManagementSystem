using EducationManagementSystem.Application.Database;
using EducationManagementSystem.Application.Extensions;
using EducationManagementSystem.Application.Features.Subjects.Dtos;
using EducationManagementSystem.Application.Shared.Auth.Models;
using EducationManagementSystem.Core.Exceptions;
using EducationManagementSystem.Core.Models;
using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Throw;

namespace EducationManagementSystem.Application.Features.Subjects;

public class SubjectsService : ISubjectsService
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<NewSubjectDto> _subjectDtoValidator;

    public SubjectsService(AppDbContext dbContext, IValidator<NewSubjectDto> subjectDtoValidator)
    {
        _dbContext = dbContext;
        _subjectDtoValidator = subjectDtoValidator;
    }

    public async Task<IReadOnlyList<SubjectDto>> GetAllSubjects(User currentUser)
    {
        return await _dbContext.Subjects
            .Include(s => s.Teachers)
            .Include(s => s.Students)
            .ProjectToType<SubjectDto>()
            .ToListAsync();
    }

    public async Task<SubjectDto> GetSubjectById(Guid id)
    {
        var subject = await _dbContext.Subjects
            .Include(s => s.Teachers)
            .Include(s => s.Students)
            .FirstOrDefaultAsync(s => s.Id == id);

        subject.ThrowIfNull(_ => new NotFoundException("Subject not found"));

        return subject.Adapt<SubjectDto>();
    }

    public async Task AddSubject(NewSubjectDto newSubjectDto, User currentUser)
    {
        currentUser.Throw().IfNotAdminOrModerator();

        await _subjectDtoValidator.ValidateAndThrowAsync(newSubjectDto);

        var subject = newSubjectDto.Adapt<Subject>();

        await _dbContext.Subjects.AddAsync(subject);
        await _dbContext.SaveChangesAsync();
    }

    public async Task EditSubject(Guid subjectId, NewSubjectDto subjectDto, User currentUser)
    {
        currentUser.Throw().IfNotAdminOrModerator();

        var subjectToUpdate = await _dbContext.Subjects.FirstOrDefaultAsync(s => s.Id == subjectId);
        subjectToUpdate.ThrowIfNull(_ => new NotFoundException("Subject not found"));

        subjectToUpdate.Title = subjectDto.Title;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteSubject(Guid subjectId, User currentUser)
    {
        currentUser.Throw().IfNotAdminOrModerator();

        var subjectToDelete = await _dbContext.Subjects.FirstOrDefaultAsync(s => s.Id == subjectId);
        subjectToDelete.ThrowIfNull(_ => new NotFoundException("Subject not found"));

        _dbContext.Subjects.Remove(subjectToDelete);
        await _dbContext.SaveChangesAsync();
    }
}