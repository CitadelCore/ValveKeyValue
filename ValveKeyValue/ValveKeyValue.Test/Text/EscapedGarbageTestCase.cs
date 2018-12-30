using System.IO;
using NUnit.Framework;
using ValveKeyValue.Test.Test_Data;

namespace ValveKeyValue.Test.Text
{
    internal class EscapedGarbageTestCase
    {
        [Test]
        public void ReadsRawValueWhenNotHasEscapeSequences()
        {
            KvObject data;
            using (var stream = TestDataHelper.OpenResource("Text.escaped_garbage.vdf"))
            {
                data = KvSerializer.Create(KvSerializationFormat.KeyValues1Text).Deserialize(stream);
            }

            Assert.That((string)data["key"], Is.EqualTo(@"\7"));
        }

        [Test]
        public void ThrowsExceptionWhenHasEscapeSequences()
        {
            var options = new KvSerializerOptions { HasEscapeSequences = true };
            using (var stream = TestDataHelper.OpenResource("Text.escaped_garbage.vdf"))
            {
                Assert.That(
                    () => KvSerializer.Create(KvSerializationFormat.KeyValues1Text).Deserialize(stream, options),
                    Throws.Exception.TypeOf<KeyValueException>()
                    .With.InnerException.TypeOf<InvalidDataException>()
                    .With.Message.EqualTo(@"Unknown escaped character '\7'."));
            }
        }
    }
}
