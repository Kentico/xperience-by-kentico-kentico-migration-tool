using CMS.Base;

namespace Migration.Toolkit.KXP.Api.Auxiliary;

public class DummyUploadedFile : IUploadedFile
{
    private DummyUploadedFile(Stream stream, string contentType, long length, string fileName)
    {
        InputStream = stream;
        FileName = fileName;
        Length = length;
        ContentType = contentType;
    }

    public Stream InputStream { get; }

    public string ContentType { get; }

    public long Length { get; }

    public string FileName { get; }

    public Stream OpenReadStream() => InputStream;

    public bool CanOpenReadStream => InputStream.CanRead;

    public static IUploadedFile FromStream(Stream stream, string contentType, long length, string fileName) => new DummyUploadedFile(stream, contentType, length, fileName);

    public static IUploadedFile FromByteArray(byte[] byteArray, string contentType, long length, string fileName) => new DummyUploadedFile(new MemoryStream(byteArray), contentType, length, fileName);
}
