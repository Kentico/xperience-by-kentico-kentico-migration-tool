using System.Collections;
using System.Reflection;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Migration.Tool.Common.Helpers;

public class SerializationHelper
{
    public static string SerializeOnlyNonComplexProperties<T>(T obj) => JsonConvert.SerializeObject(obj, Formatting.Indented,
        new JsonSerializerSettings { ContractResolver = new ShouldSerializeContractResolver(), ReferenceLoopHandling = ReferenceLoopHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore, MaxDepth = 1 });

    public static string Serialize<T>(T obj) => JsonConvert.SerializeObject(obj);

    public class ShouldSerializeContractResolver : DefaultContractResolver
    {
        public static readonly ShouldSerializeContractResolver Instance = new();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            property.ShouldSerialize = o => (!property.PropertyType!.IsClass && !property.PropertyType.IsArray && !typeof(IEnumerable).IsAssignableFrom(property.PropertyType)) || property.PropertyType == typeof(string);
            return property;
        }
    }
}
