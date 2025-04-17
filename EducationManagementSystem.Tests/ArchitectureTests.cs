using EducationManagementSystem.Application.Database;
using EducationManagementSystem.Core.Models.Base;
using FluentAssertions;
using NetArchTest.Rules;

namespace EducationManagementSystem.Tests;

public class ArchitectureTests
{
    [Fact]
    public void DomainLayer_ShouldNotReferenceApplicationLayer()
    {
        var result = Types.InAssembly(typeof(Entity).Assembly)
            .Should()
            .NotHaveDependencyOnAll("KoineCrm.Application", "KoineCrm.WebApi")
            .GetResult();
        
        result.IsSuccessful.Should().BeTrue();
    }
    
    [Fact]
    public void ApplicationLayer_ShouldNotReferenceWebApiLayer()
    {
        var result = Types.InAssembly(typeof(AppDbContext).Assembly)
            .Should()
            .NotHaveDependencyOn("KoineCrm.WebApi")
            .GetResult();
        
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void TypesThatInheritFromEntity_ShouldBeSealed()
    {
        var result = Types.InAssembly(typeof(Entity).Assembly)
            .That()
            .Inherit(typeof(Entity))
            .Should()
            .BeSealed()
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}