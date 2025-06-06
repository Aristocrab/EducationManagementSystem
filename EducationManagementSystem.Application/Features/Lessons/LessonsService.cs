﻿using EducationManagementSystem.Application.Database;
using EducationManagementSystem.Application.Features.Lessons.Dtos;
using FluentDate;
using EducationManagementSystem.Application.Extensions;
using EducationManagementSystem.Application.Shared.Auth.Models;
using EducationManagementSystem.Application.Shared.Clock;
using EducationManagementSystem.Core.Enums;
using EducationManagementSystem.Core.Exceptions;
using EducationManagementSystem.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Throw;

namespace EducationManagementSystem.Application.Features.Lessons;

public class LessonsService : ILessonsService
{
    private readonly AppDbContext _dbContext;
    private readonly IClock _clock;
    private readonly ILogger<LessonsService> _logger;

    public LessonsService(AppDbContext dbContext, IClock clock,
        ILogger<LessonsService> logger)
    {
        _dbContext = dbContext;
        _clock = clock;
        _logger = logger;
    }
    
    public async Task<Guid> AddLesson(Guid teacherId, NewLessonDto newLesson, User currentUser)
    {
        currentUser.Throw().IfNotAdminOrModerator();
        
        var teacher = await _dbContext.Teachers
            .Include(x => x.Lessons)
            .FirstOrDefaultAsync(x => x.Id == teacherId);
        teacher.ThrowIfNull(_ => new NotFoundException("Teacher not found"));

        var existingStudent = await _dbContext.Students.FirstOrDefaultAsync(x => x.Id == newLesson.ExistingStudentId);
        existingStudent.ThrowIfNull(_ => new NotFoundException("Student not found"));

        var subject = await _dbContext.Subjects
            .FirstOrDefaultAsync(x => x.Id == newLesson.SubjectId);
        subject.ThrowIfNull(_ => new NotFoundException("Subject not found"));
        
        var lessonToAdd = new Lesson
        {
            Student = existingStudent,
            DateTime = newLesson.DateTime,
            Description = newLesson.Description,
            Teacher = teacher,
            Subject = subject,
            Duration = newLesson.DurationMinutes.Minutes(),
        };

        CheckLessonTimeTaken(lessonToAdd, teacher);
        teacher.Lessons.Add(lessonToAdd);
        
        // Add lesson for next week
        if (lessonToAdd.DateTime >= _clock.Today.StartOfWeek())
        {
            var lessonNextWeek = new Lesson
            {
                Student = existingStudent,
                DateTime = newLesson.DateTime.AddDays(7),
                Description = newLesson.Description,
                Teacher = teacher,
                Subject = subject,
                Duration = newLesson.DurationMinutes.Minutes(),
            };
            
            try
            {
                CheckLessonTimeTaken(lessonNextWeek, teacher);
                teacher.Lessons.Add(lessonNextWeek);
            }
            catch
            {
                // Lesson time taken next week, do not add
            }
        }
        
        await _dbContext.SaveChangesAsync();
        
        return lessonToAdd.Id;
    }
    
    public async Task ChangeLessonStatus(Guid teacherId, Guid lessonId, Status newStatus, User currentUser)
    {
        if (currentUser.Id != teacherId)
        {
            currentUser.Throw().IfNotAdminOrModerator();
        }
        
        var teacher = await _dbContext.Teachers
            .Include(x => x.Lessons)
            .ThenInclude(x => x.Student)
            .FirstOrDefaultAsync(x => x.Id == teacherId);
        teacher.ThrowIfNull(_ => new NotFoundException("Teacher not found"));

        var lesson = teacher.Lessons
            .Find(x => x.Id == lessonId);
        lesson.ThrowIfNull(_ => new NotFoundException("Lesson not found"));
        
        if(lesson.Status == newStatus) return;

        lesson.Status = newStatus;

        await _dbContext.SaveChangesAsync();
    }
    
