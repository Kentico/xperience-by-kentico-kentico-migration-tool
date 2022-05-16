namespace Migration.Toolkit.Common.Helpers;

public class ReflectionHelper<T>
{
    public static Type CurrentType { get; set; }
    
    static ReflectionHelper()
    {
        CurrentType = typeof(T);
    }

    public static List<TAttribute> GetAttributes<TAttribute>() where TAttribute: Attribute => Attribute
        .GetCustomAttributes(typeof(T)).Aggregate(new List<TAttribute>(), (list, attribute) =>
        {
            if (attribute is TAttribute targetAttribute)
            {
                list.Add(targetAttribute);
            }

            return list;
        });

    public static TAttribute? GetFirstAttributeOrNull<TAttribute>() where TAttribute : Attribute => GetAttributes<TAttribute>().FirstOrDefault();
}