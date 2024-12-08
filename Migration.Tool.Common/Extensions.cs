using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Xml.Linq;

using CMS.Base;
using CMS.Helpers;

using Newtonsoft.Json;

namespace Migration.Tool.Common;

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
            ExpressionType.Add => throw new NotImplementedException(),
            ExpressionType.AddChecked => throw new NotImplementedException(),
            ExpressionType.And => throw new NotImplementedException(),
            ExpressionType.AndAlso => throw new NotImplementedException(),
            ExpressionType.ArrayLength => throw new NotImplementedException(),
            ExpressionType.ArrayIndex => throw new NotImplementedException(),
            ExpressionType.Call => throw new NotImplementedException(),
            ExpressionType.Coalesce => throw new NotImplementedException(),
            ExpressionType.Conditional => throw new NotImplementedException(),
            ExpressionType.Constant => throw new NotImplementedException(),
            ExpressionType.Divide => throw new NotImplementedException(),
            ExpressionType.Equal => throw new NotImplementedException(),
            ExpressionType.ExclusiveOr => throw new NotImplementedException(),
            ExpressionType.GreaterThan => throw new NotImplementedException(),
            ExpressionType.GreaterThanOrEqual => throw new NotImplementedException(),
            ExpressionType.Invoke => throw new NotImplementedException(),
            ExpressionType.Lambda => throw new NotImplementedException(),
            ExpressionType.LeftShift => throw new NotImplementedException(),
            ExpressionType.LessThan => throw new NotImplementedException(),
            ExpressionType.LessThanOrEqual => throw new NotImplementedException(),
            ExpressionType.ListInit => throw new NotImplementedException(),
            ExpressionType.MemberAccess => throw new NotImplementedException(),
            ExpressionType.MemberInit => throw new NotImplementedException(),
            ExpressionType.Modulo => throw new NotImplementedException(),
            ExpressionType.Multiply => throw new NotImplementedException(),
            ExpressionType.MultiplyChecked => throw new NotImplementedException(),
            ExpressionType.Negate => throw new NotImplementedException(),
            ExpressionType.UnaryPlus => throw new NotImplementedException(),
            ExpressionType.NegateChecked => throw new NotImplementedException(),
            ExpressionType.New => throw new NotImplementedException(),
            ExpressionType.NewArrayInit => throw new NotImplementedException(),
            ExpressionType.NewArrayBounds => throw new NotImplementedException(),
            ExpressionType.Not => throw new NotImplementedException(),
            ExpressionType.NotEqual => throw new NotImplementedException(),
            ExpressionType.Or => throw new NotImplementedException(),
            ExpressionType.OrElse => throw new NotImplementedException(),
            ExpressionType.Parameter => throw new NotImplementedException(),
            ExpressionType.Power => throw new NotImplementedException(),
            ExpressionType.Quote => throw new NotImplementedException(),
            ExpressionType.RightShift => throw new NotImplementedException(),
            ExpressionType.Subtract => throw new NotImplementedException(),
            ExpressionType.SubtractChecked => throw new NotImplementedException(),
            ExpressionType.TypeAs => throw new NotImplementedException(),
            ExpressionType.TypeIs => throw new NotImplementedException(),
            ExpressionType.Assign => throw new NotImplementedException(),
            ExpressionType.Block => throw new NotImplementedException(),
            ExpressionType.DebugInfo => throw new NotImplementedException(),
            ExpressionType.Decrement => throw new NotImplementedException(),
            ExpressionType.Dynamic => throw new NotImplementedException(),
            ExpressionType.Default => throw new NotImplementedException(),
            ExpressionType.Extension => throw new NotImplementedException(),
            ExpressionType.Goto => throw new NotImplementedException(),
            ExpressionType.Increment => throw new NotImplementedException(),
            ExpressionType.Index => throw new NotImplementedException(),
            ExpressionType.Label => throw new NotImplementedException(),
            ExpressionType.RuntimeVariables => throw new NotImplementedException(),
            ExpressionType.Loop => throw new NotImplementedException(),
            ExpressionType.Switch => throw new NotImplementedException(),
            ExpressionType.Throw => throw new NotImplementedException(),
            ExpressionType.Try => throw new NotImplementedException(),
            ExpressionType.Unbox => throw new NotImplementedException(),
            ExpressionType.AddAssign => throw new NotImplementedException(),
            ExpressionType.AndAssign => throw new NotImplementedException(),
            ExpressionType.DivideAssign => throw new NotImplementedException(),
            ExpressionType.ExclusiveOrAssign => throw new NotImplementedException(),
            ExpressionType.LeftShiftAssign => throw new NotImplementedException(),
            ExpressionType.ModuloAssign => throw new NotImplementedException(),
            ExpressionType.MultiplyAssign => throw new NotImplementedException(),
            ExpressionType.OrAssign => throw new NotImplementedException(),
            ExpressionType.PowerAssign => throw new NotImplementedException(),
            ExpressionType.RightShiftAssign => throw new NotImplementedException(),
            ExpressionType.SubtractAssign => throw new NotImplementedException(),
            ExpressionType.AddAssignChecked => throw new NotImplementedException(),
            ExpressionType.MultiplyAssignChecked => throw new NotImplementedException(),
            ExpressionType.SubtractAssignChecked => throw new NotImplementedException(),
            ExpressionType.PreIncrementAssign => throw new NotImplementedException(),
            ExpressionType.PreDecrementAssign => throw new NotImplementedException(),
            ExpressionType.PostIncrementAssign => throw new NotImplementedException(),
            ExpressionType.PostDecrementAssign => throw new NotImplementedException(),
            ExpressionType.TypeEqual => throw new NotImplementedException(),
            ExpressionType.OnesComplement => throw new NotImplementedException(),
            ExpressionType.IsTrue => throw new NotImplementedException(),
            ExpressionType.IsFalse => throw new NotImplementedException(),
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
        => Enum.TryParse<TEnum>(value, out var val) ? val : default;

    public static int? NullIfZero(this int? value) => value == 0 ? null : value;
    public static int? NullIfZero(this int value) => value == 0 ? null : value;

    public static string? NullIf(this string? s, string value) => s == value ? null : s;
    public static string? NullIf(this string? s, string value, StringComparison comparer) => string.Equals(s, value, comparer) ? null : s;

    public static void SetValueAsJson<TValue>(this ISimpleDataContainer container, string column, TValue value) => container.SetValue(column, !value?.Equals(default) ?? false ? JsonConvert.SerializeObject(value) : null);

    public static void SetValueAsJson<TValue>(this ISimpleDataContainer container, string column, IEnumerable<TValue> values) => container.SetValue(column, values.Any() ? JsonConvert.SerializeObject(values) : null);

    public static void SetValueAsJson<TValue>(this Dictionary<string, object?> container, string column, TValue value) => container[column] = value?.Equals(default) ?? true ? null : JsonConvert.SerializeObject(value);

    public static void SetValueAsJson<TValue>(this Dictionary<string, object?> container, string column, IEnumerable<TValue> values) => container[column] = values.Any() ? JsonConvert.SerializeObject(values) : null;

    #region System.Xml.Linq extensions

    /// <summary>
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

        var newElement = new XElement(name);
        elementUpdate?.Invoke(newElement);
        element.Add(newElement);
        return newElement;
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

    public static T Value<T>(this XElement element) => element?.Value == default
        ? default
        : ValidationHelper.GetValue<T>(element.Value);


    public static bool? ValueAsBool(this XElement element)
    {
        if (element != null && bool.TryParse(element.Value, out bool value))
        {
            return value;
        }

        return default;
    }
}
