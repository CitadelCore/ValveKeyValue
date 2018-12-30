using System.Collections.Generic;
using NUnit.Framework;
using ValveKeyValue.Test.Test_Data;

namespace ValveKeyValue.Test.Text
{
    internal class DictionaryPropertyTestCase
    {
        [Test]
        public void IsNotNull()
        {
            Assert.That(_data, Is.Not.Null);
        }

        [Test]
        public void NumbersIsNotNullEmpty()
        {
            Assert.That(_data.Numbers, Is.Not.Null.Or.Empty);
        }

        [Test]
        public void HasNumbers()
        {
            Assert.That(_data.Numbers, Has.Count.EqualTo(14));
        }

        [TestCase(0, "zero")]
        [TestCase(1, "one")]
        [TestCase(2, "two")]
        [TestCase(3, "three")]
        [TestCase(4, "four")]
        [TestCase(5, "five")]
        [TestCase(6, "six")]
        [TestCase(7, "seven")]
        [TestCase(8, "eight")]
        [TestCase(9, "nine")]
        [TestCase(10, "ten")]
        [TestCase(11, "eleven")]
        [TestCase(12, "twelve")]
        [TestCase(13, "thirteen")]
        public void NumbersHasValue(int key, string value)
        {
            Assert.That(_data.Numbers[key], Is.EqualTo(value));
        }

        private ContainerClass _data;

        [OneTimeSetUp]
        public void SetUp()
        {
            using (var stream = TestDataHelper.OpenResource("Text.list_of_values.vdf"))
                _data = KvSerializer.Create(KvSerializationFormat.KeyValues1Text).Deserialize<ContainerClass>(stream);
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class ContainerClass
        {
            public Dictionary<int, string> Numbers { get; set; }
        }
    }
}
