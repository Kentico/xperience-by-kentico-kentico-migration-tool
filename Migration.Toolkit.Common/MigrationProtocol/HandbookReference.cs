namespace Migration.Toolkit.Common.MigrationProtocol;

using System.Reflection;
using System.Text;
using Migration.Toolkit.Common.Services;

public class HandbookReference
{
    public static IPrintService PrintService = default;

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"NeedManualAction: {NeedManualAction}, ReferenceName: {ReferenceName}, AdditionalInfo: {AdditionalInfo}");
        if (this.Data == null) return sb.ToString();

        var arr = this.Data.ToArray();
        for (var i = 0; i < arr.Length; i++)
        {
            var (key, value) = arr[i];
            sb.Append($"{key}: {value}");

            if (i < arr.Length - 1)
            {
                sb.Append(", ");
            }
        }

        return sb.ToString();
    }

    public bool NeedManualAction { get; private set; } = false;
    public string ReferenceName { get; }
    public string? AdditionalInfo { get; }
    public Dictionary<string, object?>? Data { get; private set; } = null;

    /// <summary>
    /// Use <see cref="HandbookReferences"/> class to store factory methods, don't create instances directly in code.
    /// </summary>
    /// <param name="referenceName">Use common identifier characters to describe handbook reference (consider usage in HTML, JSON, DB, C#, href attribute in HTML)</param>
    public HandbookReference(string referenceName, string? additionalInfo = null)
    {
        this.ReferenceName = referenceName;
        this.AdditionalInfo = additionalInfo;
    }

    /// <summary>
    /// Related ID of data, specify if possible
    /// </summary>
    public HandbookReference WithId(string idName, object idValue)
    {
        this.WithData(idName, idValue);
        return this;
    }

    /// <summary>
    /// Appends message for user to see, use it for describing issue and propose fix if possible
    /// </summary>
    public HandbookReference WithMessage(string message)
    {
        this.Data ??= new();

        var msgNum = 0;
        string msgKey;
        do
        {
            msgKey = $"Message#{msgNum++}";
        } while (this.Data.ContainsKey(msgKey));

        this.WithData(msgKey, message);

        return this;
    }

    /// <summary>
    /// Appends data to dictionary for user to see
    /// </summary>
    public HandbookReference WithData(string key, object value)
    {
        this.Data ??= new();
        this.Data.Add(key, value);

        return this;
    }

    /// <summary>
    /// Appends data to dictionary for user to see
    /// </summary>
    public HandbookReference WithData<TValue>(Dictionary<string, TValue> data)
    {
        this.Data ??= new();
        foreach (var (key, value) in data)
        {
            this.Data.Add(key, value);
        }

        return this;
    }

    /// <summary>
    /// Appends data to dictionary for user to see
    /// </summary>
    /// <param name="data">All public instance properties of object are written to dictionary for user to see. Anonymous object can be used</param>
    public HandbookReference WithData(object data)
    {
        this.Data ??= new();
        var dataUpdate = data.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .ToDictionary(p => p.Name, p => p.GetMethod.Invoke(data, Array.Empty<object>())
            );
        foreach (var (key, value) in dataUpdate)
        {
            this.Data.Add(key, value);
        }

        return this;
    }

    /// <summary>
    /// Prints entity information. Type must be supported for print in method 'GetEntityIdentityPrint' in class <see cref="Printer"/>
    /// </summary>
    /// <param name="model">Models to print</param>
    /// <typeparam name="T">Type of model to print - type must be supported for print in method 'GetEntityIdentityPrint' in class <see cref="Printer"/></typeparam>
    public HandbookReference WithIdentityPrint<T>(T model)
    {
        this.Data ??= new();
        this.Data.Add("Entity", PrintService.GetEntityIdentityPrint(model));
        return this;
    }

    /// <summary>
    /// Prints entity information. Type must be supported for print in method 'GetEntityIdentityPrint' in class <see cref="Printer"/>
    /// </summary>
    /// <param name="models">Models to print</param>
    /// <typeparam name="T">Type of model to print - type must be supported for print in method 'GetEntityIdentityPrint' in class <see cref="Printer"/></typeparam>
    public HandbookReference WithIdentityPrints<T>(IEnumerable<T> models)
    {
        this.Data ??= new();
        this.Data.Add("Entity", PrintService.GetEntityIdentityPrint(models));
        return this;
    }

    /// <summary>
    /// Marks reference as higher priority with serious consequences to migration of data to target instance
    /// </summary>
    public HandbookReference NeedsManualAction()
    {
        this.NeedManualAction = true;
        return this;
    }
}