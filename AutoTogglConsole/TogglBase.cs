using Newtonsoft.Json;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SharedLibrary.ConsoleShortcuts;

namespace AutoTogglConsole
{
    public class TogglBase
    {
        public static TogglBase Instance = new TogglBase();

        private TogglBase() { }

        public static TogglBase GetInstance()
        {
            return Instance;
        }

        private static string baseUrl = "https://www.toggl.com/api/v8";
        public string apiKey { get; set; }

        public void Init(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public TimeEntry GetRunningTimer()
        {
            var x = WebCall.GetRequestWithErrorHandling($"{baseUrl}/time_entries/current", this.apiKey);
            if (x.IsSuccessful) {
                var t = JsonConvert.DeserializeObject<TimeEntryData>(x.Data);
                var timeEntry = t.data;
                return timeEntry;
            } else {
                return null;
            }
        }

        public void StartTimer(TimeEntryWrapper wrapper)
        {
            var url = $"{baseUrl}/time_entries/start";
            var x = WebCall.PostRequestWithErrorHandling(url, wrapper, this.apiKey);
        }

        public void StopRunningTimer()
        {
            var te = GetRunningTimer();
            if (te != null) {
                var url = $"{baseUrl}/time_entries/{te.id}/stop";
                WebCall.PutRequest(url, basicAuth: this.apiKey);
            }
        }

        public string CurrentTimerDuration()
        {
            var ct = GetRunningTimer();
            if (ct != null) {
                var seconds = JFUtil.SecondsSinceEpoch() + ct.duration;
                var ts = new TimeSpan(0, 0, seconds);
                return $"{ts.Days}d {ts.Hours}h {ts.Minutes}m {ts.Seconds}s";
            }
            return "00:00:00";
        }
    }
}
