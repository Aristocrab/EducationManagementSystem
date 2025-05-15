using EducationManagementSystem.Application.Features.Certificates;
using EducationManagementSystem.Application.Features.Certificates.Dtos;
using EducationManagementSystem.WebApi.Controllers.Shared;
using Microsoft.AspNetCore.Mvc;

namespace EducationManagementSystem.WebApi.Controllers;

[Route("api/certificates")]
public class CertificatesController : BaseController
{
    private readonly ICertificatesService _certificatesService;
    public CertificatesController(ICertificatesService certificatesService) => _certificatesService = certificatesService;
    
    [HttpGet]
    public async Task<IReadOnlyList<CertificateDto>> GetAll() => await _certificatesService.GetAll(CurrentUser);
    
    [HttpGet("{id}")]
    public async Task<CertificateDto> GetById(Guid id) => await _certificatesService.GetById(id);
    
    [HttpPost]
    public async Task UploadCertificate(NewCertificateDto dto) => await _certificatesService.UploadCertificate(dto, CurrentUser);
    
    [HttpPost("addAllowed")]
    public async Task AddAllowedCertificate(AllowedCertificateDto dto) => await _certificatesService.AddAllowedCertificate(dto, CurrentUser);
    
    [HttpDelete("{id}")]
    public async Task Delete(Guid id) => await _certificatesService.Delete(id, CurrentUser);
}