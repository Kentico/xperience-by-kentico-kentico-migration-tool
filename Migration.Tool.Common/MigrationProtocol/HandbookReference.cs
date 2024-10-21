using System.Reflection;
using System.Text;

using Migration.Tool.Common.Services;

namespace Migration.Tool.Common.MigrationProtocol;

public class HandbookReference
{
    public static IPrintService PrintService = default;

    /// <summary>
    ///     Use <see cref="HandbookReferences" /> class to store factory methods, don't create instances directly in code.
    /// </summary>
    /// <param name="referenceName">
    ///     Use common identifier characters to describe handbook reference (consider usage in HTML,
    ///     JSON, DB, C#, href attribute in HTML)
    /// </param>
    public HandbookReference(string referenceName, string? additionalInfo = null)
    {
        ReferenceName = referenceName;
        AdditionalInfo = additionalInfo;
    }

    public bool NeedManualAction { get; private set; }
    public string ReferenceName { get; }
    public string? AdditionalInfo { get; }
    public Dictionary<string, object?>? Data { get; private set; }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"NeedManualAction: {NeedManualAction}, ReferenceName: {ReferenceName}, AdditionalInfo: {AdditionalInfo}");
        if (Data == null)
        {
            return sb.ToString();
        }

        var arr = Data.ToArray();
        for (int i = 0; i < arr.Length; i++)
        {
            (string key, object? value) = arr[i];
            sb.Append($"{key}: {value}");

            if (i < arr.Length - 1)
            {
                sb.Append(", ");
            }
        }

        return sb.ToString();
    }

    /// <summary>
    ///     Related ID of data, specify if possible
    /// </summary>
    public HandbookReference WithId(string idName, object idValue)
    {
        WithData(idName, idValue);
        return this;
    }

    /// <summary>
    ///     Appends message for user to see, use it for describing issue and propose fix if possible
    /// </summary>
    public HandbookReference WithMessage(string message)
    {
        Data ??= [];

        int msgNum = 0;
        string msgKey;
        do
        {
            msgKey = $"Message#{msgNum++}";
        } while (Data.ContainsKey(msgKey));

        WithData(msgKey, message);

        return this;
    }

    /// <summary>
    ///     Appends data to dictionary for user to see
    /// </summary>
    public HandbookReference WithData(string key, object value)
    {
        Data ??= [];
        Data.Add(key, value);

        return this;
    }

    /// <summary>
    ///     Appends data to dictionary for user to see
    /// </summary>
    public HandbookReference WithData<TValue>(Dictionary<string, TValue> data)
    {
        Data ??= [];
        foreach ((string key, var value) in data)
        {
            Data.Add(key, value);
        }

        return this;
    }

    /// <summary>
    ///     Appends data to dictionary for user to see
    /// </summary>
    /// <param name="data">
    ///     All public instance properties of object are written to dictionary for user to see. Anonymous object
    ///     can be used
    /// </param>
    public HandbookReference WithData(object data)
    {
        Data ??= [];
        var dataUpdate = data.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .ToDictionary(p => p.Name, p => p.GetMethod.Invoke(data, Array.Empty<object>())
            );
        foreach ((string key, object? value) in dataUpdate)
        {
            Data.Add(key, value);
        }

        return this;
    }

    /// <summary>
    ///     Prints entity information. Type must be supported for print in method 'GetEntityIdentityPrint' in class
    ///     <see cref="Printer" />
    /// </summary>
    /// <param name="model">Models to print</param>
    /// <typeparam name="T">
    ///     Type of model to print - type must be supported for print in method 'GetEntityIdentityPrint' in
    ///     class <see cref="Printer" />
    /// </typeparam>
    public HandbookReference WithIdentityPrint<T>(T model)
    {
        Data ??= [];
        Data.Add("Entity", PrintService.GetEntityIdentityPrint(model));
        return this;
    }

    /// <summary>
    ///     Prints entity information. Type must be supported for print in method 'GetEntityIdentityPrint' in class
    ///     <see cref="Printer" />
    /// </summary>
    /// <param name="models">Models to print</param>
    /// <typeparam name="T">
    ///     Type of model to print - type must be supported for print in method 'GetEntityIdentityPrint' in
    ///     class <see cref="Printer" />
    /// </typeparam>
    public HandbookReference WithIdentityPrints<T>(IEnumerable<T> models)
    {
        Data ??= [];
        Data.Add("Entity", PrintService.GetEntityIdentityPrint(models));
        return this;
    }

    /// <summary>
    ///     Marks reference as higher priority with serious consequences to migration of data to target instance
    /// </summary>
    public HandbookReference NeedsManualAction()
    {
        NeedManualAction = true;
        return this;
    }
}
