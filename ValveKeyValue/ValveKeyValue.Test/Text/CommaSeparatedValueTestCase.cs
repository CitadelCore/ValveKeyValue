using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ValveKeyValue.Attributes;
using ValveKeyValue.Test.Test_Data;

namespace ValveKeyValue.Test.Text
{
    public class CommaSeparatedValueTestCase
    {
        [Test]
        public void IsNotNull()
        {
            Assert.That(_data, Is.Not.Null);
        }

        [OneTimeSetUp]
        public void SetUp()
        {
            using (var stream = TestDataHelper.OpenResource("Text.comma_separated_values.vdf"))
                _data = KvSerializer.Create(KvSerializationFormat.KeyValues1Text).Deserialize<SerializedType>(stream);
        }

        private SerializedType _data;

        // ReSharper disable once ClassNeverInstantiated.Local
        private class SerializedType
        {
            [KvProperty(ArrayType = KvCollectionType.CommaSeparated)]
            public int[] Numbers { get; set; }
        }
    }
}
