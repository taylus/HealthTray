using System.Threading.Tasks;
using System.Collections.Generic;
using HealthTray.Service.Model;

namespace HealthTray.Service
{
    /// <summary>
    /// Represents a type which retrieves <see cref="Check"/> objects.
    /// </summary>
    public interface IHealthTrayService
    {
        Task<IList<Check>> GetChecks();
        //void CreateCheck(string checkData);
        //void UpdateCheck(string checkData);
        //void PauseCheck(Guid checkId);
    }
}
