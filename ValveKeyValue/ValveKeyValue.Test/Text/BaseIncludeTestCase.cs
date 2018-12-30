using System.IO;
using System.Linq;
using NUnit.Framework;
using ValveKeyValue.Test.Test_Data;

namespace ValveKeyValue.Test.Text
{
    internal class BaseIncludeTestCase
    {
        [Test]
        public void IsNotNull()
        {
            Assert.That(_data, Is.Not.Null);
        }

        [Test]
        public void HasThreeItems()
        {
            Assert.That(_data.Count(), Is.EqualTo(3));
        }

        [TestCase("foo", "bar")]
        [TestCase("bar", "baz")]
        [TestCase("baz", "nada")]
        public void HasKeyWithValue(string key, string expectedValue)
        {
            var actualValue = (string)_data[key];
            Assert.That(actualValue, Is.EqualTo(expectedValue), key);
        }

        private KvObject _data;

        [OneTimeSetUp]
        public void SetUp()
        {
            var options = new KvSerializerOptions { FileLoader = new StubIncludedFileLoader() };

            using (var stream = TestDataHelper.OpenResource("Text.kv_with_base.vdf"))
            {
                _data = KvSerializer.Create(KvSerializationFormat.KeyValues1Text).Deserialize(stream, options);
            }
        }

        private sealed class StubIncludedFileLoader : IIncludedFileLoader
        {
            Stream IIncludedFileLoader.OpenFile(string filePath)
            {
                Assert.That(filePath, Is.EqualTo("file.vdf"));
                return TestDataHelper.OpenResource("Text.kv_base_included.vdf");
            }
        }
    }
}
