using System;
using NUnit.Framework;

namespace ValveKeyValue.Test
{
    internal class KvBasicObjectIndexerTestCase
    {
        [Test]
        public void IndexerOnValueNodeThrowsException()
        {
            Assert.That(
                () => _data["baz"],
                Throws.Exception.InstanceOf<InvalidOperationException>()
                .With.Message.EqualTo("This operation on a KvObject can only be used when the value has children."));
        }

        private KvObject _data;

        [OneTimeSetUp]
        public void SetUp()
        {
            _data = new KvObject("foo", "bar");
        }
    }
}
