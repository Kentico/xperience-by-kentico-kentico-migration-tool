namespace Migration.Toolkit.Common.MigrationProtocol;

using System.Diagnostics;
using System.Net;
using MediatR;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.Helpers;

public class MigrationProtocolInHtml: IMigrationProtocol, IDisposable
{
    private readonly ToolkitConfiguration _configuration;
    private readonly StreamWriter _streamWriter;

    public MigrationProtocolInHtml(ToolkitConfiguration configuration)
    {
        _configuration = configuration;

        var nowStartDate = DateTime.Now; 
        var processDir = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        _streamWriter = new StreamWriter(Path.Combine(processDir, $"MigrationProtocol_{nowStartDate:yyyyMMdd_hhmm}.html"));
        var head = @"
<link rel=""stylesheet"" href=""https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css"" integrity=""sha384-1BmE4kWBq78iYhFldvKuhfTAU6auU8tT94WrHftjDbrCEXSU1oBoqyl2QvZ6jIW3"" crossorigin=""anonymous"">
<script src=""https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/js/bootstrap.bundle.min.js"" integrity=""sha384-MrcW6ZMFYlzcLA8Nl+NtUVF0sA7MsXsP1UyJoMp4YLEuNSfAP+JcXn/tWtIaxVXM"" crossorigin=""anonymous""></script>

<style>
    pre {
      width: 500px;
      overflow-x: auto;
    }
</style>
";
        _streamWriter.WriteLine($"<html><head>{head}</head><body><h1>Migration protocol {nowStartDate}</h1>");
        _streamWriter.AutoFlush = true;
    }

    private string ToJsonEscaped<T>(T obj)
    {
        var serialized = WebUtility.HtmlEncode(SerializationHelper.SerializeOnlyNonComplexProperties(obj));
        var id = $"d{Guid.NewGuid()}";
        return @$"<a data-bs-toggle=""collapse"" href=""#{id}"" role=""button"" aria-expanded=""false"" aria-controls=""{id}"">data</a><pre id={id} class=""collapse"">{serialized}</pre>";
    }

    private string GetTimeStamp() => $"<td>{DateTime.Now}</td>";
    
    public void CommandRequest<TRequest, TResponse>(TRequest request) where TRequest : IRequest<TResponse>
    {
        var monikerFriendly = ReflectionHelper<TRequest>.GetStaticPropertyValue("MonikerFriendly")?.ToString();
        var moniker = ReflectionHelper<TRequest>.GetStaticPropertyValue("Moniker")?.ToString();
        _streamWriter.WriteLine(@$"<h2><a data-bs-toggle=""collapse"" href=""#{moniker}"" role=""button"" aria-expanded=""false"" aria-controls=""{moniker}"">Command {monikerFriendly} run</a></h2>");
        _streamWriter.WriteLine($"<table id={moniker} class=\"collapse table\">");
    }

    public void CommandFinished<TRequest, TResponse>(TRequest request, TResponse response) where TRequest : IRequest<TResponse> where TResponse : CommandResult
    {
        _streamWriter.WriteLine($"</table>");
    }

    public void CommandError<TRequest, TResponse>(Exception exception, TRequest request) where TRequest : IRequest<TResponse>
    {
        _streamWriter.WriteLine($"<tr>{GetTimeStamp()}<td>Command error</td><td></td><td><pre>{WebUtility.HtmlEncode(exception.ToString())}</pre></td></tr>");
        _streamWriter.WriteLine($"</table>");
    }

    public void Append(HandbookReference? handbookReference)
    {
        ArgumentNullException.ThrowIfNull(handbookReference);
        
        _streamWriter.WriteLine($"<tr>{GetTimeStamp()}<td>Manual action needed</td><td>{handbookReference.ReferenceName}</td><td>{ToJsonEscaped(handbookReference.Data)}</td><td></td></tr>");
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
        // _streamWriter.WriteLine($"<tr>{GetTimeStamp()}<td>Manual action needed</td><td>Success</td><td>{ToJsonEscaped(new {source=source, target=target})}</td></tr>");
        // _streamWriter.WriteLine($"<tr>{GetTimeStamp()}<td>Success</td><td></td><td></td></tr>");
    }

    public void Warning<T>(HandbookReference handbookRef, T? entity)
    {
        _streamWriter.WriteLine($"<tr>{GetTimeStamp()}<td>Warning</td><td>{handbookRef}</td><td>{ToJsonEscaped(entity)}</td></tr>");
    }

    public void Dispose()
    {
        _streamWriter.WriteLine("</body></html>");
        _streamWriter.Dispose();
    }
}