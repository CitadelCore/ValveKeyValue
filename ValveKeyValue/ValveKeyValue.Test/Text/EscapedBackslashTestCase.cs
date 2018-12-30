using NUnit.Framework;
using ValveKeyValue.Test.Test_Data;

namespace ValveKeyValue.Test.Text
{
    internal class EscapedBackslashTestCase
    {
        [Test]
        public void ConvertsDoubleBackslashToSingleBackslash()
        {
            Assert.That((string)_data["key"], Is.EqualTo(@"back\slash"));
        }

        [Test]
        public void DoubleBackslashQuoteEscapesJustTheBackslashNotTheQuote()
        {
            Assert.That((string)_data["edge case"], Is.EqualTo(@"this is fun\"));
        }

        private KvObject _data;

        [OneTimeSetUp]
        public void SetUp()
        {
            var options = new KvSerializerOptions { HasEscapeSequences = true };
            using (var stream = TestDataHelper.OpenResource("Text.escaped_backslash.vdf"))
                _data = KvSerializer.Create(KvSerializationFormat.KeyValues1Text).Deserialize(stream, options);
        }
    }
}
