using System.Collections;
using System.Linq;
using NUnit.Framework;

namespace ValveKeyValue.Test
{
    class KVValueToStringTestCase
    {
        [TestCaseSource(nameof(ToStringTestCases))]
        public string KVValueToStringIsSane(KvValue value) => value.ToString();

        public static IEnumerable ToStringTestCases
        {
            get
            {
                yield return new TestCaseData(new KvObject("a", "blah").Value).Returns("blah");
                yield return new TestCaseData(new KvObject("a", "yay").Value).Returns("yay");
                yield return new TestCaseData(new KvObject("a", Enumerable.Empty<KvObject>()).Value).Returns("[Collection]").SetName("{m} - Empty Collection");
                yield return new TestCaseData(new KvObject("a", new[] { new KvObject("boo", "aah") }).Value).Returns("[Collection]").SetName("{m} - Collection With Value");
            }
        }
    }
}
