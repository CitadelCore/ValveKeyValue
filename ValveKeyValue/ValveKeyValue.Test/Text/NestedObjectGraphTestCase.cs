using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using ValveKeyValue.Test.Test_Data;

namespace ValveKeyValue.Test.Text
{
    internal class NestedObjectGraphTestCase
    {
        [Test]
        public void IsNotNull()
        {
            Assert.That(_data, Is.Not.Null);
        }

        [Test]
        public void OgInt()
        {
            Assert.That(_data.OgInt, Is.EqualTo(3));
        }

        [Test]
        public void FooObj()
        {
            Assert.That(_data.FooObj, Is.Not.Null);
        }

        [Test]
        public void FooStr()
        {
            Assert.That(_data.FooObj?.FooStr, Is.EqualTo("blah"));
        }

        [Test]
        public void BarObj()
        {
            Assert.That(_data.FooObj?.BarObj, Is.Not.Null);
        }

        [Test]
        public void Baz()
        {
            Assert.That(_data.FooObj?.BarObj?.Baz, Is.EqualTo("blahdiladila"));
        }

        private ObjectGraph _data;

        [OneTimeSetUp]
        public void SetUp()
        {
            using (var stream = TestDataHelper.OpenResource("Text.nested_object_graph.vdf"))
            {
                _data = KvSerializer.Create(KvSerializationFormat.KeyValues1Text).Deserialize<ObjectGraph>(stream);
            }
        }

        
        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        private class ObjectGraph
        {
            public int OgInt { get; set; }
            public Foo FooObj { get; set; }

            public class Foo
            {
                public string FooStr { get; set; }
                public Bar BarObj { get; set; }

                public class Bar
                {
                    public string Baz { get; set; }
                }
            }
        }
    }
}
