using System;
using NUnit.Framework;

namespace ValveKeyValue.Test
{
    internal class KvObjectIndexerTestCase
    {
        [TestCase("foo", ExpectedResult = "bar")]
        [TestCase("bar", ExpectedResult = "baz")]
        [TestCase("baz", ExpectedResult = "-")]
        [TestCase("foobar", ExpectedResult = null)]
        public string IndexerReturnsChildValue(string key) => (string)_data[key];

        [Test]
        public void IndexerOnValueNodeThrowsException()
        {
            Assert.That(
                () => _data["foo"]["bar"],
                Throws.Exception.InstanceOf<NotSupportedException>()
                .With.Message.EqualTo("The indexer on a KvValue can only be used on a KvValue that has children."));
        }

        private KvObject _data;

        [OneTimeSetUp]
        public void SetUp()
        {
            _data = new KvObject(
                "test data",
                new[]
                {
                    new KvObject("foo", "bar"),
                    new KvObject("bar", "baz"),
                    new KvObject("baz", "-"),
                });
        }
    }
}
