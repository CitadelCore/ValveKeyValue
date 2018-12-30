using NUnit.Framework;
using ValveKeyValue.Test.Test_Data;

namespace ValveKeyValue.Test.Text
{
    internal class UnquotedTextTestCase
    {
        [Test]
        public void IsNotNull()
        {
            Assert.That(_data, Is.Not.Null);
        }

        [Test]
        public void Name()
        {
            Assert.That(_data.Name, Is.EqualTo("TestDocument"));
        }

        [Test]
        public void QuotedChildWithEdgeCaseValue()
        {
            Assert.That((string)_data["QuotedChild"], Is.EqualTo(@"edge\ncase\""haha\\"""));
        }

        [TestCase("Key1", ExpectedResult = "Value1")]
        [TestCase("Key2", ExpectedResult = "Value2")]
        [TestCase("Key3", ExpectedResult = "Value3")]
        public string UnquotedChildValue(string key) => (string)_data["UnquotedChild"][key];

        private KvObject _data;

        [OneTimeSetUp]
        public void SetUp()
        {
            using (var stream = TestDataHelper.OpenResource("Text.unquoted_document.vdf"))
                _data = KvSerializer.Create(KvSerializationFormat.KeyValues1Text).Deserialize(stream);
        }
    }
}