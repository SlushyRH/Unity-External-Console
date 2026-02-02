using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace SRH
{
    public static class ConsoleWindow
    {
        [Flags]
        public enum TargetBuild
        {
            NONE = 0,
            DEVELOPMENT = 1 << 0,
            RELEASE = 1 << 1,
        }

        [Flags]
        public enum StackTraceVisibility
        {
            NONE = 0,
            INFO = 1 << 0,
            WARNING = 1 << 1,
            ERROR = 1 << 2,
            EXCEPTION = 1 << 4,
            ASSERT = 1 << 8,
        }

        private const string RESOURCES_PATH = "SRH/External Console Window/";

        private static ConsoleColor _ccInfoTxtColour;
        private static ConsoleColor _ccWarningTxtColour;
        private static ConsoleColor _ccErrorTxtColour;
        private static ConsoleColor _ccExceptionTxtColour;
        private static ConsoleColor _ccAssertTxtColour;
        private static ConsoleColor _ccStackTraceTxtColour;

        private static StreamWriter _writer;
        private static bool _consoleAllocated;
        private static readonly object _lock = new object();
        
        private static Dictionary<ConsoleColor, Color32> _consoleColours = new Dictionary<ConsoleColor, Color32>()
        {
            { ConsoleColor.Black, new Color32(0, 0, 0, 255) },
            { ConsoleColor.DarkBlue, new Color32(0, 0, 139, 255) },
            { ConsoleColor.DarkGreen, new Color32(0, 100, 0, 255) },
            { ConsoleColor.DarkCyan, new Color32(0, 139, 139, 255) },
            { ConsoleColor.DarkRed, new Color32(139, 0, 0, 255) },
            { ConsoleColor.DarkMagenta, new Color32(139, 0, 139, 255) },
            { ConsoleColor.DarkYellow, new Color32(184, 134, 11, 255) },
            { ConsoleColor.Gray, new Color32(190, 190, 190, 255) },
            { ConsoleColor.DarkGray, new Color32(105, 105, 105, 255) },
            { ConsoleColor.Blue, new Color32(0, 0, 255, 255) },
            { ConsoleColor.Green, new Color32(0, 255, 0, 255) },
            { ConsoleColor.Cyan, new Color32(0, 255, 255, 255) },
            { ConsoleColor.Red, new Color32(255, 0, 0, 255) },
            { ConsoleColor.Magenta, new Color32(255, 0, 255, 255) },
            { ConsoleColor.Yellow, new Color32(255, 255, 0, 255) },
            { ConsoleColor.White, new Color32(255, 255, 255, 255) },
        };
        
        private static ConsoleWindowSettings _settings;
        public static ConsoleWindowSettings Settings
        {
            get
            {
                if (_settings != null)
                    return _settings;

                _settings = Resources.Load<ConsoleWindowSettings>(RESOURCES_PATH + "Console Window Settings");

                if (_settings == null)
                {
                    _settings = ScriptableObject.CreateInstance<ConsoleWindowSettings>();
                    _settings.name = "Console Window Settings (Default)";
                }

                return _settings;
            }
        }
        
        public static bool Pause { get; set; }
        
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool SetConsoleTitle(string lpConsoleTitle);
        
        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        private static extern bool DeleteMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        private const uint SC_CLOSE = 0xF060;
        private const uint MF_BYCOMMAND = 0x00000000;
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void Init()
        {
#if UNITY_EDITOR || !UNITY_STANDALONE_WIN || DISABLE_EXTERN_CMD
            return;
#endif
            
            if (!IsBuildAllowed())
                return;
            
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            AllocConsole();
            _consoleAllocated = true;
            
            IntPtr hwnd = GetConsoleWindow();
            if (hwnd != IntPtr.Zero)
            {
                IntPtr hMenu = GetSystemMenu(hwnd, false);
                if (hMenu != IntPtr.Zero)
                {
                    DeleteMenu(hMenu, SC_CLOSE, MF_BYCOMMAND);
                }
            }
#endif

            _writer = new StreamWriter(Console.OpenStandardError())
            {
                AutoFlush = true
            };
            Console.SetOut(_writer);
            
            OnSettingsChanged();

            Application.logMessageReceived += HandleLog;
            Settings.OnSettingChanged += OnSettingsChanged;
            
            GameObject bridge = new GameObject("Console Cleanup Bridge");
            bridge.AddComponent<ConsoleCleanupBridge>();
            bridge.hideFlags = HideFlags.HideAndDontSave;
            UnityEngine.Object.DontDestroyOnLoad(bridge);
            
            Debug.Log("External Console Window initialised!");
        }

        private static void OnSettingsChanged()
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            if (!string.IsNullOrEmpty(Settings.ConsoleWindowName))
                SetConsoleTitle(Settings.ConsoleWindowName);
#endif
            
            _ccInfoTxtColour = GetClosestConsoleColour(Settings.InfoColour);
            _ccWarningTxtColour = GetClosestConsoleColour(Settings.WarningColour);
            _ccErrorTxtColour = GetClosestConsoleColour(Settings.ErrorColour);
            _ccExceptionTxtColour = GetClosestConsoleColour(Settings.ExceptionColour);
            _ccAssertTxtColour = GetClosestConsoleColour(Settings.AssertColour);
            _ccStackTraceTxtColour = GetClosestConsoleColour(Settings.StackTreeColour);
        }

        private static ConsoleColor GetClosestConsoleColour(Color32 colour)
        {
            ConsoleColor consoleColour = ConsoleColor.White;
            double smallestDist = double.MaxValue;

            foreach (var cc in _consoleColours)
            {
                double distance = Math.Pow(cc.Value.r - colour.r, 2.0) + Math.Pow(cc.Value.g - colour.g, 2.0) + Math.Pow(cc.Value.b - colour.b, 2.0);

                if (distance < smallestDist)
                {
                    smallestDist = distance;
                    consoleColour = cc.Key;
                }
            }

            return consoleColour;
        }
        
        private static bool IsBuildAllowed()
        {
#if DISABLE_EXTERN_CMD
            return false;
#elif DEVELOPMENT_BUILD
            return Settings.TargetBuild.HasFlag(TargetBuild.DEVELOPMENT);
#else
            return Settings.TargetBuild.HasFlag(TargetBuild.RELEASE);
#endif
        }

        internal static void Cleanup()
        {
            Application.logMessageReceived -= HandleLog;

            lock (_lock) 
            {
                if (_writer != null)
                {
                    _writer.Flush();
                    _writer.Close();
                    _writer = null;
                }

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
                if (_consoleAllocated)
                {
                    FreeConsole();
                    _consoleAllocated = false;
                }
#endif
            }
        }

        private static void HandleLog(string msg, string stackTrace, LogType type)
        {
            var writerCopy = _writer;
            if (Pause || writerCopy == null)
                return;

            ConsoleColor ogColour = Console.ForegroundColor;
            bool showStack = false;

            switch (type)
            {
                case LogType.Log:
                {
                    Console.ForegroundColor = _ccInfoTxtColour;

                    if (Settings.StackTraceVisibility.HasFlag(StackTraceVisibility.INFO))
                        showStack = true;
                    break;
                }
                case LogType.Warning:
                {
                    Console.ForegroundColor = _ccWarningTxtColour;

                    if (Settings.StackTraceVisibility.HasFlag(StackTraceVisibility.WARNING))
                        showStack = true;
                    break;
                }
                case LogType.Error:
                {
                    Console.ForegroundColor = _ccErrorTxtColour;

                    if (Settings.StackTraceVisibility.HasFlag(StackTraceVisibility.ERROR))
                        showStack = true;
                    break;
                }
                case LogType.Exception:
                {
                    Console.ForegroundColor = _ccExceptionTxtColour;

                    if (Settings.StackTraceVisibility.HasFlag(StackTraceVisibility.EXCEPTION))
                        showStack = true;
                    break;
                }
                case LogType.Assert:
                {
                    Console.ForegroundColor = _ccAssertTxtColour;

                    if (Settings.StackTraceVisibility.HasFlag(StackTraceVisibility.ASSERT))
                        showStack = true;
                    break;
                }
                default:
                    Console.ForegroundColor = ogColour;
                    break;
            }

            string logType = Settings.IncludeLogType ? $"[{type.ToString()}] " : string.Empty;
            string timestamp = Settings.IncludeTimestamp ? $"[{DateTime.Now:HH:mm:ss}] " : string.Empty;

            lock (_lock)
            {
                if (_writer == null)
                    return;
                
                _writer.WriteLine($"{logType}{timestamp}{msg}");

                if (showStack && !string.IsNullOrEmpty(stackTrace))
                {
                    Console.ForegroundColor = _ccStackTraceTxtColour;
                    _writer.WriteLine(stackTrace);
                }
            }

            Console.ForegroundColor = ogColour;
        }
    }
}