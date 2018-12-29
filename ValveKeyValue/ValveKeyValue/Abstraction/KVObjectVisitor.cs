using System;
using System.Collections.Generic;

namespace ValveKeyValue.Abstraction
{
    internal sealed class KvObjectVisitor
    {
        public KvObjectVisitor(IVisitationListener listener)
        {
            Require.NotNull(listener, nameof(listener));
            _listener = listener;
        }

        private readonly IVisitationListener _listener;

        public void Visit(KvObject @object)
        {
            VisitObject(@object.Name, @object.Value);
        }

        private void VisitObject(string name, KvValue value)
        {
            switch (value.ValueType)
            {
                case KvValueType.Collection:
                    _listener.OnObjectStart(name);
                    VisitValue((IEnumerable<KvObject>)value);
                    _listener.OnObjectEnd();
                    break;

                case KvValueType.FloatingPoint:
                case KvValueType.Int32:
                case KvValueType.Pointer:
                case KvValueType.String:
                case KvValueType.UInt64:
                case KvValueType.Int64:
                    _listener.OnKeyValuePair(name, value);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(value.ValueType), value.ValueType, "Unhandled value type.");
            }
        }

        private void VisitValue(IEnumerable<KvObject> collection)
        {
            foreach (var item in collection)
            {
                VisitObject(item.Name, item.Value);
            }
        }
    }
}
