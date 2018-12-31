using System;
using System.Reflection;
using ValveKeyValue.Attributes;

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

        bool IObjectMember.IsExplicitName => PropertyAttribute?.PropertyName != null;

        string IObjectMember.Name
            => PropertyAttribute?.PropertyName ?? _propertyInfo.Name;

        KvCollectionType IObjectMember.CollectionType 
            => PropertyAttribute?.CollectionType ?? KvCollectionType.Default;

        char IObjectMember.CollectionTypeSeparator
            => PropertyAttribute?.CollectionTypeSeparator ?? ',';

        Type IObjectMember.MemberType => _propertyInfo.PropertyType;

        object IObjectMember.Value
        {
            get => _propertyInfo.GetValue(_object);
            set => _propertyInfo.SetValue(_object, value);
        }

        private KvPropertyAttribute PropertyAttribute => _propertyInfo.GetCustomAttribute<KvPropertyAttribute>();
    }
}
