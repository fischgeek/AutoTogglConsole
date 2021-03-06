﻿using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using TogglConnect;
using static SharedLibrary.ConsoleShortcuts;

namespace AutoTogglConsole
{
    class Program : _Base
    {
        private static TogglBase tb = TogglBase.GetInstance();
        public static int WorkspaceId;
        public static string lastActive = string.Empty;
        public static bool idle = false;
        public static bool aTimerIsRunning = false;
        public static List<Project> AllProjects = new List<Project>();

        private static List<Project> GetProjectsFromAppSettings()
        {
            if (AllProjects.Count == 0) {
                foreach (string item in ConfigurationManager.AppSettings.Keys) {
                    var projectId = 0;
                    if (Int32.TryParse(item, out projectId)) {
                        var p = tb.GetProject(projectId);
                        p.keywords = ConfigurationManager.AppSettings[item].Split(',').ToList();
                        AllProjects.Add(p);
                    }
                }
                cl($"Collected projects: " + AllProjects.Count());
            }
            return AllProjects;
        }

        static void Main(string[] args)
        {
            EnsureSingleInstance();
            if (ConfigurationManager.AppSettings == null || ConfigurationManager.AppSettings.Count == 0) {
                Console.WriteLine("Application settings are missing.");
                Console.ReadLine();
            } else {
                handler = new ConsoleEventDelegate(ConsoleEventCallback);
                SetConsoleCtrlHandler(handler, true);
                WorkspaceId = ConfigurationManager.AppSettings["WorkspaceID"].JFStringToInt();
                tb.Init(ConfigurationManager.AppSettings["apiKey"], WorkspaceId);
                GetProjectsFromAppSettings();
                CheckForARunningTimer();
                while (true) {
                    CheckIdleTime();
                    CheckActiveWindow();
                    System.Threading.Thread.Sleep(ConfigurationManager.AppSettings["delay"].JFStringToInt());
                }
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
                    foreach (var project in GetProjectsFromAppSettings()) {
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

        public static bool IsNeutralWindow(string title) => Regex.IsMatch(title, ConfigurationManager.AppSettings["NeutralWindowRegex"], RegexOptions.IgnoreCase);

        private static bool CurrentActiveIsValid(string currentActive) => lastActive != currentActive && currentActive.JFIsNotNull() && !IsNeutralWindow(currentActive);

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
            if (ct != null && ct.pid == project.id && ct.description == description) {
                clt($"A timer is already running for {project.name}.");
            } else {
                TimeEntryWrapper wrapper = new TimeEntryWrapper();
                wrapper.time_entry = new TimeEntry() {
                    description = description
                    , wid = int.Parse(ConfigurationManager.AppSettings["WorkspaceID"])
                    , pid = project.id
                    , created_with = ".net"
                };
                tb.StartTimer(wrapper);
                //logTimerStart(project.name); // optional logging
                clt($"Tracking started for {project.name}.");
            }
            aTimerIsRunning = true;
        }

        private static void logTimerStart(string name)
        {
            try {
                System.IO.File.AppendAllLines(@"c:\temp\AutoTogglConsole_recent.txt", new string[] { name });
            } catch {
            }
        }
    }
}
