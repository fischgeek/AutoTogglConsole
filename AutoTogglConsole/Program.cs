using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static SharedLibrary.ConsoleShortcuts;

namespace AutoTogglConsole
{
    class Program : _Base
    {
        private static TogglBase tb = TogglBase.GetInstance();
        public static string lastActive = string.Empty;
        public static bool idle = false;
        public static bool aTimerIsRunning = false;
        //public static Project custerProject = new Project() {
        //    name = "Custer Health"
        //    , pid = 148463318
        //    , keywords = new List<string>() {
        //        "custer"
        //        , "ndphsoft"
        //        , "web50"
        //    }
        //};
        //public static Project spsProject = new Project() {
        //    name = "SPS"
        //    , pid = 154608648
        //    , keywords = new List<string>() {
        //        "sps"
        //    }
        //};
        //public static Project timsProject = new Project() {
        //    name = "TIMS"
        //    , pid = 154608855
        //    , keywords = new List<string>() {
        //        "tims"
        //    }
        //};
        public static Project wcriProject = new Project() {
            name = "WCRI"
            , pid = 155152245
            , keywords = new List<string>() {
                "wcri"
                , "western capital"
                , "forticlient"
                , "cricket"
                , "service desk"
            }
        };
        //public static Project jiraProject = new Project() {
        //    name = "Jira"
        //    , pid = 154613363
        //    , keywords = new List<string>() {
        //        "service desk"
        //        , "wcridesk"
        //        , "termination"
        //    }
        //};
        //public static Project devTechProject = new Project() {
        //    name = "DevTech"
        //    , pid = 154626682
        //    , keywords = new List<string>() {
        //        "devtech"
        //    }
        //};
        public static List<string> neutralWindows = new List<string>() {
            "Task Switching"
            , "Windows PowerShell"
            , "Administrator: Windows PowerShell"
            , "Administration - Google Chrome"
            , "People - Jira - Google Chrome"
        };
        public static List<Project> projects = new List<Project>() {
            //timsProject
            //, spsProject
            //, custerProject
            //jiraProject
            wcriProject
            //, devTechProject
        };

        static void Main(string[] args)
        {
            handler = new ConsoleEventDelegate(ConsoleEventCallback);
            SetConsoleCtrlHandler(handler, true);
            tb.Init(JFUtil.Base64Encode("547b7d7d5e9b3bee1c880716e0840035:api_token"));
            CheckForARunningTimer();
            while (true) {
                CheckIdleTime();
                CheckActiveWindow();
                System.Threading.Thread.Sleep(5000);
            }
        }

        private static void CheckForARunningTimer()
        {
            var te = tb.GetRunningTimer();
            if (te != null) {
                clt($"Current running timer is {te.description}. Duration {tb.CurrentTimerDuration()}");
                aTimerIsRunning = true;
            }
        }

        private static void CheckActiveWindow()
        {
            if (!idle) {
                var currentActive = GetActiveWindowTitle();
                if (CurrentActiveIsValid(currentActive)) {
                    clt(currentActive);
                    var anyMatches = false;
                    foreach (var project in projects) {
                        if (KeywordExistsInActiveWindowTitle(project, currentActive)) {
                            StartTimer(project, currentActive);
                            anyMatches = true;
                            break;
                        }
                    }
                    if (!anyMatches && aTimerIsRunning) {
                        clt("Window doesn't match any keywords in any projects. Stopping timer.");
                        tb.StopRunningTimer();
                        aTimerIsRunning = false;
                    }
                    lastActive = currentActive;
                }
            }
        }

        private static bool CurrentActiveIsValid(string currentActive) => lastActive != currentActive && currentActive.JFIsNotNull() && !neutralWindows.Contains(currentActive);

        private static bool KeywordExistsInActiveWindowTitle(Project project, string currentActive)
        {
            foreach (var keyword in project.keywords) {
                var match = Regex.Match(currentActive, keyword, RegexOptions.IgnoreCase);
                if (match.Success) {
                    return true;
                }
            }
            return false;
        }

        private static void CheckIdleTime()
        {
            if (GetLastInputTime() >= 60) {
                if (!idle) {
                    clt("System idle. Stopping timer.");
                    tb.StopRunningTimer();
                    idle = true;
                    lastActive = string.Empty;
                    aTimerIsRunning = false;
                }
            } else {
                idle = false;
            }
        }

        private static void StartTimer(Project project, string description = "")
        {
            var ct = tb.GetRunningTimer();
            if (ct != null && ct.pid == project.pid && ct.description == description) {
                clt($"A timer is already running for {project.name}.");
            } else {
                TimeEntryWrapper wrapper = new TimeEntryWrapper();
                wrapper.time_entry = new TimeEntry() {
                    description = description
                    , wid = 3757896
                    , pid = project.pid
                    , created_with = ".net"
                };
                tb.StartTimer(wrapper);
                clt($"Tracking started for {project.name}.");
            }
            aTimerIsRunning = true;
        }
    }
}
