using System.Collections.Generic;
using UnityEngine;

namespace GamesKeystoneFramework.KeyDebug.KeyLog
{
    public class KeyTesterUpdateMonitoring : MonoBehaviour
    {
        public Queue<float> _logQueue = new();
        public float LogDeleteTime = 7;
        

        private void Update()
        {
            if (_logQueue.TryPeek(out float value) && value + LogDeleteTime < Time.time)
            {
                _logQueue.Dequeue();
                KeyLogger.OldLogDelete();
            }
        }
    }
}
