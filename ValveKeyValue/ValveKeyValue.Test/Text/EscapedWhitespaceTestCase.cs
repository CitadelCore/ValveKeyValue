using NUnit.Framework;

namespace ValveKeyValue.Test
{
    class EscapedWhitespaceTestCase
    {
        [Test]
        public void ConvertsBackslashCharToActualRepresentation()
        {
            Assert.That((string)data["key"], Is.EqualTo("line1\nline2\tline2pt2"));
        }

        KvObject data;

        [OneTimeSetUp]
        public void SetUp()
        {
            var options = new KvSerializerOptions { HasEscapeSequences = true };
            using (var stream = TestDataHelper.OpenResource("Text.escaped_whitespace.vdf"))
            {
                data = KvSerializer.Create(KvSerializationFormat.KeyValues1Text).Deserialize(stream, options);
            }
        }
    }
}
