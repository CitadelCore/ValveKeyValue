using System.Collections;
using System.Globalization;
using System.Linq;
using NUnit.Framework;

namespace ValveKeyValue.Test
{
    internal class KvValueToStringTestCase
    {
        [TestCaseSource(nameof(ToStringTestCases))]
        public string KvValueToStringIsSane(KvValue value) => value.ToString(CultureInfo.InvariantCulture);

        private static IEnumerable ToStringTestCases
        {
            // ReSharper disable once UnusedMember.Local
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
