using System.Linq.Expressions;

namespace Migration.Toolkit.Common;

using System.Data;
using System.Data.Common;
using System.Xml.Linq;
using CMS.Base;
using CMS.Helpers;
using Newtonsoft.Json;

public static class Extensions
{
    public static T Unbox<T>(this IDataReader reader, string propertyName)
    {
        if (reader.GetOrdinal(propertyName) < 0)
        {
            throw new InvalidOperationException($"Property '{propertyName}' not exists");
        }

        return reader[propertyName] switch
        {
            T r => r,
            DBNull => default!,
            _ => throw new InvalidCastException($"Unboxing property '{propertyName}' of type '{reader[propertyName].GetType().FullName}' to type '{typeof(T).FullName}' failed")
        };
    }

    public static bool IsIn<T>(this T value, params T[] values) => values.Contains(value);

    public static string GetMemberName<T>(this Expression<Func<T, object>> expr)
    {
        var stack = new Stack<string>();

        var me = expr.Body.NodeType switch
        {
            ExpressionType.Convert => (expr.Body is UnaryExpression ue ? ue.Operand : null) as MemberExpression,
            ExpressionType.ConvertChecked => (expr.Body is UnaryExpression ue ? ue.Operand : null) as MemberExpression,
            _ => expr.Body as MemberExpression
        };

        while (me != null)
        {
            stack.Push(me.Member.Name);
            me = me.Expression as MemberExpression;
        }

        return string.Join(".", stack.ToArray());
    }

    public static bool UseKenticoDefault(this bool? value) => value ?? false;
    public static int UseKenticoDefault(this int? value) => value ?? 0;

    public static TEnum AsEnum<TEnum>(this int? value) where TEnum : Enum
        => (TEnum)Enum.ToObject(typeof(TEnum), value ?? 0);

    public static TEnum AsEnum<TEnum>(this string? value) where TEnum : struct, Enum
        => Enum.TryParse<TEnum>(value, out var val) ? val : default(TEnum);

    public static int? NullIfZero(this int? value) => value == 0 ? null : value;
    public static int? NullIfZero(this int value) => value == 0 ? null : value;

    public static string? NullIf(this string? s, string value) => s == value ? null : s;

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


    public static bool? ValueAsBool(this XElement element)
    {

        if (element != null && bool.TryParse(element.Value, out var value))
        {
            return value;
        }
        else return default;
    }
}