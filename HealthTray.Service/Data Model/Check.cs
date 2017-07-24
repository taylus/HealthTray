using System;
using Newtonsoft.Json;

namespace HealthTray.Service.Model
{
    public class Check
    {
        public string pause_url { get; set; }
        public string tags { get; set; }
        public string name { get; set; }
        public int timeout { get; set; }
        public int n_pings { get; set; }
        public string ping_url { get; set; }
        public DateTime? last_ping { get; set; }
        public string update_url { get; set; }
        public DateTime? next_ping { get; set; }
        public CheckStatus status { get; set; }
        public int grace { get; set; }
        public string tz { get; set; }
        public string schedule { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public TimeSpan? SinceLastPing(DateTime? currentTime = null)
        {
            if (last_ping == null) return null;
            if (currentTime == null) currentTime = DateTime.Now;
            return currentTime.Value - last_ping.Value;
        }
    }

    public enum CheckStatus
    {
        up,
        down,
        late,
        @new,
        paused
    }
}