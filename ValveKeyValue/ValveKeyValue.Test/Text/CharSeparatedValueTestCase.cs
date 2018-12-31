using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ValveKeyValue.Attributes;
using ValveKeyValue.Test.Test_Data;

namespace ValveKeyValue.Test.Text
{
    public class CharSeparatedValueTestCase
    {
        [Test]
        public void IsNotNull()
        {
            Assert.That(_data, Is.Not.Null);
        }

        [OneTimeSetUp]
        public void SetUp()
        {
            using (var stream = TestDataHelper.OpenResource("Text.char_separated_values.vdf"))
                _data = KvSerializer.Create(KvSerializationFormat.KeyValues1Text).Deserialize<SerializedType>(stream);
        }

        private SerializedType _data;

        // ReSharper disable once ClassNeverInstantiated.Local
        private class SerializedType
        {
            [KvProperty(CollectionType = KvCollectionType.CharSeparated, CollectionTypeSeparator = ',')]
            public int[] CommaSeparated { get; set; }

            [KvProperty(CollectionType = KvCollectionType.CharSeparated, CollectionTypeSeparator = ' ')]
            public int[] WhitespaceSeparated { get; set; }
        }
    }
}
