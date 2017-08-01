using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using HealthTray.Service.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HealthTray.Service
{
    /// <summary>
    /// Retrieves <see cref="Check"/> objects from a JSON file.
    /// </summary>
    public class FileBasedHealthTrayService : IHealthTrayService
    {
        public string FileName { get; private set; }

        public FileBasedHealthTrayService(string fileName)
        {
            FileName = fileName;
        }

        public Task<IList<Check>> GetChecks()
        {
            string fileContents = File.ReadAllText(FileName);
            var checksObject = JsonConvert.DeserializeObject<JObject>(fileContents);
            var checks = checksObject["checks"]?.ToObject<IList<Check>>();
            return Task.FromResult(checks);
        }
    }
}
