﻿using System.IO;
using System.Text;
using NUnit.Framework;
using ValveKeyValue.Test.Test_Data;

namespace ValveKeyValue.Test.Text
{
    internal class InvalidConditionalTestCase
    {
        [TestCase("$ABC | $DEF")]
        [TestCase("$ABC & $DEF")]
        [TestCase("$ABC &| $DEF")]
        [TestCase("$ABC |& $DEF")]
        [TestCase("$ABC ! $DEF")]
        [TestCase("!")]
        [TestCase("&&")]
        [TestCase("||")]
        [TestCase("()")]
        [TestCase("$ABC & ()")]
        [TestCase("$ABC && (!)")]
        [TestCase("$ABC && ($DEF!)")]
        [TestCase("(")]
        [TestCase(")")]
        [TestCase("$ABC && ($DEF || $GHI")]
        [TestCase("$ABC && $DEF)")]
        public void ThrowsInvalidDataException(string conditional)
        {
            var text = TestDataHelper.ReadTextResource("Text.invalid_conditional.vdf");
            text = text.Replace("{CONDITION}", conditional);

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            {
                Assert.That(
                    () => KvSerializer.Create(KvSerializationFormat.KeyValues1Text).Deserialize(stream),
                    Throws.Exception.InstanceOf<InvalidDataException>()
                    .With.Message.EqualTo($"Invalid conditional syntax \"{conditional}\""));
            }
        }
    }
}
