using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace ValveKeyValue.Test.Text
{
    [TestFixture(typeof(StreamKvTextReader))]
    [TestFixture(typeof(StringKvTextReader))]
    internal class ObjectDeserializationTestCase<TReader>
        where TReader : IKvTextReader, new()
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
            Assert.That(_person.LastName, Is.EqualTo("Builder"));
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
            _person = new TReader().Read<Person>("Text.object_person.vdf");
        }

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
        private class Person
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public bool CanFixIt { get; set; }
        }
    }
}
