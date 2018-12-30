using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ValveKeyValue.Test.Test_Data;

namespace ValveKeyValue.Test.Text
{
    [TestFixtureSource(typeof(TestFixtureSources), nameof(TestFixtureSources.SupportedEnumerableTypesForDeserialization))]
    internal class ArrayTestCase<TEnumerable>
        where TEnumerable : IEnumerable<string>
    {
        [Test]
        public void IsNotNull()
        {
            Assert.That(_data, Is.Not.Null);
        }

        [Test]
        public void NumbersIsNotNullOrEmpty()
        {
            Assert.That(_data.Numbers, Is.Not.Null);
            Assert.That(_data.Numbers.Any(), Is.True);
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
        public void NumbersListHasValue(int index, string expectedValue)
        {
            var actualValue = _data.Numbers.ToArray()[index];
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        private SerializedType _data;

        [OneTimeSetUp]
        public void SetUp()
        {
            using (var stream = TestDataHelper.OpenResource("Text.list_of_values.vdf"))
            {
                _data = KvSerializer.Create(KvSerializationFormat.KeyValues1Text).Deserialize<SerializedType>(stream);
            }
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class SerializedType
        {
            public TEnumerable Numbers { get; set; }
        }
    }
}
