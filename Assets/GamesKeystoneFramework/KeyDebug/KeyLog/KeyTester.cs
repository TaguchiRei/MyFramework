using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace GamesKeystoneFramework.KeyDebug.KeyLog
{
    public static class KeyTester
    {
        private static Canvas _canvas;
        private static TextMeshProUGUI _logText;
        private static Queue<float> _logTimes;
        
        private static StringBuilder _logStringBuilder;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            //キャンバス作成
            var logCanvas = new GameObject("KeyTesterCanvas");
            _canvas = logCanvas.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            //ログテキスト作成
            var logTextObject = new GameObject("KeyTesterLogText");
            _logText = logTextObject.AddComponent<TextMeshProUGUI>();
            _logText.transform.SetParent(_canvas.transform);
            Log("KeyTester Initialized");
        }
        public static void Log(string message, Color color = default)
        {
            _logStringBuilder.Append($"{message}\n");
        }

        public static void OldLogDelete()
        {
            var logTests = _logText.text.Split('\n');
            _logText.text = string.Join("\n", logTests, 1, logTests.Length - 1);
        }
    }
}