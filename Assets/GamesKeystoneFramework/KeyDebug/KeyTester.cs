using System.Text;
using UnityEngine;

namespace GamesKeystoneFramework.KeyDebug
{
    public static class KeyTester
    {
        public static void Log<T>(string message, Color color = default)
        { 
            string hexColor = ColorUtility.ToHtmlStringRGBA(color);
            StringBuilder builder = new StringBuilder();
            builder.Append($"<color=#{hexColor}>");
            builder.Append($"{typeof(T).Name}: {message})");
            builder.Append($"</color>");
            Debug.Log(builder.ToString());
        }
    }
}