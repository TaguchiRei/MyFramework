using System.Diagnostics;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace GamesKeystoneFramework.KeyDebug
{
    public static class KeyTester
    {
        public static void Log(string message, Color color = default)
        {
            if (color == default) color = Color.white;
            var stackTrace = new StackTrace();
            var method = stackTrace.GetFrame(1).GetMethod();
            string className = method.DeclaringType != null ? method.DeclaringType.FullName : "Not Found";
            string hexColor = ColorUtility.ToHtmlStringRGBA(color);
            Debug.Log($"<color=cyan>{className}</color> : <color=#{hexColor}>{message}</color>");
        } 
    }
}