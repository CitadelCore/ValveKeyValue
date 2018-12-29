﻿using System.IO;
using System.Linq;
using NUnit.Framework;

namespace ValveKeyValue.Test
{
    class IncludeTestCase
    {
        [Test]
        public void IsNotNull()
        {
            Assert.That(data, Is.Not.Null);
        }

        [Test]
        public void HasThreeItems()
        {
            Assert.That(data.Count(), Is.EqualTo(3));
        }

        [TestCase("foo", "bar")]
        [TestCase("bar", "baz")]
        [TestCase("baz", "nada")]
        public void HasKeyWithValue(string key, string expectedValue)
        {
            var actualValue = (string)data[key];
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        KvObject data;

        [OneTimeSetUp]
        public void SetUp()
        {
            var options = new KvSerializerOptions { FileLoader = new StubIncludedFileLoader() };

            using (var stream = TestDataHelper.OpenResource("Text.kv_with_include.vdf"))
            {
                data = KvSerializer.Create(KvSerializationFormat.KeyValues1Text).Deserialize(stream, options);
            }
        }

        sealed class StubIncludedFileLoader : IIncludedFileLoader
        {
            Stream IIncludedFileLoader.OpenFile(string filePath)
            {
                Assert.That(filePath, Is.EqualTo("file.vdf"));
                return TestDataHelper.OpenResource("Text.kv_included.vdf");
            }
        }
    }
}
