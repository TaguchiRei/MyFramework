using UnityEngine;

namespace GamesKeystoneFramework.KeyDebug
{
    public class KeyFailSafe : MonoBehaviour
    {
        private void OnEnable()
        {
            Application.logMessageReceived += OnLog;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= OnLog;
        }

        private void OnLog(string logString, string stackTrace, LogType type)
        {
            
        }
    }
}
