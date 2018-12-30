using NUnit.Framework;
using ValveKeyValue.Test.Test_Data;

namespace ValveKeyValue.Test.Text
{
    internal class EscapedWhitespaceTestCase
    {
        [Test]
        public void ConvertsBackslashCharToActualRepresentation()
        {
            Assert.That((string)_data["key"], Is.EqualTo("line1\nline2\tline2pt2"));
        }

        private KvObject _data;

        [OneTimeSetUp]
        public void SetUp()
        {
            var options = new KvSerializerOptions { HasEscapeSequences = true };
            using (var stream = TestDataHelper.OpenResource("Text.escaped_whitespace.vdf"))
                _data = KvSerializer.Create(KvSerializationFormat.KeyValues1Text).Deserialize(stream, options);
        }
    }
}
