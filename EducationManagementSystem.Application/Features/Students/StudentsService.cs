using EducationManagementSystem.Application.Database;
using EducationManagementSystem.Application.Features.Auth.Models;
using EducationManagementSystem.Application.Features.Clock;
using EducationManagementSystem.Application.Features.Lessons.Predicates;
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
    private readonly IClock _clock;

    public StudentsService(AppDbContext dbContext, IValidator<NewStudentDto> studentDtoValidator, IClock clock)
    {
        _dbContext = dbContext;
        _studentDtoValidator = studentDtoValidator;
        _clock = clock;
    }
    
    public async Task<IReadOnlyList<StudentDto>> GetAllStudents(User currentUser)
    {
        // if(currentUser.Role == Role.Teacher)
        // {
        //     var teacher = await _dbContext.Teachers
        //         .AsNoTracking()
        //         .Include(x => x.Lessons)
        //         .ThenInclude(x => x.Student)
        //         .FirstOrDefaultAsync(x => x.Id == currentUser.Id);
        //     teacher.ThrowIfNull(_ => new NotFoundException("Teacher not found"));
        //     
        //     return teacher.Lessons
        //         .Select(x => x.Student)
        //         .AsQueryable()
        //         .OrderByDescending(x => x.Lessons.Where(y => !y.Paid).Sum(y => (int)y.Price))
        //         .ProjectToType<StudentDto>()
        //         .ToList();
        // }
        
        return await _dbContext.Students
            .OrderByDescending(x => 
                x.Lessons
                    .AsQueryable()
                    .Where(LessonPredicates.UnpaidLessonPredicate(_clock))
                    .Sum(y => (int)y.Price))
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
        studentToUpdate.MessengerLink = student.MessengerLink;
        studentToUpdate.Languages = student.Languages;
        
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