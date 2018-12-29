using NUnit.Framework;

namespace ValveKeyValue.Test
{
    class EscapedQuotationMarksTestCase
    {
        [Test]
        public void QuotedKeyReturnsQuotedValue()
        {
            Assert.That((string)data["name \"of\" key"], Is.EqualTo("value \"of\" key"));
        }

        KvObject data;

        [OneTimeSetUp]
        public void SetUp()
        {
            using (var stream = TestDataHelper.OpenResource("Text.escaped_quotation_marks.vdf"))
            {
                data = KvSerializer.Create(KvSerializationFormat.KeyValues1Text).Deserialize(stream);
            }
        }
    }
}
