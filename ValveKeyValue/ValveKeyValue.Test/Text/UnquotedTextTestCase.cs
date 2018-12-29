﻿using NUnit.Framework;

namespace ValveKeyValue.Test
{
    class UnquotedTextTestCase
    {
        [Test]
        public void IsNotNull()
        {
            Assert.That(data, Is.Not.Null);
        }

        [Test]
        public void Name()
        {
            Assert.That(data.Name, Is.EqualTo("TestDocument"));
        }

        [Test]
        public void QuotedChildWithEdgeCaseValue()
        {
            Assert.That((string)data["QuotedChild"], Is.EqualTo(@"edge\ncase\""haha\\"""));
        }

        [TestCase("Key1", ExpectedResult = "Value1")]
        [TestCase("Key2", ExpectedResult = "Value2")]
        [TestCase("Key3", ExpectedResult = "Value3")]
        public string UnquotedChildValue(string key) => (string)data["UnquotedChild"][key];

        KvObject data;

        [OneTimeSetUp]
        public void SetUp()
        {
            using (var stream = TestDataHelper.OpenResource("Text.unquoted_document.vdf"))
            {
                data = KvSerializer.Create(KvSerializationFormat.KeyValues1Text).Deserialize(stream);
            }
        }
    }
}