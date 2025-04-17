using System.Globalization;
using EducationManagementSystem.Application.Database;
using EducationManagementSystem.Application.Features.Clock;
using EducationManagementSystem.Application.Features.Dashboard.Models;
using EducationManagementSystem.Application.Features.Lessons.Dtos;
using EducationManagementSystem.Core.Enums;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace EducationManagementSystem.Application.Features.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _dbContext;
    private readonly IClock _clock;

    public DashboardService(AppDbContext dbContext, IClock clock)
    {
        _dbContext = dbContext;
        _clock = clock;
    }
    
    public Task<List<LessonDto>> GetAllLessons()
    {
        return _dbContext.Lessons
            .Where(x => x.DateTime.Year == _clock.Now.Year)
            .OrderByDescending(x => x.DateTime)
            .ProjectToType<LessonDto>()
            .ToListAsync();
    }

    public async Task<CurrentMonthData> GetCurrentMonthIncome()
    {
        var income = await _dbContext.Lessons
            .Where(x => x.DateTime.Year == _clock.Now.Year)
            .Where(x => x.Status == Status.Completed)
            .Where(x => x.DateTime.Month == _clock.Now.Month)
            .SumAsync(x => (double)(x.Price - x.TeacherEarnings));

        return new CurrentMonthData((decimal)income);
    }

    public  Task<List<DailyData>> GetDailyData()
    {
        var list = new List<DailyData>();
        var start = new DateOnly(2024, 07, 15);
        
        for (var i = 0; i < 15; i++)
        {
            var date = start.AddDays(i);
            var completed = Random.Shared.Next(0, 10);
            var canceled = Random.Shared.Next(0, 10);
            
            list.Add(new DailyData
            {
                Date = date,
                Completed = completed,
                Canceled = canceled
            });
        }
        
        return Task.FromResult(list);
    }

    public Task<List<MonthlyData>> GetMonthlyData()
    {
        var montlyDataDict = _dbContext.Lessons
            .Where(x => x.DateTime.Year == _clock.Now.Year)
            .Where(x => x.Status == Status.Completed)
            .GroupBy(x => x.DateTime.Month)
            .Select(x => new
            {
                Month = x.Key,
                Income = x.Sum(y => (double)(y.Price - y.TeacherEarnings))
            })
            .ToDictionary(x => x.Month, x => x.Income);
        
        var res = new List<MonthlyData>();
        
        for(var month = 1; month <= _clock.Now.Month; month++)
        {
            var income = montlyDataDict.GetValueOrDefault(month, 0);
            var date = new DateOnly(_clock.Now.Year, month, 1);
            
            res.Add(new MonthlyData
            {
                Month = date,
                MonthName = date.ToString("MMMM", CultureInfo.InvariantCulture),
                Income = (decimal)income
            });
        }
        
        return Task.FromResult(res);
    }

    public Task<List<StudentData>> GetStudentsList()
    {
        return _dbContext.Students
            .ProjectToType<StudentData>()
            .ToListAsync();
    }

    public Task<List<StudentData>> GetTeachersList()
    {
        return _dbContext.Teachers
            .ProjectToType<StudentData>()
            .ToListAsync();
    }

    public async Task<List<UserMontlyData>> GetStudentMonthlyData(Guid studentId)
    {
        var montlyData = await _dbContext.Lessons
            .Include(x => x.Student)
            .Where(x => x.DateTime.Year == _clock.Now.Year)
            .Where(x => x.Student.Id == studentId)
            .Where(x => x.Status == Status.Completed)
            .GroupBy(x => x.DateTime.Month)
            .Select(x => new UserMontlyData
            {
                MonthName = new DateOnly(_clock.Now.Year, x.Key, 1)
                    .ToString("MMMM", CultureInfo.InvariantCulture),
                Lessons = x.Count(),
                FullIncome = x.Sum(y => (double)y.Price),
                SchoolIncome = x.Sum(y => (double)(y.Price - y.TeacherEarnings)),
                TeacherIncome = x.Sum(y => (double)y.TeacherEarnings)
            })
            .ToListAsync();
        
        return montlyData;
    }

    public async Task<List<UserMontlyData>> GetTeacherMonthlyData(Guid teacherId)
    {
        var montlyData = await _dbContext.Lessons
            .Include(x => x.Teacher)
            .Where(x => x.DateTime.Year == _clock.Now.Year)
            .Where(x => x.Teacher.Id == teacherId)
            .Where(x => x.Status == Status.Completed)
            .GroupBy(x => x.DateTime.Month)
            .Select(x => new UserMontlyData
            {
                MonthName = new DateOnly(_clock.Now.Year, x.Key, 1)
                    .ToString("MMMM", CultureInfo.InvariantCulture),
                Lessons = x.Count(),
                FullIncome = x.Sum(y => (double)y.Price),
                SchoolIncome = x.Sum(y => (double)(y.Price - y.TeacherEarnings)),
                TeacherIncome = x.Sum(y => (double)y.TeacherEarnings)
            })
            .ToListAsync();
        
        return montlyData;
    }
}