using System;
using System.Reflection;

namespace ValveKeyValue
{
    internal sealed class PropertyMember : IObjectMember
    {
        public PropertyMember(PropertyInfo propertyInfo, object @object)
        {
            Require.NotNull(propertyInfo, nameof(propertyInfo));
            Require.NotNull(@object, nameof(@object));

            _propertyInfo = propertyInfo;
            _object = @object;
        }

        private readonly PropertyInfo _propertyInfo;
        private readonly object _object;

        bool IObjectMember.IsExplicitName => PropertyAttribute != null;

        string IObjectMember.Name
            => PropertyAttribute?.PropertyName ?? _propertyInfo.Name;

        Type IObjectMember.MemberType => _propertyInfo.PropertyType;

        object IObjectMember.Value
        {
            get => _propertyInfo.GetValue(_object);
            set => _propertyInfo.SetValue(_object, value);
        }

        private KvPropertyAttribute PropertyAttribute => _propertyInfo.GetCustomAttribute<KvPropertyAttribute>();
    }
}
