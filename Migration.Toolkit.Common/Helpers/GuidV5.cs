
using System.Security.Cryptography;
using System.Text;

namespace Migration.Toolkit.Common.Helpers;
public static class GuidV5
{
    private static readonly UuidV5Generator V5Generator = new();
    public static Guid NewNameBased(Guid namespaceId, string name) => V5Generator.New(namespaceId, name);
}

internal class UuidV5Generator
{
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
        TryWriteBytes(namespaceId, namespaceBytes, bigEndian: true, out _);
        int bytesToHashCount = namespaceBytes.Length + utf8NameBytes.Length;
        var bytesToHash = utf8NameByteCount > 256 ? new byte[bytesToHashCount] : stackalloc byte[bytesToHashCount];
        namespaceBytes.CopyTo(bytesToHash);
        utf8NameBytes.CopyTo(bytesToHash[namespaceBytes.Length..]);

        var hashAlgorithm = HashAlgorithm.Value!;
        Span<byte> hash = stackalloc byte[hashAlgorithm.HashSize / 8];
        hashAlgorithm.TryComputeHash(bytesToHash, hash, out int _);

        var bigEndianBytes = hash[..16];
        const int VERSION_BYTE = 6;
        bigEndianBytes[VERSION_BYTE] &= 0b0000_1111;
        bigEndianBytes[VERSION_BYTE] |= 5 << 4;
        const int VARIANT_BYTE = 8;
        bigEndianBytes[VARIANT_BYTE] &= 0b0011_1111;
        bigEndianBytes[VARIANT_BYTE] |= 0b1000_0000;

        Span<byte> localBytes = stackalloc byte[bigEndianBytes.Length];
        bigEndianBytes.CopyTo(localBytes);
        Permut(localBytes, 0, 3);
        Permut(localBytes, 1, 2);
        Permut(localBytes, 5, 4);
        Permut(localBytes, 6, 7);

        return new Guid(localBytes);
    }

    private ThreadLocal<HashAlgorithm?> HashAlgorithm { get; } = new(SHA1.Create);
}
