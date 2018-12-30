﻿using System.IO;
using System.Reflection;
using System.Text;

namespace ValveKeyValue.Test.Test_Data
{
    internal static class TestDataHelper
    {
        public static Stream OpenResource(string name)
        {
            var resourceName = "ValveKeyValue.Test.Test_Data." + name;
            var stream = typeof(TestDataHelper).GetTypeInfo().Assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
                throw new FileNotFoundException("Embedded Resource not found.", resourceName);

            return stream;
        }

        public static string ReadTextResource(string name)
        {
            var builder = new StringBuilder();

            using (var stream = OpenResource(name))
            using (var reader = new StreamReader(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    builder.Append(line);
                    builder.Append("\n");
                }
            }

            return builder.ToString();
        }
    }
}
