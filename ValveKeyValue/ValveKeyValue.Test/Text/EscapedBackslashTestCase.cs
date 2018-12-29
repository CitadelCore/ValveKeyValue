using NUnit.Framework;

namespace ValveKeyValue.Test
{
    class EscapedBackslashTestCase
    {
        [Test]
        public void ConvertsDoubleBackslashToSingleBackslash()
        {
            Assert.That((string)data["key"], Is.EqualTo(@"back\slash"));
        }

        [Test]
        public void DoubleBackslashQuoteEscapesJustTheBackslashNotTheQuote()
        {
            Assert.That((string)data["edge case"], Is.EqualTo(@"this is fun\"));
        }

        KvObject data;

        [OneTimeSetUp]
        public void SetUp()
        {
            var options = new KvSerializerOptions { HasEscapeSequences = true };
            using (var stream = TestDataHelper.OpenResource("Text.escaped_backslash.vdf"))
            {
                data = KvSerializer.Create(KvSerializationFormat.KeyValues1Text).Deserialize(stream, options);
            }
        }
    }
}
