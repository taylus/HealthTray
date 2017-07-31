using System.Threading.Tasks;
using System.Collections.Generic;
using HealthTray.Service.Model;

namespace HealthTray.Service
{
    /// <summary>
    /// Retrieves <see cref="Check"/> objects from an in-memory list.
    /// </summary>
    public class StubHealthTrayService : IHealthTrayService
    {
        public IList<Check> Checks { get; private set; }

        public StubHealthTrayService(IList<Check> checks)
        {
            Checks = checks;
        }

        public Task<IList<Check>> GetChecks()
        {
            return Task.FromResult(Checks);
        }
    }
}
