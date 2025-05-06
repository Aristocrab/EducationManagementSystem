using EducationManagementSystem.Application.Database;
using EducationManagementSystem.Application.Extensions;
using EducationManagementSystem.Application.Features.Certificates.Dtos;
using EducationManagementSystem.Application.Features.Certificates.Helpers;
using EducationManagementSystem.Application.Shared.Auth.Models;
using EducationManagementSystem.Core.Exceptions;
using EducationManagementSystem.Core.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Throw;

namespace EducationManagementSystem.Application.Features.Certificates;

public class CertificatesService : ICertificatesService
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<NewCertificateDto> _validator;
    private readonly ICertificateParserResolver _parserResolver;

    public CertificatesService(AppDbContext dbContext, IValidator<NewCertificateDto> validator, ICertificateParserResolver parserResolver)
    {
        _dbContext = dbContext;
        _validator = validator;
        _parserResolver = parserResolver;
    }

    public async Task<IReadOnlyList<CertificateDto>> GetAll(User currentUser)
    {
        return await _dbContext.Certificates
            .Include(c => c.Student)
            .Include(c => c.Subject)
            .Select(c => new CertificateDto
            {
                Id = c.Id,
                StudentName = c.Student.FullName,
                SubjectTitle = c.Subject.Title,
                CourseTitle = c.CourseTitle,
                CourseGrade = c.CourseGrade,
                Issuer = c.Issuer,
                Link = c.Link,
                IssuedAt = c.IssuedAt
            })
            .ToListAsync();
    }

    public async Task<CertificateDto> GetById(Guid id)
    {
        var cert = await _dbContext.Certificates
            .Include(c => c.Student)
            .Include(c => c.Subject)
            .FirstOrDefaultAsync(c => c.Id == id);

        cert.ThrowIfNull(_ => new NotFoundException("Certificate not found"));

        return new CertificateDto
        {
            Id = cert.Id,
            StudentName = cert.Student.FullName,
            SubjectTitle = cert.Subject.Title,
            CourseTitle = cert.CourseTitle,
            CourseGrade = cert.CourseGrade,
            Issuer = cert.Issuer,
            Link = cert.Link,
            IssuedAt = cert.IssuedAt
        };
    }

    public async Task Add(NewCertificateDto dto, User currentUser)
    {
        await _validator.ValidateAndThrowAsync(dto);
        currentUser.Throw().IfNotAdminOrModerator();

        var student = await _dbContext.Students
            .Include(s => s.Subjects).Include(student => student.Grades)
            .FirstOrDefaultAsync(x => x.Id == dto.StudentId);
        student.ThrowIfNull(_ => new NotFoundException("Student not found"));

        var subject = await _dbContext.Subjects.FirstOrDefaultAsync(x => x.Id == dto.SubjectId);
        subject.ThrowIfNull(_ => new NotFoundException("Subject not found"));

        var parser = _parserResolver.Resolve(dto.Link);
        var parsed = await parser.ParseAsync(dto.Link);

        var certificate = new Certificate
        {
            Student = student,
            Subject = subject,
            CourseTitle = parsed.CourseTitle,
            CourseGrade = parsed.CourseGrade ?? 100,
            Issuer = parsed.Issuer,
            Link = dto.Link,
            IssuedAt = parsed.IssuedAt
        };

        await _dbContext.Certificates.AddAsync(certificate);

        var studentCourse = student.Grades.FirstOrDefault(c => c.Id == subject.Id);
        if (studentCourse != null)
        {
            var maxGrade = subject.CertificateMaxGrade;
            studentCourse.Value = Math.Round(certificate.CourseGrade / 100 * maxGrade);
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(Guid id, User currentUser)
    {
        currentUser.Throw().IfNotAdminOrModerator();

        var cert = await _dbContext.Certificates.FirstOrDefaultAsync(c => c.Id == id);
        cert.ThrowIfNull(_ => new NotFoundException("Certificate not found"));

        _dbContext.Certificates.Remove(cert);
        await _dbContext.SaveChangesAsync();
    }
}