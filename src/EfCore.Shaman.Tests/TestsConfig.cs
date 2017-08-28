using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EfCore.Shaman.Tests
{
    public class TestsConfig
    {
        public static TestsConfig Load(string fn = null)
        {
            if (string.IsNullOrEmpty(fn))
                fn = DefaultFileName;
            TestsConfig result;
            if (!File.Exists(fn))
            {
                result = new TestsConfig
                {
                    ConnectionStringTemplate =
                        "Server=(localdb)\\ShamanTests;Database=ShamanTests_;Integrated Security=true"
                };
                result.Save(fn);
                return result;
            }
            string json = File.ReadAllText(fn);
            result = JsonConvert.DeserializeObject<TestsConfig>(json);          
            return result;
        }

        private static string DefaultFileName
        {
            get
            {
                var fn = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "EfCoreShaman",
                    "TestsConfig.json"
                );
                return fn;
            }
        }

        private void Save(string fn)
        {
            new FileInfo(fn).Directory.Create();
            File.WriteAllText(fn, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public string ConnectionStringTemplate { get; set; }
    }
}
