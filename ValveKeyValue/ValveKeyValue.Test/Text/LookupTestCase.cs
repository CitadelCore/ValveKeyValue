using System.Linq;
using NUnit.Framework;
using ValveKeyValue.Test.Helpers;
using ValveKeyValue.Test.Test_Data;

namespace ValveKeyValue.Test.Text
{
    internal class LookupTestCase
    {
        [Test]
        public void IsNotNullOrEmpty()
        {
            Assert.That(_data, Is.Not.Null.Or.Empty);
        }

        [Test]
        public void LookupIsNotNull()
        {
            Assert.That(_data.FooLookup, Is.Not.Null);
        }

        [Test]
        public void LookupHasTwoItems()
        {
            Assert.That(_data.FooLookup, Has.Count.EqualTo(2));
        }

        [TestCase("Foo", new string[] { "I am Foo." })]
        [TestCase("Bar", new string[] { "First Bar", "Second Bar", "Third Bar" })]
        public void LookupItems(string key, string[] expectedValues)
        {
            var lookupValues = _data.FooLookup[key].ToArray();
            Assert.That(lookupValues, Is.EquivalentTo(expectedValues));
        }

        private ContainerClass _data;

        [OneTimeSetUp]
        public void SetUp()
        {
            _data = KvSerializer.Create(KvSerializationFormat.KeyValues1Text).Deserialize<ContainerClass>(TestDataHelper.ReadTextResource("Text.duplicate_keys_object.vdf"));
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class ContainerClass
        {
            public ILookup<string, string> FooLookup { get; set; }
        }
    }
}
