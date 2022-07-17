namespace Migration.Toolkit.Common.Helpers;

using System.Collections;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public class SerializationHelper
{
    public class ShouldSerializeContractResolver : DefaultContractResolver
    {
        public static readonly ShouldSerializeContractResolver Instance = new();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            property.ShouldSerialize = o => (!property.PropertyType.IsClass && !property.PropertyType.IsArray && !typeof(IEnumerable).IsAssignableFrom(property.PropertyType)) || property.PropertyType == typeof(string);
            return property;
        }
    }
    
    public static string SerializeOnlyNonComplexProperties<T>(T obj)
    {
        return JsonConvert.SerializeObject(obj, Formatting.Indented,
            new JsonSerializerSettings
            {
                ContractResolver = new ShouldSerializeContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                MaxDepth = 1
            });
    }
}