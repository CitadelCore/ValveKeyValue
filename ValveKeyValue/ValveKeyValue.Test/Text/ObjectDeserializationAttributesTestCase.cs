using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using ValveKeyValue.Test.Test_Data;

namespace ValveKeyValue.Test.Text
{
    internal class ObjectDeserializationAttributesTestCase
    {
        [Test]
        public void IsNotNull()
        {
            Assert.That(_person, Is.Not.Null);
        }

        [Test]
        public void FirstName()
        {
            Assert.That(_person.FirstName, Is.EqualTo("Bob"));
        }

        [Test]
        public void LastName()
        {
            Assert.That(_person.LastName, Is.Null);
        }

        [Test]
        public void CanFixIt()
        {
            Assert.That(_person.CanFixIt, Is.True);
        }

        private Person _person;

        [OneTimeSetUp]
        public void SetUp()
        {
            using (var stream = TestDataHelper.OpenResource("Text.object_person_attributes.vdf"))
                _person = KvSerializer.Create(KvSerializationFormat.KeyValues1Text).Deserialize<Person>(stream);
        }

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
        private class Person
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
