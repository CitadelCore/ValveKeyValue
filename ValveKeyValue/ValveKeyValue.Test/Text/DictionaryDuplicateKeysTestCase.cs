﻿using System.Collections.Generic;
using NUnit.Framework;

namespace ValveKeyValue.Test
{
    class DictionaryDuplicateKeysTestCase
    {
        [Test]
        public void IsNotNull()
        {
            Assert.That(data, Is.Not.Null);
        }

        [Test]
        public void IsNotEmpty()
        {
            Assert.That(data, Is.Not.Empty);
        }

        [Test]
        public void LateValueOverridesEarlyValue()
        {
            Assert.That(data["foo"], Is.EqualTo("baz"));
        }

        Dictionary<string, string> data;

        [OneTimeSetUp]
        public void SetUp()
        {
            using (var stream = TestDataHelper.OpenResource("Text.duplicate_keys.vdf"))
            {
                data = KvSerializer.Create(KvSerializationFormat.KeyValues1Text).Deserialize<Dictionary<string, string>>(stream);
            }
        }
    }
}
