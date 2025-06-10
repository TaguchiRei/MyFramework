using System.Collections.Generic;
using UnityEngine;

namespace GamesKeystoneFramework.KeyDebug.KeyLog
{
    public class KeyTesterUpdateMonitoring : MonoBehaviour
    {
        public Queue<float> _logQueue = new Queue<float>();

        private void Update()
        {
            if (_logQueue.TryPeek(out float value))
            {
                _logQueue.Dequeue();
                
            }
        }
    }
}
