using Microsoft.Extensions.Configuration;
using Migration.Tool.CLI;

namespace Migration.Tool.Tests;

public class ConfigurationValidatorTests
{
    [Fact]
    public void GetValidationErrors_WhenCommerceConfigurationSectionDoesNotExist_ShouldNotReturnValidationErrors()
    {
        // Arrange - Create configuration without CommerceConfiguration section
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Settings:KxConnectionString"] = "Server=localhost;Database=Kentico;",
                ["Settings:XbyKDirPath"] = "C:\\XbyK",
                ["Settings:XbyKApiSettings:ConnectionStrings:CMSConnectionString"] = "Server=localhost;Database=XbyK;"
            })
            .Build();

        // Act
        var errors = ConfigurationValidator.GetValidationErrors(configuration).ToList();

        // Assert - Should not contain CommerceConfiguration validation error
        Assert.DoesNotContain(errors, e => e.Message.Contains("CommerceConfiguration"));
    }

    [Fact]
    public void GetValidationErrors_WhenCommerceConfigurationExistsWithEmptySiteNames_ShouldReturnValidationError()
    {
        // Arrange - Create configuration with CommerceConfiguration section containing empty site name
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Settings:KxConnectionString"] = "Server=localhost;Database=Kentico;",
                ["Settings:XbyKDirPath"] = "C:\\XbyK",
                ["Settings:XbyKApiSettings:ConnectionStrings:CMSConnectionString"] = "Server=localhost;Database=XbyK;",
                ["Settings:CommerceConfiguration:CommerceSiteNames:0"] = "" // Empty value
            })
            .Build();

        // Act
        var errors = ConfigurationValidator.GetValidationErrors(configuration).ToList();

        // Assert - Should contain validation error for empty site names
        Assert.Contains(errors, e => e.Message.Contains("CommerceConfiguration:CommerceSiteNames") &&
                                     e.Message.Contains("cannot contain empty or whitespace values"));
    }

    [Fact]
    public void GetValidationErrors_WhenCommerceConfigurationExistsWithValidSiteNames_ShouldNotReturnCommerceValidationErrors()
    {
        // Arrange - Create configuration with valid CommerceConfiguration
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Settings:KxConnectionString"] = "Server=localhost;Database=Kentico;",
                ["Settings:XbyKDirPath"] = "C:\\XbyK",
                ["Settings:XbyKApiSettings:ConnectionStrings:CMSConnectionString"] = "Server=localhost;Database=XbyK;",
                ["Settings:CommerceConfiguration:CommerceSiteNames:0"] = "MySite"
            })
            .Build();

        // Act
        var errors = ConfigurationValidator.GetValidationErrors(configuration).ToList();

        // Assert - Should not contain CommerceConfiguration validation errors
        Assert.DoesNotContain(errors, e => e.Message.Contains("CommerceConfiguration:CommerceSiteNames") &&
                                           e.Message.Contains("must contain at least one site name"));
        Assert.DoesNotContain(errors, e => e.Message.Contains("CommerceConfiguration:CommerceSiteNames") &&
                                           e.Message.Contains("cannot contain empty or whitespace values"));
    }
}
