// using FluentAssertions;
// using FluentAssertions.Extensions;
// using FluentDateTime;
// using EducationManagementSystem.Application.Database;
// using EducationManagementSystem.Application.Extensions;
// using EducationManagementSystem.Application.Features.Lessons;
// using EducationManagementSystem.Application.Features.Lessons.Dtos;
// using EducationManagementSystem.Application.Shared.Clock;
// using EducationManagementSystem.Core.Enums;
// using EducationManagementSystem.Core.Exceptions;
// using EducationManagementSystem.Core.Models;
// using EducationManagementSystem.Tests.Shared;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Logging;
// using NSubstitute;
//
// namespace EducationManagementSystem.Tests;
//
// public class LessonsServiceTests : ServiceTests
// {
//     private Teacher _teacher = null!;
//     private AppDbContext _dbContext = null!;
//     private readonly IClock _clock = new KyivTimeClock();
//     private readonly Group _group = new()
//     {
//         Id = Guid.NewGuid(),
//         GroupId = "ІС-12",
//         Students = []
//     };
//     
//     private LessonsService GetLessonsService()
//     {
//         var options = new DbContextOptionsBuilder<AppDbContext>().
//             UseSqlite($"Data Source = {nameof(LessonsServiceTests)}.db")
//             .Options;
//
//         _dbContext = new AppDbContext(options);
//         _dbContext.Database.EnsureDeleted();
//         _dbContext.Database.EnsureCreated();
//         
//         var school = new School
//         {
//             Id = Guid.NewGuid(),
//             Balance = 0
//         };
//         _dbContext.Schools.Add(school);
//         
//         _teacher = new Teacher
//         {
//             Id = Admin.Id,
//             FullName = "Bob",
//             Username = "bobbob",
//             PasswordHash = "hashed_password",
//             PasswordSalt = "password_salt",
//             Role = Role.Teacher,
//             RegisteredAt = _clock.Today.SetTime(10, 00),
//         };
//         _dbContext.Teachers.Add(_teacher);
//         _dbContext.SaveChanges();
//
//         return new LessonsService(_dbContext, _clock, Substitute.For<ILogger<LessonsService>>());
//     }
//
//     [Fact]
//     public async Task AddLesson_WhenNewStudent_ShouldAddLesson()
//     {
//         // Arrange
//         var lessonsService = GetLessonsService();
//         var newLessonDto = new NewLessonDto
//         {
//             StudentName = "John Doe",
//             ExistingStudentId = null,
//             DateTime = _clock.Today.SetTime(10, 00),
//             Description = "Math Lesson",
//             DurationMinutes = 60
//         };
//
//         // Act
//         var addedLessonId = await lessonsService.AddLesson(_teacher.Id, newLessonDto, Admin);
//
//         // Assert
//         var addedLesson = await _dbContext.Lessons
//             .Include(lesson => lesson.Student)
//             .FirstOrDefaultAsync(x => x.Id == addedLessonId);
//         
//         addedLesson.Should().NotBeNull();
//         addedLesson!.Id.Should().Be(addedLessonId);
//         addedLesson.Student.FullName.Should().Be(newLessonDto.StudentName);
//         addedLesson.Description.Should().Be(newLessonDto.Description);
//         
//         var nextWeekLesson = await _dbContext.Lessons
//             .FirstOrDefaultAsync(x => x.DateTime == newLessonDto.DateTime.AddDays(7));
//         nextWeekLesson.Should().NotBeNull();
//     }
//     
//     [Fact]
//     public async Task AddLesson_WithExistingStudent_ShouldAddLesson()
//     {
//         // Arrange
//         var lessonsService = GetLessonsService();
//         
//         var newStudent = new Student
//         {
//             FullName = "Bobert",
//             Group = _group,
//         };
//         _dbContext.Students.Add(newStudent);
//         await _dbContext.SaveChangesAsync();
//         
//         var newLessonDto = new NewLessonDto
//         {
//             StudentName = null,
//             ExistingStudentId = newStudent.Id,
//             DateTime = _clock.Today.SetTime(10, 00),
//             Description = "Test",
//             DurationMinutes = 60
//         };
//
//         // Act
//         var addedLessonId = await lessonsService.AddLesson(_teacher.Id, newLessonDto, Admin);
//
//         // Assert
//         var addedLesson = await _dbContext.Lessons
//             .Include(lesson => lesson.Student)
//             .FirstOrDefaultAsync(x => x.Id == addedLessonId);
//         addedLesson.Should().NotBeNull();
//         addedLesson!.Student.FullName.Should().Be(newStudent.FullName);
//         
//         newStudent.Lessons.Should().Contain(addedLesson);
//         
//         var nextWeekLesson = await _dbContext.Lessons
//             .FirstOrDefaultAsync(x => x.DateTime == newLessonDto.DateTime.AddDays(7));
//         nextWeekLesson.Should().NotBeNull();
//     }
//
//     [Fact]
//     public async Task ChangeLessonStatus_WhenNewStatusIsCompleted_ShouldChangeStatus()
//     {
//         // Arrange
//         var lessonsService = GetLessonsService();
//         var lesson = new Lesson
//         {
//             Student = new Student { 
//                 FullName = "John Doe",
//                 Group = _group,
//             },
//             DateTime = _clock.Today.SetTime(10, 00),
//             Description = "Math Lesson",
//             Teacher = _teacher,
//             Duration = 1.Hours()
//         };
//         _teacher.Lessons.Add(lesson);
//         await _dbContext.SaveChangesAsync();
//
//         // Act
//         await lessonsService.ChangeLessonStatus(_teacher.Id, lesson.Id, Status.Completed, Admin);
//
//         // Assert
//         var updatedLesson = await _dbContext.Lessons.FindAsync(lesson.Id);
//         updatedLesson.Should().NotBeNull();
//         updatedLesson!.Status.Should().Be(Status.Completed);
//     }
//     
//     [Fact]
//     public async Task ChangeLessonStatus_WhenNewStatusIsCancelled_ShouldChangeStatus()
//     {
//         // Arrange
//         var lessonsService = GetLessonsService();
//         var lesson = new Lesson
//         {
//             Student = new Student { FullName = "John Doe", Group = _group},
//             DateTime = _clock.Today.SetTime(10, 00),
//             Description = "Math Lesson",
//             Teacher = _teacher,
//             Duration = 1.Hours()
//         };
//         _teacher.Lessons.Add(lesson);
//         await _dbContext.SaveChangesAsync();
//
//         // Act
//         await lessonsService.ChangeLessonStatus(_teacher.Id, lesson.Id, Status.Cancelled, Admin);
//
//         // Assert
//         var updatedLesson = await _dbContext.Lessons.FindAsync(lesson.Id);
//         updatedLesson.Should().NotBeNull();
//         updatedLesson!.Status.Should().Be(Status.Cancelled);
//     }
//     
//     [Fact]
//     public async Task ChangeLessonStatus_FromCompletedToPending_ShouldChangeStatus()
//     {
//         // Arrange
//         var lessonsService = GetLessonsService();
//         var lesson = new Lesson
//         {
//             Student = new Student { FullName = "John Doe",
//                 Group = _group, },
//             DateTime = _clock.Today.SetTime(10, 00),
//             Description = "Math Lesson",
//             Teacher = _teacher,
//             Duration = 1.Hours(),
//             Status = Status.Completed
//         };
//         _teacher.Lessons.Add(lesson);
//         await _dbContext.SaveChangesAsync();
//
//         // Act
//         await lessonsService.ChangeLessonStatus(_teacher.Id, lesson.Id, Status.Pending, Admin);
//
//         // Assert
//         var updatedLesson = await _dbContext.Lessons.FindAsync(lesson.Id);
//         updatedLesson.Should().NotBeNull();
//         updatedLesson!.Status.Should().Be(Status.Pending);
//     }
//
//     [Theory]
//     [ClassData(typeof(TestsData.OverlappingTimesTheoryData))]
//     public async Task CheckLessonTimeTaken_WhenTimeOverlaps_ShouldThrowException(string existingLessonStartTime, string lessonToAddStartTime, string expectedExceptionMessage)
//     {
//         // Arrange
//         var lessonsService = GetLessonsService();
//     
//         var existingLesson = new Lesson
//         {
//             Student = new Student { FullName = "John Doe", 
//                 Group = _group, },
//             DateTime = new DateTime(DateOnly.FromDateTime(_clock.Today), TimeOnly.Parse(existingLessonStartTime)),
//             Description = "Math Lesson",
//             Teacher = _teacher,
//             Duration = 1.Hours()
//         };
//     
//         var lessonToAdd = new NewLessonDto
//         {
//             StudentName = "John Doe",
//             ExistingStudentId = null,
//             Description = "Math Lesson",
//             DateTime = new DateTime(DateOnly.FromDateTime(_clock.Today), TimeOnly.Parse(lessonToAddStartTime)),
//             DurationMinutes = 60,
//         };
//     
//         _teacher.Lessons.Add(existingLesson);
//         await _dbContext.SaveChangesAsync();
//     
//         // Act
//         var act = async () => await lessonsService.AddLesson(_teacher.Id, lessonToAdd, Admin);
//     
//         // Assert
//         await act.Should()
//             .ThrowAsync<LessonsOverlapException>()
//             .WithMessage(expectedExceptionMessage);
//     }
//
//     [Theory]
//     [ClassData(typeof(TestsData.OverlappingTimesWithDurationsTheoryData))]
//     public async Task CheckLessonTimeTaken_WhenTimeOverlapsWithDifferentDurations_ShouldThrowException(string existingLessonStartTime,
//         int existingLessonDurationMinutes, string lessonToAddStartTime, int lessonToAddDurationMinutes)
//     {
//         // Arrange
//         var lessonsService = GetLessonsService();
//     
//         var existingLesson = new Lesson
//         {
//             Student = new Student { FullName = "John Doe", Group = _group},
//             DateTime = new DateTime(_clock.Today.ToDateOnly(), TimeOnly.Parse(existingLessonStartTime)),
//             Description = "Math Lesson",
//             Teacher = _teacher,
//             Duration = existingLessonDurationMinutes.Minutes()
//         };
//     
//         var lessonToAdd = new NewLessonDto
//         {
//             StudentName = "John Doe",
//             ExistingStudentId = null,
//             Description = "Math Lesson",
//             DateTime = new DateTime(_clock.Today.ToDateOnly(), TimeOnly.Parse(lessonToAddStartTime)),
//             DurationMinutes = lessonToAddDurationMinutes,
//         };
//     
//         _teacher.Lessons.Add(existingLesson);
//         await _dbContext.SaveChangesAsync();
//     
//         // Act
//         var act = async () => await lessonsService.AddLesson(_teacher.Id, lessonToAdd, Admin);
//     
//         // Assert
//         await act.Should()
//             .ThrowAsync<LessonsOverlapException>();
//     }
//     
//     [Theory]
//     [ClassData(typeof(TestsData.NonOverlappingTimesTheoryData))]
//     public async Task CheckLessonTimeTaken_WhenTimeDoesNotOverlap_ShouldNotThrowException(string existingLessonStartTime, string lessonToAddStartTime)
//     {
//         // Arrange
//         var lessonsService = GetLessonsService();
//     
//         var existingLesson = new Lesson
//         {
//             Student = new Student { FullName = "John Doe", Group = _group},            
//             DateTime = new DateTime(DateOnly.FromDateTime(_clock.Today), TimeOnly.Parse(existingLessonStartTime)),
//             Description = "Math Lesson",
//             Teacher = _teacher,
//             Duration = 1.Hours()
//         };
//     
//         var lessonToAdd = new NewLessonDto
//         {
//             StudentName = "John Doe",
//             ExistingStudentId = null,
//             DateTime = new DateTime(DateOnly.FromDateTime(_clock.Today), TimeOnly.Parse(lessonToAddStartTime)),
//             Description = "Math Lesson",
//             DurationMinutes = 60,
//         };
//     
//         _teacher.Lessons.Add(existingLesson);
//         await _dbContext.SaveChangesAsync();
//     
//         // Act
//         var act = async () => await lessonsService.AddLesson(_teacher.Id, lessonToAdd, Admin);
//     
//         // Assert
//         await act.Should()
//             .NotThrowAsync();
//     }
//     
//     [Theory]
//     [ClassData(typeof(TestsData.NonOverlappingTimesWithDurationsTheoryData))]
//     public async Task CheckLessonTimeTaken_WhenTimeDoesNotOverlapWithDifferentDurations_ShouldNotThrowException(string existingLessonStartTime,
//         int existingLessonDurationMinutes, string lessonToAddStartTime, int lessonToAddDurationMinutes)
//     {
//         // Arrange
//         var lessonsService = GetLessonsService();
//     
//         var existingLesson = new Lesson
//         {
//             Student = new Student { FullName = "John Doe", Group = _group},            
//             DateTime = new DateTime(DateOnly.FromDateTime(_clock.Today), TimeOnly.Parse(existingLessonStartTime)),
//             Description = "Math Lesson",
//             Teacher = _teacher,
//             Duration = existingLessonDurationMinutes.Minutes()
//         };
//     
//         var lessonToAdd = new NewLessonDto
//         {
//             StudentName = "John Doe",
//             ExistingStudentId = null,
//             DateTime = new DateTime(DateOnly.FromDateTime(_clock.Today), TimeOnly.Parse(lessonToAddStartTime)),
//             Description = "Math Lesson",
//             DurationMinutes = lessonToAddDurationMinutes,
//         };
//     
//         _teacher.Lessons.Add(existingLesson);
//         await _dbContext.SaveChangesAsync();
//     
//         // Act
//         var act = async () => await lessonsService.AddLesson(_teacher.Id, lessonToAdd, Admin);
//     
//         // Assert
//         await act.Should()
//             .NotThrowAsync();
//     }
//     
//     [Fact]
//     public async Task EditLesson_ShouldEditLesson()
//     {
//         // Arrange
//         var lessonsService = GetLessonsService();
//         var lesson = new Lesson
//         {
//             Student = new Student { FullName = "John Doe", Group = _group},
//             DateTime = _clock.Today.SetTime(10, 00),
//             Description = "Math Lesson",
//             Teacher = _teacher,
//             Duration = 1.Hours()
//         };
//         _teacher.Lessons.Add(lesson);
//         await _dbContext.SaveChangesAsync();
//
//         var newLessonDto = new NewLessonDto
//         {
//             StudentName = "Jane Doe",
//             ExistingStudentId = null,
//             DateTime = _clock.Today.SetTime(10, 00),
//             Description = "History Lesson",
//             DurationMinutes = 60,
//         };
//
//         // Act
//         await lessonsService.EditLesson(_teacher.Id, lesson.Id, newLessonDto, Admin);
//
//         // Assert
//         var editedLesson = await _dbContext.Lessons
//             .Include(x => x.Student)
//             .FirstOrDefaultAsync(x => x.Id == lesson.Id);
//         editedLesson.Should().NotBeNull();
//         editedLesson!.Student.FullName.Should().Be(newLessonDto.StudentName);
//         editedLesson.DateTime.Should().Be(newLessonDto.DateTime);
//         editedLesson.Description.Should().Be(newLessonDto.Description);
//     }
//     
//     [Fact]
//     public async Task EditLesson_ShouldAlsoEditLessonNextWeek()
//     {
//         // Arrange
//         var lessonsService = GetLessonsService();
//         var lesson1 = new Lesson
//         {
//             Student = new Student { FullName = "John Doe", Group = _group},
//             DateTime = _clock.Today.SetTime(10, 00),
//             Description = "Math Lesson",
//             Teacher = _teacher,
//             Duration = 1.Hours()
//         };
//         var lesson2 = new Lesson
//         {
//             Student = lesson1.Student,
//             DateTime = _clock.Today.AddDays(7).SetTime(10, 00),
//             Description = "Math Lesson",
//             Teacher = _teacher,
//             Duration = 1.Hours()
//         };
//         _teacher.Lessons.AddRange([lesson1, lesson2]);
//         await _dbContext.SaveChangesAsync();
//
//         var newLessonDto = new NewLessonDto
//         {
//             StudentName = "Jane Doe",
//             ExistingStudentId = null,
//             DateTime = _clock.Today.SetTime(10, 00),
//             Description = "History Lesson",
//             DurationMinutes = 60,
//         };
//
//         // Act
//         await lessonsService.EditLesson(_teacher.Id, lesson1.Id, newLessonDto, Admin);
//
//         // Assert
//         var editedLesson1 = await _dbContext.Lessons
//             .Include(x => x.Student)
//             .FirstOrDefaultAsync(x => x.Id == lesson1.Id);
//         editedLesson1.Should().NotBeNull();
//         
//         var editedLesson2 = await _dbContext.Lessons
//             .Include(x => x.Student)
//             .FirstOrDefaultAsync(x => x.Id == lesson2.Id);
//         editedLesson2.Should().NotBeNull();
//         
//         editedLesson1!.Student.FullName.Should().Be(newLessonDto.StudentName);
//         editedLesson1.DateTime.Should().Be(newLessonDto.DateTime);
//         editedLesson1.Description.Should().Be(newLessonDto.Description);
//         
//         editedLesson2!.Student.FullName.Should().Be(newLessonDto.StudentName);
//         editedLesson2.DateTime.Should().Be(newLessonDto.DateTime.AddDays(7));
//         editedLesson2.Description.Should().Be(newLessonDto.Description);
//     }
//
//     [Fact]
//     public async Task DeleteLesson_ShouldDeleteLesson()
//     {
//         // Arrange
//         var lessonsService = GetLessonsService();
//         var lesson = new Lesson
//         {
//             Student = new Student { FullName = "John Doe", Group = _group},
//             DateTime = _clock.Today.SetTime(10, 00),
//             Description = "Math Lesson",
//             Teacher = _teacher,
//             Duration = 1.Hours()
//         };
//         _teacher.Lessons.Add(lesson);
//         await _dbContext.SaveChangesAsync();
//
//         // Act
//         await lessonsService.DeleteLesson(_teacher.Id, lesson.Id, Admin);
//
//         // Assert
//         var deletedLesson = await _dbContext.Lessons.FindAsync(lesson.Id);
//         deletedLesson.Should().BeNull();
//     }
//     
//     [Fact]
//     public async Task DeleteLesson_ShouldAlsoDeleteLessonNextWeek()
//     {
//         // Arrange
//         var lessonsService = GetLessonsService();
//         var lesson1 = new Lesson
//         {
//             Student = new Student { FullName = "John Doe", Group = _group},
//             DateTime = _clock.Today.SetTime(10, 00),
//             Description = "Math Lesson",
//             Teacher = _teacher,
//             Duration = 1.Hours()
//         };
//         var lesson2 = new Lesson
//         {
//             Student = lesson1.Student,
//             DateTime = _clock.Today.AddDays(7).SetTime(10, 00),
//             Description = "Math Lesson",
//             Teacher = _teacher,
//             Duration = 1.Hours()
//         };
//         _teacher.Lessons.AddRange([lesson1, lesson2]);
//         await _dbContext.SaveChangesAsync();
//
//         // Act
//         await lessonsService.DeleteLesson(_teacher.Id, lesson1.Id, Admin);
//
//         // Assert
//         var deletedLesson1 = await _dbContext.Lessons.FindAsync(lesson1.Id);
//         deletedLesson1.Should().BeNull();
//         
//         var deletedLesson2 = await _dbContext.Lessons.FindAsync(lesson2.Id);
//         deletedLesson2.Should().BeNull();
//     }
//
//     [Fact]
//     public async Task MoveLessonsToNextWeek_ShouldMoveLessons()
//     {
//         // Arrange
//         var lessonsService = GetLessonsService();
//         var lesson1Old = new Lesson
//         {
//             Student = new Student { FullName = "John Doe", Group = _group},
//             DateTime = _clock.Today.AddDays(-7).SetTime(10, 00),
//             Description = "Math Lesson",
//             Teacher = _teacher,
//             Duration = 1.Hours(),
//             Status = Status.Completed
//         };
//         var lesson2Old = new Lesson
//         {
//             Student = new Student { FullName = "Jane Doe", Group = _group },
//             DateTime = _clock.Today.AddDays(-7).SetTime(10, 00),
//             Description = "History Lesson",
//             Teacher = _teacher,
//             Duration = 1.Hours(),
//             Status = Status.Completed
//         };
//         var lesson1 = new Lesson
//         {
//             Student = new Student { FullName = "John Doe", Group = _group},
//             DateTime = _clock.Today.SetTime(11, 00),
//             Description = "Math Lesson",
//             Teacher = _teacher,
//             Duration = 1.Hours(),
//             Status = Status.Completed
//         };
//         var lesson2 = new Lesson
//         {
//             Student = new Student { FullName = "Jane Doe", Group = _group },
//             DateTime = _clock.Today.SetTime(11, 00),
//             Description = "History Lesson",
//             Teacher = _teacher,
//             Duration = 1.Hours(),
//             Status = Status.Completed
//         };
//         var lesson3 = new Lesson
//         {
//             Student = new Student { FullName = "Jane Doe", Group = _group },
//             DateTime = _clock.Today.SetTime(12, 00),
//             Description = "History Lesson",
//             Teacher = _teacher,
//             Duration = 1.Hours(),
//             Status = Status.Completed
//         };
//         var lesson3New = new Lesson
//         {
//             Student = new Student { FullName = "Jane Doe", Group = _group },
//             DateTime = _clock.Today.AddDays(7).SetTime(12, 00),
//             Description = "History Lesson",
//             Teacher = _teacher,
//             Duration = 1.Hours(),
//             Status = Status.Completed
//         };
//         _teacher.Lessons.AddRange([lesson1Old, lesson2Old, lesson1, lesson2, lesson3, lesson3New]);
//         await _dbContext.SaveChangesAsync();
//
//         // Act
//         await lessonsService.CopyLessonsToNextWeek();
//
//         // Assert
//         var lesson1AfterReset = await _dbContext.Lessons.FindAsync(lesson1.Id);
//         lesson1AfterReset.Should().NotBeNull();
//         
//         var copyOfLesson3NextWeek = await _dbContext.Lessons
//             .Where(x => x.DateTime == lesson3.DateTime.AddDays(7) && x.Description == lesson3.Description)
//             .ToListAsync();
//         copyOfLesson3NextWeek.Should().HaveCount(1);
//         var copyOfLesson3NewNextWeek = await _dbContext.Lessons
//             .Where(x => x.DateTime == lesson3.DateTime.AddDays(14) && x.Description == lesson3.Description)
//             .ToListAsync();
//         copyOfLesson3NewNextWeek.Should().HaveCount(1);
//     }
// }