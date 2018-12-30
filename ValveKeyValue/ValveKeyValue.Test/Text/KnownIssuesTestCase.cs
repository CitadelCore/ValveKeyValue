using System.Collections.Generic;
using NUnit.Framework;
using ValveKeyValue.Test.Test_Data;

namespace ValveKeyValue.Test.Text
{
    internal class KnownIssuesTestCase
    {
        [Test]
        public void CanDeserializeValveResourceFormatSettings()
        {
            VkvConfig config;

            using (var stream = TestDataHelper.OpenResource("Text.vrf_settings_sample.vdf"))
            {
                config = KvSerializer.Create(KvSerializationFormat.KeyValues1Text).Deserialize<VkvConfig>(stream);
            }

            Assert.That(config, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(config.BackgroundColor, Is.EqualTo("#3C3C3C"));
                Assert.That(config.OpenDirectory, Is.EqualTo(@"D:\SteamLibrary\steamapps\common\The Lab\RobotRepair\vr"));
                Assert.That(config.SaveDirectory, Is.EqualTo(@"D:\SteamLibrary\steamapps\common\The Lab\RobotRepair\vr"));
                Assert.That(config.GameSearchPaths, Is.Not.Null & Has.Count.Zero);
            });
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class VkvConfig
        {
            public List<string> GameSearchPaths { get; set; }
            public string BackgroundColor { get; set; }
            public string OpenDirectory { get; set; }
            public string SaveDirectory { get; set; }
        }
    }
}