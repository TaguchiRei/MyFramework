using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ColorUtility = UnityEngine.ColorUtility;

namespace GamesKeystoneFramework.KeyDebug.KeyLog
{
    public static class KeyLogger
    {
        private static readonly Vector2 Anchor = new(0f, 1f);
        private static Canvas _canvas;
        private static TextMeshProUGUI _logText;
        private static KeyTesterUpdateMonitoring _updateMonitor;
        
        private static List<string> _allLogs = new List<string>();

        private static int _mostOldLog;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            //キャンバス作成
            var logCanvas = new GameObject("KeyTesterCanvas");
            _canvas = logCanvas.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var logCanvasScaler = logCanvas.AddComponent<CanvasScaler>();
            logCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            
            //キャンバスにUpdate監視追加
            _updateMonitor = logCanvas.AddComponent<KeyTesterUpdateMonitoring>();
            
            //ログテキスト作成
            var logTextObject = new GameObject("KeyTesterLogText");
            _logText = logTextObject.AddComponent<TextMeshProUGUI>();
            _logText.transform.SetParent(_canvas.transform);
            
            //ログテキストの位置を決定
            _logText.rectTransform.anchorMax = Vector2.up;
            _logText.rectTransform.anchorMin = Vector2.up;
            _logText.rectTransform.pivot = Vector2.up;
            _logText.rectTransform.anchoredPosition = Vector2.zero;
            
            //ログテキストの初期化
            _logText.textWrappingMode = TextWrappingModes.NoWrap;
            _logText.richText = true;
            _logText.fontSize = 14;
            
            //_allLogs準備
            _allLogs = new List<string>();
            _mostOldLog = 0;
            Log("<color=purple>KeyTester</color> <color=black>:</color> Initialized",Color.cyan);
        }

        /// <summary>
        /// ログを流す
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        /// <param name="color"></param>
        private static void LogInternal(string message, object type , Color color)
        {
            if(color == default) color = Color.black;

            StringBuilder st = new();
            if (type != null)
            {
                st.Append($"<color=purple>{type.GetType().Name}</color><color=black> : </color>");
            }
            st.Append($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{message}</color>\n");
            string log = st.ToString();
            _allLogs.Add(log);

            UpdateLogText();

            _updateMonitor._logQueue.Enqueue((Time.time,_allLogs.Count));
        }

        
        /// <summary>
        /// 画面上にログを流す。
        /// </summary>
        /// <param name="message">ログの内容</param>
        /// <param name="color">ログのカスタムカラー</param>
        public static void Log(string message, Color color = default)
        {
            LogInternal(message, null, color);
        }

        /// <summary>
        /// 画面上にログを流す
        /// </summary>
        /// <param name="message">ログの内容</param>
        /// <param name="type">呼び出し元の型</param>
        /// <param name="color">ログのカスタムカラー</param>
        /// <typeparam name="T">呼び出し元の型</typeparam>
        public static void Log<T>(string message, [NotNull] T type, Color color = default)
        {
            LogInternal(message, type, color);
        }

        public static void LogWarning(string message)
        {
            LogInternal($"[Warning] {message}", null, Color.yellow);
        }

        public static void LogWarning<T>(string message, [NotNull] T type)
        {
            LogInternal($"[Warning] {message}", type, Color.yellow);
        }

        public static void LogError(string message)
        {
            LogInternal($"[Error] {message}", null ,Color.red);
        }

        public static void LogError<T>(string message, [NotNull] T type)
        {
            LogInternal($"[Error] {message}", type, Color.red);
        }

        private static void UpdateLogText()
        {
            var visibleLogs = _allLogs.GetRange(_mostOldLog, _allLogs.Count - _mostOldLog);
            _logText.text = string.Join("", visibleLogs);
        }

        public static void OldLogDelete()
        {
            if (_mostOldLog < _allLogs.Count)
            {
                _mostOldLog++;
                UpdateLogText();
            }
        }
    }
}