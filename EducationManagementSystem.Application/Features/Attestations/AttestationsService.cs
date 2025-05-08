using EducationManagementSystem.Application.Database;
using EducationManagementSystem.Application.Extensions;
using EducationManagementSystem.Application.Features.Attestations.Dtos;
using EducationManagementSystem.Application.Shared.Auth.Models;
using EducationManagementSystem.Core.Exceptions;
using EducationManagementSystem.Core.Models;
using Microsoft.EntityFrameworkCore;
using Throw;

namespace EducationManagementSystem.Application.Features.Attestations;

public class AttestationsService : IAttestationsService
{
    private readonly AppDbContext _dbContext;

    public AttestationsService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<AttestationDto>> GetAll(User currentUser)
    {
        return await _dbContext.Attestations
            .Include(a => a.Student)
            .Include(a => a.Subject)
            .Select(a => new AttestationDto
            {
                Id = a.Id,
                StudentName = a.Student.FullName,
                SubjectTitle = a.Subject.Title,
                Result = a.Result,
                Date = a.Date
            })
            .ToListAsync();
    }

    public async Task<AttestationDto> GetById(Guid id)
    {
        var attestation = await _dbContext.Attestations
            .Include(a => a.Student)
            .Include(a => a.Subject)
            .FirstOrDefaultAsync(a => a.Id == id);

        attestation.ThrowIfNull(_ => new NotFoundException("Attestation not found"));

        return new AttestationDto
        {
            Id = attestation.Id,
            StudentName = attestation.Student.FullName,
            SubjectTitle = attestation.Subject.Title,
            Result = attestation.Result,
            Date = attestation.Date
        };
    }

    public async Task Add(NewAttestationDto dto, User currentUser)
    {
        currentUser.Throw().IfNotAdminOrModerator();

        var student = await _dbContext.Students.FirstOrDefaultAsync(s => s.Id == dto.StudentId);
        student.ThrowIfNull(_ => new NotFoundException("Student not found"));

        var subject = await _dbContext.Subjects.FirstOrDefaultAsync(s => s.Id == dto.SubjectId);
        subject.ThrowIfNull(_ => new NotFoundException("Subject not found"));

        var attestation = new Attestation
        {
            Student = student,
            Subject = subject,
            Result = dto.Result,
            Date = dto.Date
        };

        await _dbContext.Attestations.AddAsync(attestation);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Edit(Guid id, NewAttestationDto dto, User currentUser)
    {
        currentUser.Throw().IfNotAdminOrModerator();

        var attestation = await _dbContext.Attestations
            .Include(a => a.Student)
            .Include(a => a.Subject)
            .FirstOrDefaultAsync(a => a.Id == id);

        attestation.ThrowIfNull(_ => new NotFoundException("Attestation not found"));

        if (attestation.Student.Id != dto.StudentId)
        {
            var newStudent = await _dbContext.Students.FirstOrDefaultAsync(s => s.Id == dto.StudentId);
            newStudent.ThrowIfNull(_ => new NotFoundException("New student not found"));
            attestation.Student = newStudent;
        }

        if (attestation.Subject.Id != dto.SubjectId)
        {
            var newSubject = await _dbContext.Subjects.FirstOrDefaultAsync(s => s.Id == dto.SubjectId);
            newSubject.ThrowIfNull(_ => new NotFoundException("New subject not found"));
            attestation.Subject = newSubject;
        }

        attestation.Result = dto.Result;
        attestation.Date = dto.Date;

        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(Guid id, User currentUser)
    {
        currentUser.Throw().IfNotAdminOrModerator();

        var attestation = await _dbContext.Attestations.FirstOrDefaultAsync(a => a.Id == id);
        attestation.ThrowIfNull(_ => new NotFoundException("Attestation not found"));

        _dbContext.Attestations.Remove(attestation);
        await _dbContext.SaveChangesAsync();
    }
}