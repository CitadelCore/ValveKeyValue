using System;

namespace ValveKeyValue.Deserialization
{
    internal interface IVisitingReader : IDisposable
    {
        void ReadObject();
    }
}
