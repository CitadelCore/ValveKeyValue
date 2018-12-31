using System;

namespace ValveKeyValue
{
    internal interface IObjectMember
    {
        bool IsExplicitName { get; }
        string Name { get; }
        KvCollectionType CollectionType { get; }
        Type MemberType { get; }
        object Value { get; set; }
    }
}
