using EducationManagementSystem.Application.Database;
using EducationManagementSystem.Application.Features.SubjectsSelection.Dtos;
using EducationManagementSystem.Core.Exceptions;
using EducationManagementSystem.Core.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Throw;

namespace EducationManagementSystem.Application.Features.SubjectsSelection;

public sealed class SubjectsSelectionService : ISubjectsSelectionService
{
    private readonly AppDbContext _dbContext;

    public SubjectsSelectionService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<SelectedSubjectGroupDto>> GetAllAsync()
    {
        var groups = await _dbContext.SelectedSubjectGroups
            .Include(g => g.Subject)
            .Include(g => g.Students)
            .ToListAsync();
        
        return groups.Adapt<IReadOnlyList<SelectedSubjectGroupDto>>();
    }

    public async Task<SelectedSubjectGroupDto> GetByIdAsync(Guid groupId)
    {
        var group = await _dbContext.SelectedSubjectGroups
            .Include(g => g.Subject)
            .Include(g => g.Students)
            .FirstOrDefaultAsync(g => g.Id == groupId);
        
        group.ThrowIfNull(_ => new NotFoundException("Selected subject group not found"));
        
        return group.Adapt<SelectedSubjectGroupDto>();
    }

    public async Task AddAsync(CreateSelectedSubjectGroupDto dto)
    {
        var subject = await _dbContext.Subjects.FirstOrDefaultAsync(s => s.Id == dto.SubjectId);
        subject.ThrowIfNull(_ => new NotFoundException("Subject not found"));
        
        var group = dto.Adapt<SelectedSubjectGroup>();
        group.Subject = subject;
        
        await _dbContext.SelectedSubjectGroups.AddAsync(group);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddStudentToGroupAsync(Guid groupId, Guid studentId)
    {
        var group = await _dbContext.SelectedSubjectGroups.Include(g => g.Students).FirstOrDefaultAsync(g => g.Id == groupId);
        group.ThrowIfNull(_ => new NotFoundException("Selected subject group not found"));
        
        var student = await _dbContext.Students.FirstOrDefaultAsync(s => s.Id == studentId);
        student.ThrowIfNull(_ => new NotFoundException("Student not found"));
        
        if (group.Students.Count >= group.MaxStudents) throw new Exception("Group is full");
        
        group.Students.Add(student);
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveStudentFromGroupAsync(Guid groupId, Guid studentId)
    {
        var group = await _dbContext.SelectedSubjectGroups.Include(g => g.Students).FirstOrDefaultAsync(g => g.Id == groupId);
        group.ThrowIfNull(_ => new NotFoundException("Selected subject group not found"));
        
        var student = group.Students.FirstOrDefault(s => s.Id == studentId);
        student.ThrowIfNull(_ => new NotFoundException("Student not found in this group"));
        
        group.Students.Remove(student);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid groupId)
    {
        var group = await _dbContext.SelectedSubjectGroups.FirstOrDefaultAsync(g => g.Id == groupId);
        group.ThrowIfNull(_ => new NotFoundException("Selected subject group not found"));
        
        _dbContext.SelectedSubjectGroups.Remove(group);
        
        await _dbContext.SaveChangesAsync();
    }
}