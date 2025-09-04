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

    private static readonly string suffixPool = "7iwo0xo9g3ozgbgz9bm6xu3ngot1axv32hn1x09fczzdkaraaord9ij53mf1v5gxvmtlolar1ivv" +
        "tml615ccfrhmzdm1t3lma5st4utfhtxaiz0t0c5k2jkwond3oqrbtxnxc91xkisdas6lf8lkdpr1to9p30b5i034kvkz2llp523txae9xp7ouak574mm" +
        "no5sx0qu2dwd3vhj6hfrmg56yqebhdiyvy0b4k7tchwdx4g9h4nxzxht4r0jzyafufynpq5a9yn17ha7un7cpaq9yiia7nr1qdwab966199deu0b60dc" +
        "9uc8negpap5fxt5w6yt99z0ghv4vhy0fg7uxp2nazn0ytww9jc86egdezp83i7i8y9ejm9r79p37w4zpzdp708b501u0a60m4yhfyaghu5xdkbexd8gf" +
        "7m4qkhrkkf5c2fuka7cv6d6ekg2p4wfzf7tj5cv39u620wchhybydyqwaky0fv9yk6zvw9zi30zvnx6hm12phexx8tb5c3qx3g5nuqv0gmv0hytyhhvc" +
        "7j8iq2ez4jfx439qrf6337gy12rkx0vndkzw3wz4argiim6q8n572bcg8gwpqfyffmmgyrb2g4x5gj2gvwqctutfjntp9pmkjn3ff5wk8u05b28m9f0z" +
        "vvyh2wpiiqnamfw3d3iyv23cda7pte12bgu9yq4y42nmvvmp5dq6vtfz9ema4rqyr5ak014z7e2hpmhj9d7jj345khib1v0j1iph8xvhybibdiihfqnt" +
        "ku9789pnfqn37p0kgbwig5q4fhpf";

    public const int SuffixLength = 4;
    public static int UniqueSuffixCount => suffixPool.Length / SuffixLength;
}
