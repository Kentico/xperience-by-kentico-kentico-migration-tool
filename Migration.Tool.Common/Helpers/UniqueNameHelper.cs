namespace Migration.Tool.Common.Helpers;
public static class UniqueNameHelper
{
    /// <summary>
    /// Makes name unique by trying to append pseudorandom suffixes. Candidates are offered to 
    /// <paramref name="availabilityChecker"/>, which determines, whether the candidate can be used
    /// </summary>
    public static string MakeUnique(string name, Func<string, bool> availabilityChecker)
    {
        string uniqueCodeName = name;
        int attemptIndex = 0;
        while (attemptIndex < UniqueSuffixCount && !availabilityChecker(uniqueCodeName)) // While conflict, try new GUID suffix to make unique
        {
            uniqueCodeName = $"{name}-{GetSuffix(attemptIndex++)}";
        }
        if (attemptIndex >= UniqueSuffixCount)
        {
            throw new Exception("Unable to obtain unique name");
        }
        return uniqueCodeName;
    }

    /// <summary>
    /// Returns pseudorandom string of <see cref="SuffixLength"/> characters. 
    /// Seed can run from 0 to <see cref="UniqueSuffixCount"/>-1, before the suffixes
    /// start repeating.
    /// </summary>
    /// <param name="seed"></param>
    /// <returns></returns>
    public static string GetSuffix(int seed) => suffixPool.Substring(seed % UniqueSuffixCount * SuffixLength, SuffixLength);

    private static readonly string suffixPool = "7iwo0xo9g3ozgbgz9bm6xu3ngot1axv32hn1x09fczzdkaraaord9ij53mf1v5gxvmtlolar1ivvtml615ccfrhmzdm1t3lma5st4utfhtxaiz0t0c5k2jkwond3oqrbtxnxc91xkisdas6lf8lkdpr1to9p30b5i034kvkz2llp523txae9xp7ouak574mmno5sx0qu";
    public const int SuffixLength = 4;
    public static int UniqueSuffixCount => suffixPool.Length / SuffixLength;
}
