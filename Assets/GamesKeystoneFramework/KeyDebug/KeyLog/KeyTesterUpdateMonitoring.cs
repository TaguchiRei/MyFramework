using System.Collections.Generic;
using UnityEngine;

namespace GamesKeystoneFramework.KeyDebug.KeyLog
{
    public class KeyTesterUpdateMonitoring : MonoBehaviour
    {
        public Queue<(float,int)> _logQueue = new();
        public float LogDeleteTime = 7;
        

        private void Update()
        {
            if (_logQueue.TryPeek(out (float,int) value) && value.Item1 + LogDeleteTime < Time.time)
            {
                _logQueue.Dequeue();
                KeyLogger.OldLogDelete();
            }
        }
    }
}