    public async Task EditLesson(Guid teacherId, Guid lessonId, NewLessonDto newLesson, User currentUser)
    {
        currentUser.Throw().IfNotAdminOrModerator();
        
        var teacher = await _dbContext.Teachers
            .Include(x => x.Lessons)
            .FirstOrDefaultAsync(x => x.Id == teacherId);
        teacher.ThrowIfNull(_ => new NotFoundException("Teacher not found"));

        var lessonToEdit = teacher.Lessons.Find(x => x.Id == lessonId);
        lessonToEdit.ThrowIfNull(_ => new NotFoundException("Lesson not found"));
        
            var existingStudent = await _dbContext.Students.FirstOrDefaultAsync(x => x.Id == newLesson.ExistingStudentId);
            existingStudent.ThrowIfNull(_ => new NotFoundException("Student not found"));

        // Check if there is a lesson next week and edit it as well
        if (_dbContext.Lessons.Include(x => x.Student)
            .Any(IsSameLessonNextWeek(lessonToEdit)))
        {
            var lessonNextWeek = await _dbContext.Lessons.FirstAsync(x => x.DateTime == lessonToEdit.DateTime.AddDays(7));
            var newLessonNextWeek = newLesson with { DateTime = lessonToEdit.DateTime.AddDays(7) };
            
            try
            {
                await EditLesson(teacherId, lessonNextWeek.Id, newLessonNextWeek, currentUser);
            }
            catch
            {
                // Lesson not found, do nothing
            }
        }
        
        lessonToEdit.Student = existingStudent;
        lessonToEdit.DateTime = newLesson.DateTime;
        lessonToEdit.Description = newLesson.Description;
        lessonToEdit.Duration = newLesson.DurationMinutes.Minutes();

        CheckLessonTimeTaken(lessonToEdit, teacher);
        
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task DeleteLesson(Guid teacherId, Guid lessonId, User currentUser)
    {
        currentUser.Throw().IfNotAdminOrModerator();
        
        var teacher = await _dbContext.Teachers
            .Include(x => x.Lessons)
            .ThenInclude(x => x.Student)
            .FirstOrDefaultAsync(x => x.Id == teacherId);
        teacher.ThrowIfNull(_ => new NotFoundException("Teacher not found"));

        var lessonToDelete = teacher.Lessons.Find(x => x.Id == lessonId);
        lessonToDelete.ThrowIfNull(_ => new NotFoundException("Lesson not found"));
        
        // Check if there is a lesson next week and delete it as well
        if (teacher.Lessons.Any(IsSameLessonNextWeek(lessonToDelete)))
        {
            var lessonNextWeek = teacher.Lessons
                .First(x => x.DateTime == lessonToDelete.DateTime.AddDays(7));
            
            try
            {
                await DeleteLesson(teacherId, lessonNextWeek.Id, currentUser);
            }
            catch
            {
                // Lesson not found, do nothing
            }
        }
        
        _dbContext.Remove(lessonToDelete);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task CopyLessonsToNextWeek()
    {
        _logger.LogInformation("Copying lessons to next week");

        var startOfCurrentWeek = _clock.Today.StartOfWeek();

        var lessonsToCopy = await _dbContext.Lessons
            .Include(x => x.Student)
            .Include(x => x.Teacher)
            .Include(lesson => lesson.Subject)
            .Where(x => x.DateTime >= startOfCurrentWeek)
            .ToListAsync();

        var newLessons = lessonsToCopy.Select(lesson => new
        {
            ExistingStudentId = lesson.Student.Id,
            DateTime = lesson.DateTime.AddDays(7),
            SubjectId = lesson.Subject.Id,
            Description = lesson.Description,
            Teacher = lesson.Teacher,
            DurationMinutes = (int)lesson.Duration.TotalMinutes
        });

        foreach (var newLessonDto in newLessons)
        {
            try
            {
                await AddLesson(newLessonDto.Teacher.Id, new NewLessonDto
                {
                    ExistingStudentId = newLessonDto.ExistingStudentId,
                    SubjectId = newLessonDto.SubjectId,
                    DateTime = newLessonDto.DateTime,
                    Description = newLessonDto.Description,
                    DurationMinutes = newLessonDto.DurationMinutes
                }, new User(Guid.NewGuid(), Role.Admin));
            }
            catch
            {
                // Lesson time taken, do not add
            }
        }
        await _dbContext.SaveChangesAsync();
    }

    private static void CheckLessonTimeTaken(Lesson lessonToAdd, Teacher teacher)
    {
        var lessonToAddEndTime = lessonToAdd.DateTime.AddMinutes(lessonToAdd.Duration.TotalMinutes);
        
        var overlappingLesson = teacher.Lessons
            .Where(x => x != lessonToAdd && x.DateTime.Date == lessonToAdd.DateTime.Date)
            .Select(x => new 
            { 
                StartTime = x.DateTime.ToTimeOnly(), 
                EndTime = x.DateTime.AddMinutes(x.Duration.TotalMinutes).ToTimeOnly(),
                DifferenceInMinutes = (int)(x.DateTime - lessonToAdd.DateTime).TotalMinutes
            })
            .FirstOrDefault(x => 
                (x.StartTime > lessonToAdd.DateTime.ToTimeOnly() && x.StartTime < lessonToAddEndTime.ToTimeOnly()) || 
                (x.EndTime > lessonToAdd.DateTime.ToTimeOnly() && x.EndTime < lessonToAddEndTime.ToTimeOnly()) ||
                x.StartTime == lessonToAdd.DateTime.ToTimeOnly() ||
                x.EndTime == lessonToAddEndTime.ToTimeOnly()
            );
        
        if (overlappingLesson is null) return;
        
        LessonsOverlapException.Throw(overlappingLesson.StartTime, overlappingLesson.DifferenceInMinutes);
    }
    
    private static Func<Lesson, bool> IsSameLessonNextWeek(Lesson lesson)
    {
        return x => x.DateTime == lesson.DateTime.AddDays(7)
                    && x.Student.Id == lesson.Student.Id
                    && x.Duration == lesson.Duration;
    }
}