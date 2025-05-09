using EducationManagementSystem.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace EducationManagementSystem.Application.Database;

public sealed class AppDbContext : DbContext
{
    public DbSet<Teacher> Teachers { get; init; }
    public DbSet<Lesson> Lessons { get; init; }
    public DbSet<School> Schools { get; init; }
    public DbSet<Student> Students { get; init; }
    public DbSet<Group> Groups { get; init; }
    public DbSet<Subject> Subjects { get; init; }
    public DbSet<SubjectGrade> SubjectGrades { get; init; }
    public DbSet<Attestation> Attestations { get; init; }
    public DbSet<Certificate> Certificates { get; init; }
    public DbSet<Session> Sessions { get; init; }
    public DbSet<AllowedCertificate> AllowedCertificates { get; init; }
    public DbSet<SelectedSubjectGroup> SelectedSubjectGroups { get; init; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}