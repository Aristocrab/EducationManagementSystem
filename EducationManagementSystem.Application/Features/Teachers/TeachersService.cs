using EducationManagementSystem.Application.Database;
using EducationManagementSystem.Application.Features.Auth.Dtos;
using EducationManagementSystem.Application.Features.Auth.Models;
using EducationManagementSystem.Application.Features.PasswordHashing;
using EducationManagementSystem.Application.Features.Teachers.Dtos;
using FluentValidation;
using EducationManagementSystem.Application.Extensions;
using EducationManagementSystem.Core.Enums;
using EducationManagementSystem.Core.Exceptions;
using EducationManagementSystem.Core.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Throw;

namespace EducationManagementSystem.Application.Features.Teachers;

public class TeachersService : ITeachersService
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<RegisterDto> _registerDtoValidator;
    private readonly IPasswordHashingService _passwordHashingService;

    public TeachersService(AppDbContext dbContext, 
        IValidator<RegisterDto> registerDtoValidator,
        IPasswordHashingService passwordHashingService)
    {
        _dbContext = dbContext;
        _registerDtoValidator = registerDtoValidator;
        _passwordHashingService = passwordHashingService;
    }
    
    public async Task<List<TeacherDto>> GetAllTeachers(User currentUser)
    {
        currentUser.Throw().IfNotAdminOrModerator();
        
        var teachers = await _dbContext.Teachers
            .AsNoTracking()
            .Include(x => x.Lessons)
            .ThenInclude(x => x.Student)
            .AsSplitQuery()
            .ProjectToType<TeacherDto>()
            .ToListAsync();

        return teachers;
    }
    
    public async Task<TeacherDto> GetTeacherById(Guid teacherId, User currentUser)
    {
        if (currentUser.Id != teacherId)
        {
            currentUser.Throw().IfNotAdminOrModerator();
        }
        
        var teacher = await _dbContext.Teachers
            .AsNoTracking()
            .Include(x => x.Lessons)
            .ThenInclude(x => x.Student)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == teacherId);
        teacher.ThrowIfNull(_ => new NotFoundException("Teacher not found"));

        return teacher.Adapt<TeacherDto>();
    }
    
    public async Task RegisterTeacher(RegisterDto registerDto, User currentUser, Role role = Role.Teacher)
    {
        await _registerDtoValidator.ValidateAndThrowAsync(registerDto);
        currentUser.Throw().IfNotAdminOrModerator();

        if (await _dbContext.Teachers.AnyAsync(t => t.Username == registerDto.Username))
        {
            throw new UserAlreadyExistsException("User already exists");
        }

        var (passwordHash, passwordSalt) = _passwordHashingService.HashPassword(registerDto.Password);
        var newTeacher = new Teacher
        {
            Role = role,
            FullName = registerDto.FullName,
            Username = registerDto.Username,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            RegisteredAt = DateTime.UtcNow,
        };

        await _dbContext.Teachers.AddAsync(newTeacher);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task EditTeacherWorkingHours(Guid teacherId, string workingHours, User currentUser)
    {
        var teacher = await _dbContext.Teachers.FirstOrDefaultAsync(t => t.Id == teacherId);
        teacher.ThrowIfNull();
        
        if(currentUser.Role == Role.Moderator && (teacher.Role is Role.Admin or Role.Moderator))
        {
            throw new UnauthorizedException("Moderators can't edit admins and moderators");
        }
        
        if(currentUser.Id != teacherId && currentUser.Role != Role.Admin)
        {
            throw new UnauthorizedException("You can't edit other teachers");
        }
        
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task EditTeacherBalance(Guid teacherId, decimal balance, User currentUser)
    {
        currentUser.Throw().IfNotAdminOrModerator();
        
        var teacher = await _dbContext.Teachers.FirstOrDefaultAsync(t => t.Id == teacherId);
        teacher.ThrowIfNull();
        
        if(currentUser.Role == Role.Moderator && (teacher.Role is Role.Admin or Role.Moderator))
        {
            throw new UnauthorizedException("Moderators can't edit admins and moderators");
        }

        await _dbContext.SaveChangesAsync();
    }
    
    public async Task DeleteTeacher(Guid teacherId, User currentUser)
    {
        currentUser.Throw().IfNotAdminOrModerator();
        
        var teacher = await _dbContext.Teachers.FirstOrDefaultAsync(t => t.Id == teacherId);
        teacher.ThrowIfNull();
        
        if(teacher.Role == Role.Admin)
        {
            throw new UnauthorizedException("You can't delete admin");
        }
        
        if(currentUser.Role == Role.Moderator && (teacher.Role is Role.Admin or Role.Moderator))
        {
            throw new UnauthorizedException("Moderators can't delete admins and moderators");
        }
        
        _dbContext.Teachers.Remove(teacher);
        await _dbContext.SaveChangesAsync();
    }
}