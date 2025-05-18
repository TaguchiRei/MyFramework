using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace GamesKeystoneFramework.Test
{
    public class TimeCounter
    {
        private Dictionary<string, Stopwatch> watchesDict = new Dictionary<string, Stopwatch>();
        
        private Stopwatch watch;

        public void StopWatchStart()
        {
            watch ??= new Stopwatch();
            watch.Start();
        }

        public void StopWatchStop()
        {
            watch.Stop();
            UnityEngine.Debug.Log($"time : {watch.ElapsedMilliseconds}ms");
            watch.Reset();
        }

        public void StopWatchStart(string watchName)
        {
            if (watchesDict.ContainsKey(watchName))
            {
                UnityEngine.Debug.Log($"watchName:{watchName} is running");
            }
            watchesDict.Add(watchName, new Stopwatch());
        }

        public void StopWatchStart(int watchID)
        {
            if (watchesDict.ContainsKey(watchID.ToString()))
            {
                UnityEngine.Debug.Log($"watchID{watchID} is running");
            }
            watchesDict.Add(watchID.ToString(), new Stopwatch());
        }

        public void StopwatchStop(string watchName)
        {
            if (!watchesDict.ContainsKey(watchName))
            {
                UnityEngine.Debug.Log($"watchName:{watchName} is notfound");
            }
            watchesDict[watchName].Stop();
            UnityEngine.Debug.Log($"WatchName{watchName}　: {watchesDict[watchName].ElapsedMilliseconds}ms");
            watchesDict.Remove(watchName);
        }

        public void StopwatchStop(int watchID)
        {
            if (!watchesDict.ContainsKey(watchID.ToString()))
            {
                UnityEngine.Debug.Log($"watchID{watchID} is not found");
            }
            watchesDict[watchID.ToString()].Stop();
            UnityEngine.Debug.Log($" WatchID{watchID}　: {watchesDict[watchID.ToString()].ElapsedMilliseconds}ms");
            watchesDict.Remove(watchID.ToString());
        }
    }
}
