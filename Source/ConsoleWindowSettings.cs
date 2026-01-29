using System;
using UnityEngine;

namespace SRH
{
    public class ConsoleWindowSettings : ScriptableObject
    {
        public event Action OnSettingChanged;
        
        [Header("Settings")]
        [SerializeField] private string consoleWindowName = "Console Debugger";
        [SerializeField] private ConsoleWindow.TargetBuild targetBuild = ConsoleWindow.TargetBuild.DEVELOPMENT;
        [SerializeField] private ConsoleWindow.StackTraceVisibility stackTraceVisibility = ConsoleWindow.StackTraceVisibility.EXCEPTION;
        [SerializeField] private bool includeLogType = false;
        [SerializeField] private bool includeTimestamp = true;

        [Header("Colours")]
        [SerializeField] private Color32 infoColour = Color.white;
        [SerializeField] private Color32 warningColour = Color.yellow;
        [SerializeField] private Color32 errorColour = Color.red;
        [SerializeField] private Color32 exceptionColour = Color.red;
        [SerializeField] private Color32 assertColour = Color.white;
        [SerializeField] private Color32 stackTraceColour = Color.red;

        public string ConsoleWindowName
        {
            get => consoleWindowName;
            set
            {
                if (consoleWindowName == value)
                    return;

                consoleWindowName = value;
                NotifySettingChange();
            }
        }

        public ConsoleWindow.TargetBuild TargetBuild
        {
            get => targetBuild;
            set
            {
                if (targetBuild == value)
                    return;

                targetBuild = value;
                NotifySettingChange();
            }
        }

        public ConsoleWindow.StackTraceVisibility StackTraceVisibility
        {
            get => stackTraceVisibility;
            set
            {
                if (stackTraceVisibility == value)
                    return;

                stackTraceVisibility = value;
                NotifySettingChange();
            }
        }

        public bool IncludeLogType
        {
            get => includeLogType;
            set
            {
                if (includeLogType == value)
                    return;

                includeLogType = value;
                NotifySettingChange();
            }
        }

        public bool IncludeTimestamp
        {
            get => includeTimestamp;
            set
            {
                if (includeTimestamp == value)
                    return;

                includeTimestamp = value;
                NotifySettingChange();
            }
        }

        public Color32 InfoColour
        {
            get => infoColour;
            set
            {
                if (infoColour.Equals(value))
                    return;

                infoColour = value;
                NotifySettingChange();
            }
        }

        public Color32 WarningColour
        {
            get => warningColour;
            set
            {
                if (warningColour.Equals(value))
                    return;

                warningColour = value;
                NotifySettingChange();
            }
        }

        public Color32 ErrorColour
        {
            get => errorColour;
            set
            {
                if (errorColour.Equals(value))
                    return;

                errorColour = value;
                NotifySettingChange();
            }
        }

        public Color32 ExceptionColour
        {
            get => exceptionColour;
            set
            {
                if (exceptionColour.Equals(value))
                    return;

                exceptionColour = value;
                NotifySettingChange();
            }
        }

        public Color32 AssertColour
        {
            get => assertColour;
            set
            {
                if (assertColour.Equals(value))
                    return;

                assertColour = value;
                NotifySettingChange();
            }
        }

        public Color32 StackTreeColour
        {
            get => stackTraceColour;
            set
            {
                if (stackTraceColour.Equals(value))
                    return;

                stackTraceColour = value;
                NotifySettingChange();
            }
        }

        private void NotifySettingChange()
        {
            OnSettingChanged?.Invoke();
        }
    }
}
