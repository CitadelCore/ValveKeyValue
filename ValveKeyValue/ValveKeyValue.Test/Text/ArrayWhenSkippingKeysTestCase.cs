using System;
using System.Collections.Generic;
using NUnit.Framework;
using ValveKeyValue.Test.Test_Data;

namespace ValveKeyValue.Test.Text
{
    [TestFixtureSource(typeof(TestFixtureSources), nameof(TestFixtureSources.SupportedEnumerableTypesForDeserialization))]
    internal class ArrayWhenSkippingKeysTestCase<TEnumerable>
        where TEnumerable : IEnumerable<string>
    {
        [Test]
        public void ThrowsInvalidOperationException()
        {
            using (var stream = TestDataHelper.OpenResource("Text.list_of_values_skipping_keys.vdf"))
            {
                Assert.That(
                     () => KvSerializer.Create(KvSerializationFormat.KeyValues1Text).Deserialize<SerializedType>(stream),
                     Throws.Exception.InstanceOf<InvalidOperationException>()
                     .With.Message.EqualTo($"Cannot deserialize a non-array value to type \"{typeof(TEnumerable).Namespace}.{typeof(TEnumerable).Name}\"."));
            }
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class SerializedType
        {
            public TEnumerable Numbers { get; set; }
        }
    }
}
