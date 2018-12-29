﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ValveKeyValue.Test
{
    class KVObjectIndexerTestCase
    {
        [TestCase("foo", ExpectedResult = "bar")]
        [TestCase("bar", ExpectedResult = "baz")]
        [TestCase("baz", ExpectedResult = "-")]
        [TestCase("foobar", ExpectedResult = null)]
        public string IndexerReturnsChildValue(string key) => (string)data[key];

        [Test]
        public void IndexerOnValueNodeThrowsException()
        {
            Assert.That(
                () => data["foo"]["bar"],
                Throws.Exception.InstanceOf<NotSupportedException>()
                .With.Message.EqualTo("The indexer on a KVValue can only be used on a KVValue that has children."));
        }

        KvObject data;

        [OneTimeSetUp]
        public void SetUp()
        {
            data = new KvObject(
                "test data",
                new[]
                {
                    new KvObject("foo", "bar"),
                    new KvObject("bar", "baz"),
                    new KvObject("baz", "-"),
                });
        }
    }
}
