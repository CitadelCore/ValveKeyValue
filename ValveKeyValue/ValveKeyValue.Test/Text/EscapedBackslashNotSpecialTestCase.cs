using NUnit.Framework;
using ValveKeyValue.Test.Test_Data;

namespace ValveKeyValue.Test.Text
{
    internal class EscapedBackslashNotSpecialTestCase
    {
        [Test]
        public void ReadsDoubleBackslashQuoteCorrectly()
        {
            Assert.That((string)_data["edge case"], Is.EqualTo(@"this is \"" quite fun"));
        }

        private KvObject _data;

        [OneTimeSetUp]
        public void SetUp()
        {
            using (var stream = TestDataHelper.OpenResource("Text.escaped_backslash_not_special.vdf"))
                _data = KvSerializer.Create(KvSerializationFormat.KeyValues1Text).Deserialize(stream);
        }
    }
}
