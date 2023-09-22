using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace PosMaster.Helper
{
    /// <summary>
    /// Helps to ignore empty collection while serializing
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class SerializeContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// SerializeContractResolver Instance
        /// </summary>
        public static readonly SerializeContractResolver Instance = new SerializeContractResolver();

        /// <summary>
        /// Ignores empty array
        /// </summary>
        /// <param name="member"></param>
        /// <param name="memberSerialization"></param>
        /// <returns></returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (property.PropertyType != typeof(string))
            {
                if (property.PropertyType.GetInterface(nameof(IEnumerable)) != null)
                    property.ShouldSerialize =
                        instance => (instance?.GetType().GetProperty(property.PropertyName).GetValue(instance) as IEnumerable<object>)?.Count() > 0;
            }
            return property;
        }
    }
}
