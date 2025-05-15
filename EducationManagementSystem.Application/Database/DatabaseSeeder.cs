using EducationManagementSystem.Application.Shared.PasswordHashing;
using EducationManagementSystem.Core.Enums;
using EducationManagementSystem.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace EducationManagementSystem.Application.Database;

public class DatabaseSeeder
{
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly IPasswordHashingService _passwordHashingService;

    public DatabaseSeeder(AppDbContext dbContext, IConfiguration configuration, IPasswordHashingService passwordHashingService)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _passwordHashingService = passwordHashingService;
    }
    
    public void FillDb()
    {
        _dbContext.Database.Migrate();
        
        if (!_dbContext.Schools.Any())
        {
            _dbContext.Schools.Add(new School
            {
                SchoolName = "KPI"
            });
            _dbContext.SaveChanges();
        }
        
        if (!_dbContext.Teachers.Any())
        {
            var adminsFromConfig = _configuration
                .GetSection("Admins")
                .Get<List<Dictionary<string, string>>>()!;
            
            var admins = new List<Teacher>();
            foreach (var admin in adminsFromConfig)
            {        
                var (hash, salt) = _passwordHashingService.HashPassword(admin["Password"]);
                var teacher = new Teacher
                {
                    Id = Guid.Parse(admin["Id"]),
                    Role = Role.Admin,
                    FullName = admin["FullName"],
                    Username = admin["Username"],
                    PasswordHash = hash,
                    PasswordSalt = salt,
                    RegisteredAt = DateTime.UtcNow
                };
                admins.Add(teacher);
            }
            
            _dbContext.Teachers.AddRange(admins);
            _dbContext.SaveChanges();
        }

        if (!_dbContext.Subjects.Any())
        {
            var subjects = new List<Subject>
            {
                new() { Title = "Mathematics" },
                new() { Title = "Physics" },
                new() { Title = "Chemistry" },
                new() { Title = "Biology" },
                new() { Title = "Computer Science" }
            };
            _dbContext.Subjects.AddRange(subjects);
            _dbContext.SaveChanges();
        }

        if (!_dbContext.Groups.Any())
        {
            var groups = new List<Group>
            {
                new() { GroupId = "ІС-11" },
                new() { GroupId = "ІС-12" },
                new() { GroupId = "ІС-13" },
            };
            _dbContext.Groups.AddRange(groups);
            _dbContext.SaveChanges();
        }

        if (!_dbContext.Students.Any())
        {
            var groupA = _dbContext.Groups.First(g => g.GroupId == "ІС-11");
            var groupB = _dbContext.Groups.First(g => g.GroupId == "ІС-12");
            var students = new List<Student>
            {
                new() { FullName = "Alice Johnson", Group = groupA },
                new() { FullName = "Bob Smith", Group = groupA },
                new() { FullName = "Charlie Brown", Group = groupB },
                new() { FullName = "Diana Prince", Group = groupB }
            };
            _dbContext.Students.AddRange(students);
            _dbContext.SaveChanges();
        }

        if (!_dbContext.Lessons.Any())
        {
            var teacher = _dbContext.Teachers.First();
            var students = _dbContext.Students.ToList();
            var subjects = _dbContext.Subjects.ToList();
            var lessons = new List<Lesson>();
            foreach (var student in students)
            {
                foreach (var subject in subjects)
                {
                    lessons.Add(new Lesson
                    {
                        Teacher = teacher,
                        Student = student,
                        Subject = subject,
                        DateTime = DateTime.ParseExact("2025-05-10 10:00", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                        Duration = TimeSpan.FromHours(1),
                        Description = $"{subject.Title} lesson with {student.FullName}"
                    });
                }
            }
            _dbContext.Lessons.AddRange(lessons);
            _dbContext.SaveChanges();
        }

        if (!_dbContext.EducationEvents.Any())
        {
            var events = new List<EducationEvent>
            {
                new() { EventType = EducationEventType.SubjectsSelection, StartDate = new DateTime(2025, 8, 1), EndDate = new DateTime(2025, 8, 20) },
                new() { EventType = EducationEventType.FirstAttestation, StartDate = new DateTime(2025, 4, 28), EndDate = new DateTime(2025, 5, 11) },
                new() { EventType = EducationEventType.SecondAttestation, StartDate = new DateTime(2025, 5, 19), EndDate = new DateTime(2025, 6, 1) },
                new() { EventType = EducationEventType.Session, StartDate = new DateTime(2025, 6, 9), EndDate = new DateTime(2025, 6, 22) }
            };
            _dbContext.EducationEvents.AddRange(events);
            _dbContext.SaveChanges();
        }
    }
}
