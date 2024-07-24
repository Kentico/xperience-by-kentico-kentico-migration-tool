using System.Diagnostics;

using MediatR;

using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.Services;

namespace Migration.Toolkit.Common.MigrationProtocol;

public class TextMigrationProtocol : IMigrationProtocol, IDisposable
{
    private readonly IPrintService printService;
    private readonly StreamWriter streamWriter;

    public TextMigrationProtocol(ToolkitConfiguration configuration, IPrintService printService)
    {
        this.printService = printService;

        var nowStartDate = DateTime.Now;
        if (string.IsNullOrWhiteSpace(configuration.MigrationProtocolPath) && Process.GetCurrentProcess().MainModule?.FileName is { } programPath)
        {
            string? processDir = Path.GetDirectoryName(programPath);
            if (processDir != null)
            {
                streamWriter = new StreamWriter(Path.Combine(processDir, $"MigrationProtocol_{nowStartDate:yyyyMMdd_hhmm}.html"));
            }
        }
        else
        {
            string? directoryName = Path.GetDirectoryName(configuration.MigrationProtocolPath);
            if (directoryName != null)
            {
                Directory.CreateDirectory(directoryName);
                string? nameWithoutExtension = Path.GetFileNameWithoutExtension(configuration.MigrationProtocolPath);
                string? extension = Path.GetExtension(configuration.MigrationProtocolPath);
                streamWriter = new StreamWriter(Path.Combine(directoryName, $"{nameWithoutExtension}{nowStartDate:yyyyMMdd_hhmm}{extension}"));
            }
        }

        if (streamWriter == null)
        {
            throw new InvalidOperationException("Unable to get path for migration protocol initialization - configure settings in path 'Settings.MigrationProtocolPath'");
        }

        streamWriter.AutoFlush = true;
    }

    public void Dispose() => streamWriter.Dispose();

    public void MappedTarget<TTarget>(IModelMappingResult<TTarget> mapped)
    {
    }

    public void FetchedTarget<TTarget>(TTarget? target)
    {
    }

    public void FetchedSource<TSource>(TSource? source)
    {
    }

    public void Success<TSource, TTarget>(TSource source, TTarget target, IModelMappingResult<TTarget>? mapped) => WriteLine($"Success: {printService.GetEntityIdentityPrint(target)}");

    public void Warning<T>(HandbookReference handbookRef, T? entity) => WriteLine($"{handbookRef}");

    public void CommandRequest<TRequest, TResponse>(TRequest request) where TRequest : IRequest<TResponse> => WriteLine($"Command {request} requested");

    public void CommandFinished<TRequest, TResponse>(TRequest request, TResponse response) where TRequest : IRequest<TResponse> where TResponse : CommandResult => WriteLine($"Command {request} successfully finished");

    public void CommandError<TRequest, TResponse>(Exception exception, TRequest request) where TRequest : IRequest<TResponse> => WriteLine($"Command {request} error: {exception}");

    public void Append(HandbookReference? handbookReference) => WriteLine($"{handbookReference}");

    private void WriteLine(string line) => streamWriter.WriteLine($"{DateTime.Now:yyyyMMdd_hhmmss}: {line}");
}
