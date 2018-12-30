﻿using System.Collections;
using System.Linq;
using NUnit.Framework;

namespace ValveKeyValue.Test.Text
{
    [TestFixture(typeof(StreamKvTextReader))]
    [TestFixture(typeof(StringKvTextReader))]
    internal class LegacyDepotDataSubsetTestCase<TReader>
        where TReader : IKvTextReader, new()
    {
        [Test]
        public void IsNotNull()
        {
            Assert.That(_data, Is.Not.Null);
        }

        [Test]
        public void Name()
        {
            Assert.That(_data.Name, Is.EqualTo("depots"));
        }

        [Test]
        public void HasFourItems()
        {
            Assert.That(_data.Children.Count(), Is.EqualTo(4));
        }

        [TestCaseSource(nameof(ItemsTestCaseData))]
        public void HasItems(string key, string expectedValue)
        {
            var value = _data[key];
            Assert.That((string)value, Is.EqualTo(expectedValue));
        }

        [TestCaseSource(nameof(ItemsTestCaseData))]
        public void HasItemWithValueCast(string key, string expectedValue)
        {
            var value = _data[key];
            Assert.That((string)value, Is.EqualTo(expectedValue));
        }

        [TestCaseSource(nameof(GarbageItemsTestCaseData))]
        public void DoesNotHaveGarbageItems(string key)
        {
            var value = _data[key];
            Assert.That(value, Is.Null);
        }

        [TestCaseSource(nameof(GarbageItemsTestCaseData))]
        public void DoesNotHaveGarbageWithValueCast(string key)
        {
            var value = _data[key];
            Assert.That((string)value, Is.Null);
        }

        private static IEnumerable ItemsTestCaseData
        {
            // ReSharper disable once UnusedMember.Local
            get
            {
                yield return new TestCaseData("0", "A10D0BCD94CE6105D0E2256FE06B2B22");
                yield return new TestCaseData("1", "60EF870FB4B9A2EBB5E511BC8CEC8858");
                yield return new TestCaseData("3", "AA4CA0B6300E96774BC3C2B55C3388C9");
                yield return new TestCaseData("7", "636DC4B351732EDC6022021B89124CC9");
            }
        }

        private static IEnumerable GarbageItemsTestCaseData
        {
            // ReSharper disable once UnusedMember.Local
            get
            {
                yield return "2";
                yield return "asdasd";
                yield return "123";
                yield return "hello there";
            }
        }

        private KvObject _data;

        [OneTimeSetUp]
        public void SetUp()
        {
            _data = new TReader().Read("Text.legacydepotdata_subset.vdf");
        }
    }
}
