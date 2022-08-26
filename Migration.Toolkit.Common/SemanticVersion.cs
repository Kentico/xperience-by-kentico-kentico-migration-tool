namespace Migration.Toolkit.Common;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

/// <summary>
/// simplified implementation of semantic version object for this use-case
/// </summary>
/// <param name="Major"></param>
/// <param name="Minor"></param>
/// <param name="Hotfix"></param>
public record SemanticVersion(int Major, int? Minor, int? Hotfix) //, string? Build
{
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(Major);
        if (!Minor.HasValue) return sb.ToString();
        sb.Append($".{Minor}");
        if (!Hotfix.HasValue) return sb.ToString();
        sb.Append($".{Hotfix}");
        return sb.ToString();
    }

    public static bool TryParse(string? s, [NotNullWhen(true)] out SemanticVersion? semVer)
    {
        semVer = null;
        if (!string.IsNullOrWhiteSpace(s))
        {
            var split = s.Split(".");
            switch (split.Length)
            {
                case 1 when int.TryParse(split[0], out var major):
                {
                    semVer = new(major, null, null);
                    return true;
                }
                case 2 when int.TryParse(split[0], out var major) && int.TryParse(split[1], out var minor):
                {
                    semVer = new(major, minor, null);
                    return true;
                } 
                case 3 when int.TryParse(split[0], out var major) && int.TryParse(split[1], out var minor) && int.TryParse(split[2], out var hotfix):
                {
                    semVer = new(major, minor, hotfix);
                    return true;
                }
            }
        }

        return false;
    }

    public bool IsLesserThan(SemanticVersion ver, bool compareOnlySpecified = true)
    {
        var (major, minor, hotfix) = ver;
        if (compareOnlySpecified)
        {
            if (Major - major < 0)
            {
                return true;
            }

            if (minor.HasValue && Minor.HasValue && Minor - minor < 0)
            {
                return true;
            }
            
            if (hotfix.HasValue && Hotfix.HasValue && Hotfix - hotfix < 0)
            {
                return true;
            }
        }
        else
        {
            if (Major - major < 0)
            {
                return true;
            }

            if (Minor.GetValueOrDefault(0) - minor.GetValueOrDefault(0) < 0)
            {
                return true;
            }
            
            if (Hotfix.GetValueOrDefault(0) - hotfix.GetValueOrDefault(0) < 0)
            {
                return true;
            }
        }

        return false;
    }
}