using System.IO;
using NUnit.Framework;
using ValveKeyValue.Test.Test_Data;

namespace ValveKeyValue.Test.Text
{
    internal class SerializationTestCase
    {
        [Test]
        public void CreatesTextDocument()
        {
            var kv = new KvObject(
                "test data",
                new[]
                {
                    new KvObject(
                        "0",
                        new[]
                        {
                            new KvObject("description", "Dota 2 is a complex game where you get sworn at\nin Russian all the time."),
                            new KvObject("developer", "Valve Software"),
                            new KvObject("name", "Dota 2")
                        }),
                    new KvObject(
                        "1",
                        new[]
                        {
                            new KvObject("description", "Known as \"America's #1 war-themed hat simulator\", this game lets you wear stupid items while killing people."),
                            new KvObject("developer", "Valve Software"),
                            new KvObject("name", "Team Fortress 2")
                        })
                });

            string text;
            using (var ms = new MemoryStream())
            {
                KvSerializer.Create(KvSerializationFormat.KeyValues1Text).Serialize(ms, kv);

                ms.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(ms))
                    text = reader.ReadToEnd();
            }

            var expected = TestDataHelper.ReadTextResource("Text.serialization_expected.vdf");
            Assert.That(text, Is.EqualTo(expected));
        }
    }
}
