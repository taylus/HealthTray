using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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

    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CheckStatus
    {
        up,
        down,
        [EnumMember(Value = "grace")]       //serialize "late" and "grace" as late -- https://github.com/taylus/HealthTray/issues/1
        late,
        [EnumMember(Value = "started")]     //serialize "started" and "new" as "new" -- https://github.com/taylus/HealthTray/issues/3
        @new,
        paused
    }
}