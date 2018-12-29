using System.Collections.Generic;

namespace ValveKeyValue
{
    internal interface IObjectReflector
    {
        IEnumerable<IObjectMember> GetMembers(object @object);
    }
}
