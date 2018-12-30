using System.Collections.Generic;
using NUnit.Framework;

namespace ValveKeyValue.Test.Text
{
    [TestFixture(typeof(StreamKvTextReader))]
    [TestFixture(typeof(StringKvTextReader))]
    internal class DictionaryTestCase<TReader>
        where TReader : IKvTextReader, new()
    {
        [Test]
        public void IsNotNull()
        {
            Assert.That(_data, Is.Not.Null);
        }

        [Test]
        public void IsNotEmpty()
        {
            Assert.That(_data, Is.Not.Empty);
        }

        [TestCase("FirstName", ExpectedResult = "Bob")]
        [TestCase("LastName", ExpectedResult = "Builder")]
        [TestCase("CanFixIt", ExpectedResult = "1")]
        public string HasValueForKey(string key) => _data[key];

        private Dictionary<string, string> _data;

        [OneTimeSetUp]
        public void SetUp()
        {
            _data = new TReader().Read<Dictionary<string, string>>("Text.object_person.vdf");
        }
    }
}
