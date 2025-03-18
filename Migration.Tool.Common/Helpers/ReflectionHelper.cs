using System.Diagnostics;
using System.Reflection;

namespace Migration.Tool.Common.Helpers;

public static class ReflectionHelper<T>
{
    static ReflectionHelper()
    {
        int i = 0;
        PropertyGetterMaps = [];
        foreach (var propertyInfo in CurrentType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
        {
            if (PropertyGetterMaps.ContainsKey(propertyInfo.Name))
            {
                Debugger.Break();
            }

            PropertyGetterMaps.Add(propertyInfo.Name, new ObjectPropertyGetterMap { PropertyName = propertyInfo.Name, PropertyGetMethod = propertyInfo.GetMethod, PropertyIndex = i++ });
        }

        StaticPropertyGetterMaps = [];
        foreach (var propertyInfo in CurrentType.GetProperties(BindingFlags.Public | BindingFlags.Static))
        {
            if (StaticPropertyGetterMaps.ContainsKey(propertyInfo.Name))
            {
                Debugger.Break();
            }

            StaticPropertyGetterMaps.Add(propertyInfo.Name, new ObjectPropertyGetterMap { PropertyName = propertyInfo.Name, PropertyGetMethod = propertyInfo.GetMethod, PropertyIndex = i++ });
        }
    }

    public static Type CurrentType { get; } = typeof(T);

    public static Dictionary<string, ObjectPropertyGetterMap> StaticPropertyGetterMaps { get; }
    public static Dictionary<string, ObjectPropertyGetterMap> PropertyGetterMaps { get; }

    public static List<TAttribute> GetAttributes<TAttribute>() where TAttribute : Attribute => Attribute
        .GetCustomAttributes(typeof(T)).Aggregate(new List<TAttribute>(), (list, attribute) =>
        {
            if (attribute is TAttribute targetAttribute)
            {
                list.Add(targetAttribute);
            }

            return list;
        });

    public static TAttribute? GetFirstAttributeOrNull<TAttribute>() where TAttribute : Attribute => GetAttributes<TAttribute>().FirstOrDefault();

    public static IEnumerable<ObjectPropertyGetterMap> GetPropertyGetterMaps() => PropertyGetterMaps.Values;

    public static object? GetPropertyValue(T obj, string propertyName)
    {
        if (!PropertyGetterMaps.ContainsKey(propertyName))
        {
            throw new InvalidOperationException($"Property '{propertyName}' doesn't exist on object of type '{CurrentType.FullName}'");
        }

        if (obj == null)
        {
            throw new NullReferenceException("Null reference, ReflectionHelper.GetPropertyValue needs obj argument reference");
        }

        return PropertyGetterMaps[propertyName].PropertyGetMethod!.Invoke(obj, Array.Empty<object>());
    }

    public static bool TryGetPropertyValue(T obj, string propertyName, StringComparison propNameComparison, out object? value)
    {
        string? propName = PropertyGetterMaps.Keys.FirstOrDefault(x => x.Equals(propertyName, propNameComparison));
        if (propName == null || !PropertyGetterMaps.ContainsKey(propName))
        {
            value = null;
            return false;
        }

        if (obj == null)
        {
            throw new NullReferenceException("Null reference, ReflectionHelper.GetPropertyValue needs obj argument reference");
        }

        value = PropertyGetterMaps[propName].PropertyGetMethod!.Invoke(obj, Array.Empty<object>());
        return true;
    }

    public static object? GetStaticPropertyValue(string propertyName)
    {
        if (!StaticPropertyGetterMaps.ContainsKey(propertyName))
        {
            throw new InvalidOperationException($"Property '{propertyName}' doesn't exist on object of type '{CurrentType.FullName}'");
        }

        return StaticPropertyGetterMaps[propertyName].PropertyGetMethod!.Invoke(null, Array.Empty<object>());
    }

    public static object? GetStaticFieldOrNull(string memberName)
    {
        var type = typeof(T);
        return type.GetField(memberName, BindingFlags.Static | BindingFlags.Public)?.GetValue(type);
    }

    public static object? GetStaticPropertyOrNull(string propertyName) => (!StaticPropertyGetterMaps.ContainsKey(propertyName)) ? null : StaticPropertyGetterMaps[propertyName].PropertyGetMethod!.Invoke(null, Array.Empty<object>());
}

public class ObjectPropertyGetterMap
{
    public string? PropertyName { get; set; }
    public MethodInfo? PropertyGetMethod { get; set; }
    public int PropertyIndex { get; set; }
}

public class ReflectionHelper
{
    public ReflectionHelper(Type type) => CurrentType = type;
    public Type CurrentType { get; }

    public List<TAttribute> GetAttributes<TAttribute>() where TAttribute : Attribute => Attribute
        .GetCustomAttributes(CurrentType).Aggregate(new List<TAttribute>(), (list, attribute) =>
        {
            if (attribute is TAttribute targetAttribute)
            {
                list.Add(targetAttribute);
            }

            return list;
        });

    public TAttribute? GetFirstAttributeOrNull<TAttribute>() where TAttribute : Attribute => GetAttributes<TAttribute>().FirstOrDefault();
}
