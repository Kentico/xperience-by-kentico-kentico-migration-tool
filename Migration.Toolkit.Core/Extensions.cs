namespace Migration.Toolkit.Core;

using System.Xml.Linq;
using CMS.Base;
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
}