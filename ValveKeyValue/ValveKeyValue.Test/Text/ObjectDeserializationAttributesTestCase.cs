﻿using NUnit.Framework;

namespace ValveKeyValue.Test
{
    class ObjectDeserializationAttributesTestCase
    {
        [Test]
        public void IsNotNull()
        {
            Assert.That(person, Is.Not.Null);
        }

        [Test]
        public void FirstName()
        {
            Assert.That(person.FirstName, Is.EqualTo("Bob"));
        }

        [Test]
        public void LastName()
        {
            Assert.That(person.LastName, Is.Null);
        }

        [Test]
        public void CanFixIt()
        {
            Assert.That(person.CanFixIt, Is.True);
        }

        Person person;

        [OneTimeSetUp]
        public void SetUp()
        {
            using (var stream = TestDataHelper.OpenResource("Text.object_person_attributes.vdf"))
            {
                person = KvSerializer.Create(KvSerializationFormat.KeyValues1Text).Deserialize<Person>(stream);
            }
        }

        class Person
        {
            [KvProperty("First Name")]
            public string FirstName { get; set; }

            [KvIgnore]
            public string LastName { get; set; }

            [KvProperty("Can Fix It")]
            public bool CanFixIt { get; set; }
        }
    }
}
