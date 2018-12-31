using System.Collections.Generic;
using System.Reflection;
using ValveKeyValue.Attributes;

namespace ValveKeyValue
{
    internal sealed class DefaultObjectReflector : IObjectReflector
    {
        IEnumerable<IObjectMember> IObjectReflector.GetMembers(object @object)
        {
            Require.NotNull(@object, nameof(@object));
            var properties = @object.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var property in properties)
            {
                if (property.GetCustomAttribute<KvIgnoreAttribute>() != null)
                    continue;
                yield return new PropertyMember(property, @object);
            }
        }
    }
}
