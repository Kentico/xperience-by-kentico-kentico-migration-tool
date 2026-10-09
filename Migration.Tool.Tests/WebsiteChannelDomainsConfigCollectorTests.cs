using System.Text.Json;

using Migration.Tool.Common.Helpers;

namespace Migration.Tool.Tests;

public class WebsiteChannelDomainsConfigCollectorTests
{
    [Fact]
    public void EmptyCollector_HasAny_IsFalse()
    {
        var collector = new WebsiteChannelDomainsConfigCollector();

        Assert.False(collector.HasAny);
    }

    [Fact]
    public void LanguageDomains_AreGroupedByChannelAndLanguage_AndKeepOrder()
    {
        var collector = new WebsiteChannelDomainsConfigCollector();
        collector.AddLanguageDomain("DancingGoat", "en-US", "dg.com");
        collector.AddLanguageDomain("DancingGoat", "fr-FR", "fr.dg.com");
        collector.AddLanguageDomain("DancingGoat", "cs-CZ", "www.dg.cz");
        collector.AddLanguageDomain("DancingGoat", "cs-CZ", "dg.cz");
        // duplicate (case-insensitive) is ignored
        collector.AddLanguageDomain("DancingGoat", "cs-CZ", "DG.CZ");

        var root = JsonDocument.Parse(collector.BuildJson()).RootElement;
        var domains = root.GetProperty("WebsiteChannelDomains").GetProperty("LanguageDomains").GetProperty("DancingGoat").GetProperty("Domains");

        Assert.Equal("dg.com", domains.GetProperty("en-US")[0].GetString());
        Assert.Equal("fr.dg.com", domains.GetProperty("fr-FR")[0].GetString());
        Assert.Equal(["www.dg.cz", "dg.cz"], domains.GetProperty("cs-CZ").EnumerateArray().Select(x => x.GetString()).ToArray());
    }

    [Fact]
    public void DomainOverrides_AreEmittedAsSeparateSection()
    {
        var collector = new WebsiteChannelDomainsConfigCollector();
        collector.AddDomainOverride("Site1", "site1.com");
        collector.AddDomainOverride("Site1", "www.site1.com");

        var root = JsonDocument.Parse(collector.BuildJson()).RootElement;
        var websiteChannelDomains = root.GetProperty("WebsiteChannelDomains");

        Assert.Equal(["site1.com", "www.site1.com"],
            websiteChannelDomains.GetProperty("DomainOverrides").GetProperty("Site1").GetProperty("Domains").EnumerateArray().Select(x => x.GetString()).ToArray());
        Assert.False(websiteChannelDomains.TryGetProperty("LanguageDomains", out _));
    }
}

public class UriHelperTryNormalizeDomainTests
{
    [Theory]
    [InlineData("fr.example.com", "fr.example.com")]
    [InlineData("FR.EXAMPLE.COM/", "fr.example.com")]
    [InlineData("https://example.cz", "example.cz")]
    [InlineData("http://example.com:8080", "example.com:8080")]
    [InlineData("example.com/shop", "example.com/shop")]
    [InlineData(" example.com ", "example.com")]
    public void ValidDomains_AreNormalized(string input, string expected)
    {
        Assert.True(UriHelper.TryNormalizeDomain(input, out string normalized));
        Assert.Equal(expected, normalized);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void InvalidDomains_ReturnFalse(string? input) => Assert.False(UriHelper.TryNormalizeDomain(input, out _));
}
