namespace Migration.Toolkit.Core;

using System.Data.Common;
using System.Xml.Linq;
using CMS.Base;
using CMS.Helpers;
using Newtonsoft.Json;

public static class Extensions
{
    public static void SetValueAsJson<TValue>(this ISimpleDataContainer container, string column, TValue value)
    {
        container.SetValue(column, !value?.Equals(default) ?? false ? JsonConvert.SerializeObject(value) : null);
    }

    public static void SetValueAsJson<TValue>(this ISimpleDataContainer container, string column, IEnumerable<TValue> values)
    {
        container.SetValue(column, values.Any() ? JsonConvert.SerializeObject(values) : null);
    }

    public static void SetValueAsJson<TValue>(this Dictionary<string, object?> container, string column, TValue value)
    {
        container[column] = value?.Equals(default) ?? true ? null : JsonConvert.SerializeObject(value);
    }

    public static void SetValueAsJson<TValue>(this Dictionary<string, object?> container, string column, IEnumerable<TValue> values)
    {
        container[column] = values.Any() ? JsonConvert.SerializeObject(values) : null;
    }

    #region System.Xml.Linq extensions

    /// <summary>
    ///
    /// </summary>
    /// <param name="element"></param>
    /// <param name="name"></param>
    /// <param name="elementUpdate"></param>
    /// <returns>Returns ensured XElement</returns>
    public static XElement EnsureElement(this XElement element, XName name, Action<XElement>? elementUpdate = null)
    {
        if (element.Element(name) is { } result)
        {
            elementUpdate?.Invoke(result);
            return result;
        }
        else
        {
            var newElement = new XElement(name);
            elementUpdate?.Invoke(newElement);
            element.Add(newElement);
            return newElement;
        }
    }

    #endregion

    public static T Unbox<T>(this DbDataReader reader, string propertyName)
    {
        if (reader.GetOrdinal(propertyName) < 0)
        {
            throw new InvalidOperationException($"Property '{propertyName}' not exists");
        }

        return reader[propertyName] switch
        {
            T r => r,
            DBNull => default,
            _ => throw new InvalidCastException($"Unboxing property '{propertyName}' of type '{reader[propertyName].GetType().FullName}' to type '{typeof(T).FullName}' failed")
        };
    }

    public static T Value<T>(this XElement element)
    {
        return element?.Value == default
            ? default
            : ValidationHelper.GetValue<T>(element.Value);
    }


    public static bool? ValueAsBool(this XElement element) {

        if (element != null && bool.TryParse(element.Value, out var value)) {
            return value;
        }
        else return default;
    }
}