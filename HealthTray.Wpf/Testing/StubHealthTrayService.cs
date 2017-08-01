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
        private IList<Check> checks;

        public StubHealthTrayService(IList<Check> checks)
        {
            this.checks = checks;
        }

        public Task<IList<Check>> GetChecks()
        {
            return Task.FromResult(checks);
        }
    }
}
