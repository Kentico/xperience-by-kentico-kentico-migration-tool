using System.Security.Cryptography;
using System.Text;

namespace Migration.Tool.Common.Helpers;

public static class GuidV5
{
    private static readonly UuidV5Generator v5Generator = new();
    public static Guid NewNameBased(Guid namespaceId, string name) => v5Generator.New(namespaceId, name);
}

internal class UuidV5Generator
{
    private ThreadLocal<HashAlgorithm?> HashAlgorithm { get; } = new(SHA1.Create);

    public static bool TryWriteBytes(Guid guid, Span<byte> bytes, bool bigEndian, out int bytesWritten)
    {
        if (bytes.Length < 16 || !guid.TryWriteBytes(bytes))
        {
            bytesWritten = 0;
            return false;
        }

        if (bigEndian)
        {
            Permut(bytes, 0, 3);
            Permut(bytes, 1, 2);
            Permut(bytes, 5, 4);
            Permut(bytes, 6, 7);
        }

        bytesWritten = 16;
        return true;
    }


    private static void Permut(Span<byte> array, int indexSource, int indexDest) => (array[indexSource], array[indexDest]) = (array[indexDest], array[indexSource]);


    public Guid New(Guid namespaceId, string name)
    {
        int utf8NameByteCount = Encoding.UTF8.GetByteCount(name.Normalize(NormalizationForm.FormD));
        var utf8NameBytes = utf8NameByteCount > 256 ? new byte[utf8NameByteCount] : stackalloc byte[utf8NameByteCount];
        Encoding.UTF8.GetBytes(name, utf8NameBytes);
        Span<byte> namespaceBytes = stackalloc byte[16];
        TryWriteBytes(namespaceId, namespaceBytes, true, out _);
        int bytesToHashCount = namespaceBytes.Length + utf8NameBytes.Length;
        var bytesToHash = utf8NameByteCount > 256 ? new byte[bytesToHashCount] : stackalloc byte[bytesToHashCount];
        namespaceBytes.CopyTo(bytesToHash);
        utf8NameBytes.CopyTo(bytesToHash[namespaceBytes.Length..]);

        var hashAlgorithm = HashAlgorithm.Value!;
        Span<byte> hash = stackalloc byte[hashAlgorithm.HashSize / 8];
        hashAlgorithm.TryComputeHash(bytesToHash, hash, out int _);

        var bigEndianBytes = hash[..16];
        const int versionByte = 6;
        bigEndianBytes[versionByte] &= 0b0000_1111;
        bigEndianBytes[versionByte] |= 5 << 4;
        const int variantByte = 8;
        bigEndianBytes[variantByte] &= 0b0011_1111;
        bigEndianBytes[variantByte] |= 0b1000_0000;

        Span<byte> localBytes = stackalloc byte[bigEndianBytes.Length];
        bigEndianBytes.CopyTo(localBytes);
        Permut(localBytes, 0, 3);
        Permut(localBytes, 1, 2);
        Permut(localBytes, 5, 4);
        Permut(localBytes, 6, 7);

        return new Guid(localBytes);
    }
}
