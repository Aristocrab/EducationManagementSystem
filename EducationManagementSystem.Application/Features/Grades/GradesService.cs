using EducationManagementSystem.Application.Database;
using EducationManagementSystem.Application.Extensions;
using EducationManagementSystem.Application.Features.Grades.Dtos;
using EducationManagementSystem.Application.Shared.Auth.Models;
using EducationManagementSystem.Core.Exceptions;
using EducationManagementSystem.Core.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Throw;

namespace EducationManagementSystem.Application.Features.Grades;

public class GradesService : IGradesService
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<NewSubjectGradeDto> _gradeValidator;

    public GradesService(AppDbContext dbContext, IValidator<NewSubjectGradeDto> gradeValidator)
    {
        _dbContext = dbContext;
        _gradeValidator = gradeValidator;
    }

    public async Task<IReadOnlyList<SubjectGradeDto>> GetAllGrades(User currentUser)
    {
        return await _dbContext.SubjectGrades
            .Include(g => g.Student)
            .Include(g => g.Subject)
            .Select(g => new SubjectGradeDto
            {
                Id = g.Id,
                StudentName = g.Student.FullName,
                SubjectTitle = g.Subject.Title,
                Value = g.Value,
                IssuedAt = g.IssuedAt
            })
            .ToListAsync();
    }

    public async Task<SubjectGradeDto> GetGradeById(Guid id)
    {
        var grade = await _dbContext.SubjectGrades
            .Include(g => g.Student)
            .Include(g => g.Subject)
            .FirstOrDefaultAsync(g => g.Id == id);

        grade.ThrowIfNull(_ => new NotFoundException("Grade not found"));

        return new SubjectGradeDto
        {
            Id = grade.Id,
            StudentName = grade.Student.FullName,
            SubjectTitle = grade.Subject.Title,
            Value = grade.Value,
            IssuedAt = grade.IssuedAt
        };
    }

    public async Task AddGrade(NewSubjectGradeDto newGradeDto, User currentUser)
    {
        await _gradeValidator.ValidateAndThrowAsync(newGradeDto);
        currentUser.Throw().IfNotAdminOrModerator();

        var student = await _dbContext.Students.FirstOrDefaultAsync(s => s.Id == newGradeDto.StudentId);
        student.ThrowIfNull(_ => new NotFoundException("Student not found"));

        var subject = await _dbContext.Subjects.FirstOrDefaultAsync(s => s.Id == newGradeDto.SubjectId);
        subject.ThrowIfNull(_ => new NotFoundException("Subject not found"));

        var grade = new SubjectGrade
        {
            Student = student,
            Subject = subject,
            Value = newGradeDto.Value,
            IssuedAt = newGradeDto.IssuedAt
        };

        await _dbContext.SubjectGrades.AddAsync(grade);
        await _dbContext.SaveChangesAsync();
    }

    public async Task EditGrade(Guid gradeId, NewSubjectGradeDto gradeDto, User currentUser)
    {
        await _gradeValidator.ValidateAndThrowAsync(gradeDto);
        currentUser.Throw().IfNotAdminOrModerator();

        var grade = await _dbContext.SubjectGrades
            .Include(g => g.Student)
            .Include(g => g.Subject)
            .FirstOrDefaultAsync(g => g.Id == gradeId);
        grade.ThrowIfNull(_ => new NotFoundException("Grade not found"));

        grade.Value = gradeDto.Value;
        grade.IssuedAt = gradeDto.IssuedAt;

        if (grade.Student.Id != gradeDto.StudentId)
        {
            var newStudent = await _dbContext.Students.FirstOrDefaultAsync(s => s.Id == gradeDto.StudentId);
            newStudent.ThrowIfNull(_ => new NotFoundException("New student not found"));
            grade.Student = newStudent;
        }

        if (grade.Subject.Id != gradeDto.SubjectId)
        {
            var newSubject = await _dbContext.Subjects.FirstOrDefaultAsync(s => s.Id == gradeDto.SubjectId);
            newSubject.ThrowIfNull(_ => new NotFoundException("New subject not found"));
            grade.Subject = newSubject;
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteGrade(Guid gradeId, User currentUser)
    {
        currentUser.Throw().IfNotAdminOrModerator();

        var grade = await _dbContext.SubjectGrades.FirstOrDefaultAsync(g => g.Id == gradeId);
        grade.ThrowIfNull(_ => new NotFoundException("Grade not found"));

        _dbContext.SubjectGrades.Remove(grade);
        await _dbContext.SaveChangesAsync();
    }
}
