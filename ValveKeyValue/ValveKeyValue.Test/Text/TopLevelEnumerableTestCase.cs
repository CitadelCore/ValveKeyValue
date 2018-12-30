﻿using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ValveKeyValue.Test.Test_Data;

namespace ValveKeyValue.Test.Text
{
    [TestFixtureSource(typeof(TestFixtureSources), nameof(TestFixtureSources.SupportedEnumerableTypesForDeserialization))]
    internal class TopLevelEnumerableTestCase<TEnumerable>
        where TEnumerable : IEnumerable<string>
    {
        [Test]
        public void IsNotNull()
        {
            Assert.That(_data, Is.Not.Null);
        }

        [Test]
        public void IsNotEmpty()
        {
            Assert.That(_data.Any(), Is.True);
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
        public void HasValue(int index, string expectedValue)
        {
            var actualValue = _data.ToArray()[index];
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        private IEnumerable<string> _data;

        [OneTimeSetUp]
        public void SetUp()
        {
            using (var stream = TestDataHelper.OpenResource("Text.top_level_list_of_values.vdf"))
                _data = KvSerializer.Create(KvSerializationFormat.KeyValues1Text).Deserialize<TEnumerable>(stream);
        }
    }
}
