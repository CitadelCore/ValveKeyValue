using NUnit.Framework;
using ValveKeyValue.Test.Test_Data;

namespace ValveKeyValue.Test.Text
{
    internal class SingleLineCommentTestCase
    {
        [TestCase("comment_singleline")]
        [TestCase("comment_singleline_wholeline")]
        [TestCase("comment_singleline_singleslash")]
        [TestCase("comment_singleline_singleslash_wholeline")]
        public void SingleLineComment(string resourceName)
        {
            using (var stream = TestDataHelper.OpenResource("Text." + resourceName + ".vdf"))
                Assert.That(() => KvSerializer.Create(KvSerializationFormat.KeyValues1Text).Deserialize(stream), Throws.Nothing);
        }
    }
}
