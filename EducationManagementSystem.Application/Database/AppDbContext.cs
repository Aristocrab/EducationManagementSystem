using EducationManagementSystem.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace EducationManagementSystem.Application.Database;

public sealed class AppDbContext : DbContext
{
    public DbSet<Teacher> Teachers { get; init; }
    public DbSet<Lesson> Lessons { get; init; }
    public DbSet<School> Schools { get; init; }
    public DbSet<Student> Students { get; init; }
    public DbSet<Payment> Payments { get; init; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}