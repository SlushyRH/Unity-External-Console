<h1 align="center">Unity External Console</h1>
A lightweight external CMD console for Unity that mirrors the `Debug` logging in real-time. It features automatic injection and
a settings menu under `Tools/SRH/External Console Settings`. Perfect for off-screen debugging in multi-monitor environments.

<img src="https://github.com/SlushyRH/Unity-External-Console/blob/main/readme/Showcase.gif" alt="External Console Showcase" align="center">

[!WARNING]
The console will only open in a Windows OS build. If it is not, then the console simply won't show, but your game will run as normal.

# How To Use
1. Install Unity External Console by adding package using Git URL:
```
https://github.com/SlushyRH/Unity-External-Console.git
```
2. Done! You can optionally edit the settings at `Tools/SRH/External Console Settings`

[!INFO]
If at any point you want to disable the External Console window, you can either set the
`Target Build` to NONE, or add `DISABLE_EXTERN_CMD` to the Scripting Define Symbols.

# Settings
<img src="https://github.com/SlushyRH/Unity-External-Console/blob/main/readme/InspectorWindow.gif" alt="External Console Inspector Settings">

You can change the settings under `Tools/SRH/External Console Settings` in the Unity Editor, or by accessing the
settings through `ConsoleWindow.Settings` during runtime.

- **Console Window Name** controls the name of the console window. If empty, it will display the path to the game's exe
- **Target Build** controls when the console is allowed based on the type of build:
    - **None** means it will never show
    - **Development** means it will only show on development builds (This is the default)
    - **Release** means it will only show on release builds
    - **Everything** means it will show in both development and standard builds
- **Stack Trace Visibility** controls whether the stack trace will be logged to the console.
  - <i>Stack Trace will only be visible in Error & Exception messages unless in a Development build in which case all Stack Traces will be visible.</i>
- **Include Log Type** will add the log type in front of the message:
    - [Info] Log Message Here
    - [Error] Log Message Here
- **Include Timestamp** will add the time of the log in front of the message:
    - [16:24:14] Log Message Here
- **Timestamp Format** will allow you to switch the format of the timestamp between 12 and 24 hour formatting:
    - [4:24:14 PM] Log Message Here
    - OR
    - [16:24:14] Log Message Here
- **Log Colours** will allow you to control what colour each log appears as in the console.
  <br>
<p>If both <b>Include Log Type</b> and <b>Include Timestamp</b> are on. Then the LogType will come first and will be displayed in the console like this: [LogType] [Timestamp] Log Messager Here</p>