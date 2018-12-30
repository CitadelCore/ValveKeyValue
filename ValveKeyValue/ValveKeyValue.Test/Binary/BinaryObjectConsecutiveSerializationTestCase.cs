using System.IO;
using NUnit.Framework;

namespace ValveKeyValue.Test.Binary
{
    internal class BinaryObjectConsecutiveSerializationTestCase
    {
        [Test]
        public void SerializesToBinaryStructure()
        {
            var first = new KvObject("FirstObject", new[]
            {
                new KvObject("firstkey", "firstvalue")
            });

            var second = new KvObject("SecondObject", new[]
            {
                new KvObject("secondkey", "secondvalue")
            });

            var expectedData = new byte[]
            {
                0x00, // object: FirstObject
                0x46, 0x69, 0x72, 0x73, 0x74, 0x4f, 0x62, 0x6a, 0x65, 0x63, 0x74, 0x00,
                0x01, // string: firstkey = firstvalue
                0x66, 0x69, 0x72, 0x73, 0x74, 0x6b, 0x65, 0x79, 0x00,
                0x66, 0x69, 0x72, 0x73, 0x74, 0x76, 0x61, 0x6c, 0x75, 0x65, 0x00,
                0x08, // end object
                0x08, // end document

                0x00, // object: SecondObject
                0x53, 0x65, 0x63, 0x6f, 0x6e, 0x64, 0x4f, 0x62, 0x6a, 0x65, 0x63, 0x74, 0x00,
                0x01, // string: secondkey = secondvalue
                0x73, 0x65, 0x63, 0x6f, 0x6e, 0x64, 0x6b, 0x65, 0x79, 0x00,
                0x73, 0x65, 0x63, 0x6f, 0x6e, 0x64, 0x76, 0x61, 0x6c, 0x75, 0x65, 0x00,
                0x08, // end object
                0x08, // end document
            };

            using (var stream = new MemoryStream())
            {
                KvSerializer.Create(KvSerializationFormat.KeyValues1Binary).Serialize(stream, first);
                KvSerializer.Create(KvSerializationFormat.KeyValues1Binary).Serialize(stream, second);
                Assert.That(stream.ToArray(), Is.EqualTo(expectedData));
            }
        }
    }
}
