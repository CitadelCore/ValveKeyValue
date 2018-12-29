using System;
using System.Collections;
using NUnit.Framework;

namespace ValveKeyValue.Test
{
    class KVSerializerNullInputsTestCase
    {
        [TestCaseSource(nameof(Formats))]
        public void DeserializeWithNullStream(KvSerializationFormat format)
        {
            Assert.That(
                () => KvSerializer.Create(format).Deserialize(stream: null),
                Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("stream"));
        }

        public static IEnumerable Formats => Enum.GetValues(typeof(KvSerializationFormat));
    }
}
