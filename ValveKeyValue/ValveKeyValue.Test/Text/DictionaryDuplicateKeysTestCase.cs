using System.Collections.Generic;
using NUnit.Framework;
using ValveKeyValue.Test.Test_Data;

namespace ValveKeyValue.Test.Text
{
    internal class DictionaryDuplicateKeysTestCase
    {
        [Test]
        public void IsNotNull()
        {
            Assert.That(_data, Is.Not.Null);
        }

        [Test]
        public void IsNotEmpty()
        {
            Assert.That(_data, Is.Not.Empty);
        }

        [Test]
        public void LateValueOverridesEarlyValue()
        {
            Assert.That(_data["foo"], Is.EqualTo("baz"));
        }

        private Dictionary<string, string> _data;

        [OneTimeSetUp]
        public void SetUp()
        {
            using (var stream = TestDataHelper.OpenResource("Text.duplicate_keys.vdf"))
                _data = KvSerializer.Create(KvSerializationFormat.KeyValues1Text).Deserialize<Dictionary<string, string>>(stream);
        }
    }
}
