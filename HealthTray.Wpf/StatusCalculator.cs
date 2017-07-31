using System.Linq;
using System.Collections.Generic;
using HealthTray.Service.Model;

namespace HealthTray.Wpf
{
    public static class StatusCalculator
    {
        /// <summary>
        /// Summarizes and returns an "overall status" based on the given checks.
        /// E.g. up if everything is up, but down if anything is down, etc.
        /// </summary>
        public static CheckStatus CalculateOverallStatusFrom(IList<Check> checks)
        {
            if (checks == null || checks.Count == 0) return CheckStatus.@new;
            if (checks.All(c => c.status == CheckStatus.up)) return CheckStatus.up;
            if (checks.Any(c => c.status == CheckStatus.down)) return CheckStatus.down;
            if (checks.Any(c => c.status == CheckStatus.late)) return CheckStatus.late;
            if (checks.Any(c => c.status == CheckStatus.paused)) return CheckStatus.paused;
            return CheckStatus.@new;
        }
    }
}
