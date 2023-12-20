namespace Migration.Toolkit.Common.MigrationProtocol;

using System.Diagnostics;
using MediatR;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.Services;

public class TextMigrationProtocol: IMigrationProtocol, IDisposable
{
    private readonly ToolkitConfiguration _configuration;
    private readonly IPrintService _printService;
    private readonly StreamWriter _streamWriter;

    public TextMigrationProtocol(ToolkitConfiguration configuration, IPrintService printService)
    {
        _configuration = configuration;
        _printService = printService;

        var nowStartDate = DateTime.Now;
        if (string.IsNullOrWhiteSpace(configuration.MigrationProtocolPath) && Process.GetCurrentProcess().MainModule?.FileName is {} programPath)
        {
            var processDir = Path.GetDirectoryName(programPath);
            if (processDir != null)
                _streamWriter = new StreamWriter(Path.Combine(processDir, $"MigrationProtocol_{nowStartDate:yyyyMMdd_hhmm}.html"));
        }
        else
        {
            var directoryName = Path.GetDirectoryName(configuration.MigrationProtocolPath);
            if (directoryName != null)
            {
                Directory.CreateDirectory(directoryName);
                var nameWithoutExtension = Path.GetFileNameWithoutExtension(configuration.MigrationProtocolPath);
                var extension = Path.GetExtension(configuration.MigrationProtocolPath);
                _streamWriter = new StreamWriter(Path.Combine(directoryName, $"{nameWithoutExtension}{nowStartDate:yyyyMMdd_hhmm}{extension}"));
            }
        }

        if (_streamWriter == null)
        {
            throw new InvalidOperationException("Unable to get path for migration protocol initialization - configure settings in path 'Settings.MigrationProtocolPath'");
        }

        _streamWriter.AutoFlush = true;
    }

    private void WriteLine(string line)
    {
        _streamWriter.WriteLine($"{DateTime.Now:yyyyMMdd_hhmmss}: {line}");
    }

    public void MappedTarget<TTarget>(IModelMappingResult<TTarget> mapped)
    {

    }

    public void FetchedTarget<TTarget>(TTarget? target)
    {

    }

    public void FetchedSource<TSource>(TSource? source)
    {

    }

    public void Success<TSource, TTarget>(TSource source, TTarget target, IModelMappingResult<TTarget>? mapped)
    {
        WriteLine($"Success: {_printService.GetEntityIdentityPrint(target)}");
    }

    public void Warning<T>(HandbookReference handbookRef, T? entity)
    {
        WriteLine($"{handbookRef}");
    }

    public void Warning<TSource, TTarget>(HandbookReference handbookRef, TSource? source, TTarget? target)
    {
        WriteLine($"{handbookRef}");
    }

    public void CommandRequest<TRequest, TResponse>(TRequest request) where TRequest : IRequest<TResponse>
    {
        WriteLine($"Command {request} requested");
    }

    public void CommandFinished<TRequest, TResponse>(TRequest request, TResponse response) where TRequest : IRequest<TResponse> where TResponse : CommandResult
    {
        WriteLine($"Command {request} successfully finished");
    }

    public void CommandError<TRequest, TResponse>(Exception exception, TRequest request) where TRequest : IRequest<TResponse>
    {
        WriteLine($"Command {request} error: {exception}");
    }

    public void Append(HandbookReference? handbookReference)
    {
        WriteLine($"{handbookReference}");
    }

    public void Dispose()
    {
        _streamWriter.Dispose();
    }
}