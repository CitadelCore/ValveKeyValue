using NUnit.Framework;
using ValveKeyValue.Test.Test_Data;

namespace ValveKeyValue.Test.Text
{
    internal class EscapedQuotationMarksTestCase
    {
        [Test]
        public void QuotedKeyReturnsQuotedValue()
        {
            Assert.That((string)_data["name \"of\" key"], Is.EqualTo("value \"of\" key"));
        }

        private KvObject _data;

        [OneTimeSetUp]
        public void SetUp()
        {
            using (var stream = TestDataHelper.OpenResource("Text.escaped_quotation_marks.vdf"))
                _data = KvSerializer.Create(KvSerializationFormat.KeyValues1Text).Deserialize(stream);
        }
    }
}
