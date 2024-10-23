using System.Diagnostics.CodeAnalysis;
using System.Text;

using Kentico.Xperience.UMT.Services;

using Microsoft.Extensions.Logging;

namespace Migration.Tool.Source;

public static class Extensions
{
    public static async Task<AssertSuccessResult<TResult>> AssertSuccess<TResult>(this Task<IImportResult> resultTask, ILogger logger)
    {
        switch (await resultTask)
        {
            case { Success: true, Imported: TResult info }:
            {
                return new AssertSuccessResult<TResult>(true, info);
            }
            case { } result:
            {
                var sb = new StringBuilder();
                if (result.ModelValidationResults is { } validationResults)
                {
                    validationResults.ForEach(vr => sb.Append($"[{string.Join(",", vr.MemberNames)}]: {vr.ErrorMessage}"));
                }

                if (result.Exception is { } exception)
                {
                    logger.LogError(exception, "Error occured while importing entity {ValidationErrors}", sb);
                }

                return new AssertSuccessResult<TResult>(false, default);
            }
            default:
                throw new NotImplementedException("Undefined state");
        }
    }

    public record AssertSuccessResult<TResult>(
        [property: MemberNotNullWhen(true, "Info")] bool Success,
        TResult? Info
    );
}
