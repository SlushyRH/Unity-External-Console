using System;
using UnityEngine;

namespace SRH
{
    public class ConsoleCleanupBridge : MonoBehaviour
    {
        private void OnApplicationQuit()
        {
            ConsoleWindow.Cleanup();
        }
    }
}