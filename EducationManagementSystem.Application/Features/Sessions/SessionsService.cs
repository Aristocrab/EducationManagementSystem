using EducationManagementSystem.Application.Database;
using EducationManagementSystem.Application.Extensions;
using EducationManagementSystem.Application.Features.Sessions.Dtos;
using EducationManagementSystem.Application.Shared.Auth.Models;
using EducationManagementSystem.Core.Exceptions;
using EducationManagementSystem.Core.Models;
using Microsoft.EntityFrameworkCore;
using Throw;

namespace EducationManagementSystem.Application.Features.Sessions;

public class SessionsService : ISessionsService
{
    private readonly AppDbContext _dbContext;

    public SessionsService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<SessionDto>> GetAll(User currentUser)
    {
        return await _dbContext.Sessions
            .Include(s => s.Student)
            .Include(s => s.Subject)
            .Select(s => new SessionDto
            {
                Id = s.Id,
                StudentName = s.Student.FullName,
                SubjectTitle = s.Subject.Title,
                Grade = s.Grade,
                Date = s.Date
            })
            .ToListAsync();
    }

    public async Task<SessionDto> GetById(Guid id)
    {
        var session = await _dbContext.Sessions
            .Include(s => s.Student)
            .Include(s => s.Subject)
            .FirstOrDefaultAsync(s => s.Id == id);

        session.ThrowIfNull(_ => new NotFoundException("Session not found"));

        return new SessionDto
        {
            Id = session.Id,
            StudentName = session.Student.FullName,
            SubjectTitle = session.Subject.Title,
            Grade = session.Grade,
            Date = session.Date
        };
    }

    public async Task Add(NewSessionDto dto, User currentUser)
    {
        currentUser.Throw().IfNotAdminOrModerator();

        var student = await _dbContext.Students.FirstOrDefaultAsync(s => s.Id == dto.StudentId);
        student.ThrowIfNull(_ => new NotFoundException("Student not found"));

        var subject = await _dbContext.Subjects.FirstOrDefaultAsync(s => s.Id == dto.SubjectId);
        subject.ThrowIfNull(_ => new NotFoundException("Subject not found"));

        var session = new Session
        {
            Student = student,
            Subject = subject,
            Grade = dto.Grade,
            Date = dto.Date
        };

        await _dbContext.Sessions.AddAsync(session);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Edit(Guid id, NewSessionDto dto, User currentUser)
    {
        currentUser.Throw().IfNotAdminOrModerator();

        var session = await _dbContext.Sessions
            .Include(s => s.Student)
            .Include(s => s.Subject)
            .FirstOrDefaultAsync(s => s.Id == id);

        session.ThrowIfNull(_ => new NotFoundException("Session not found"));

        if (session.Student.Id != dto.StudentId)
        {
            var newStudent = await _dbContext.Students.FirstOrDefaultAsync(s => s.Id == dto.StudentId);
            newStudent.ThrowIfNull(_ => new NotFoundException("New student not found"));
            session.Student = newStudent;
        }

        if (session.Subject.Id != dto.SubjectId)
        {
            var newSubject = await _dbContext.Subjects.FirstOrDefaultAsync(s => s.Id == dto.SubjectId);
            newSubject.ThrowIfNull(_ => new NotFoundException("New subject not found"));
            session.Subject = newSubject;
        }

        session.Grade = dto.Grade;
        session.Date = dto.Date;

        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(Guid id, User currentUser)
    {
        currentUser.Throw().IfNotAdminOrModerator();

        var session = await _dbContext.Sessions.FirstOrDefaultAsync(s => s.Id == id);
        session.ThrowIfNull(_ => new NotFoundException("Session not found"));

        _dbContext.Sessions.Remove(session);
        await _dbContext.SaveChangesAsync();
    }
}