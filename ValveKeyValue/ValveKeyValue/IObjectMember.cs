using System;

namespace ValveKeyValue
{
    internal interface IObjectMember
    {
        bool IsExplicitName { get; }
        string Name { get; }
        Type MemberType { get; }
        object Value { get; set; }
    }
}
