using EducationManagementSystem.Application.Database;
using EducationManagementSystem.Application.Features.Auth.Models;
using EducationManagementSystem.Application.Features.Students.Dtos;
using FluentValidation;
using EducationManagementSystem.Application.Extensions;
using EducationManagementSystem.Core.Exceptions;
using EducationManagementSystem.Core.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Throw;

namespace EducationManagementSystem.Application.Features.Students;

public class StudentsService : IStudentsService
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<NewStudentDto> _studentDtoValidator;

    public StudentsService(AppDbContext dbContext, IValidator<NewStudentDto> studentDtoValidator)
    {
        _dbContext = dbContext;
        _studentDtoValidator = studentDtoValidator;
    }
    
    public async Task<IReadOnlyList<StudentDto>> GetAllStudents(User currentUser)
    {
        return await _dbContext.Students
            .ProjectToType<StudentDto>()
            .ToListAsync();
    }

    public async Task<StudentDto> GetStudentById(Guid id)
    {
        var student = await _dbContext.Students
            .AsNoTracking()
            .Include(x => x.Lessons)
            .ThenInclude(x => x.Teacher)
            .FirstOrDefaultAsync(x => x.Id == id);
        
        student.ThrowIfNull(_ => new NotFoundException("Student not found"));
        
        var ret = student.Adapt<StudentDto>();
        
        return ret;
    }

    public async Task AddStudent(NewStudentDto newStudent, User currentUser)
    {
        await _studentDtoValidator.ValidateAndThrowAsync(newStudent);
        currentUser.Throw().IfNotAdminOrModerator();
        
        var student = newStudent.Adapt<Student>();
        
        await _dbContext.Students.AddAsync(student);
        await _dbContext.SaveChangesAsync();
    }

    public async Task EditStudent(Guid studentId, NewStudentDto student, User currentUser)
    {
        currentUser.Throw().IfNotAdminOrModerator();
        
        var studentToUpdate = await _dbContext.Students.FirstOrDefaultAsync(x => x.Id == studentId);
        studentToUpdate.ThrowIfNull(_ => new NotFoundException("Student not found"));

        studentToUpdate.FullName = student.FullName;
        
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteStudent(Guid studentId, User currentUser)
    {
        currentUser.Throw().IfNotAdminOrModerator();
        
        var studentToDelete = await _dbContext.Students.FirstOrDefaultAsync(x => x.Id == studentId);
        studentToDelete.ThrowIfNull(_ => new NotFoundException("Student not found"));

        _dbContext.Students.Remove(studentToDelete);
        await _dbContext.SaveChangesAsync();
    }
}