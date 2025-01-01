using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace GamesKeystoneFramework.TextSystem
{
    public class TextDataEditor : EditorWindow
    {
        private List<TextData> textData = new();
        private string fileName;
        private Vector2 scrollPosition = Vector2.zero;
        [MenuItem("Window/TextDataEditor")]
        public static void ShowWindow()
        {
            GetWindow<TextDataEditor>("TextDataEditor").Show();
        }
        private void OnGUI()
        {
            fileName = EditorGUILayout.TextField("ファイル名を入力", fileName);
            string path = Path.Combine(Application.dataPath, "TextData", fileName + ".json");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("FileRead", GUILayout.Width(80), GUILayout.Height(20)) && File.Exists(path))
            {
                textData = JsonUtility.FromJson<TextWrpper>(File.ReadAllText(path)).wrpperData;
            }
            if (GUILayout.Button("Reset", GUILayout.Width(50), GUILayout.Height(20)))
            {
                textData.Clear();
                for (int i = 0; i < 5; i++)
                {
                    textData.Add(new TextData
                    {
                        dataType = TextDataType.Text,
                        text = ""
                    });
                }
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Complete", GUILayout.Width(80), GUILayout.Height(20)))
            {
                TextWrpper wrpper = new() { wrpperData = textData };
                string dataPath = Path.Combine(Application.dataPath, "TextData", fileName + ".json");
                if (File.Exists(dataPath))
                {
                    File.WriteAllText(dataPath, JsonUtility.ToJson(wrpper));
                    Debug.Log(dataPath + "に保存しました");
                }
                else
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(dataPath));
                    File.WriteAllText(dataPath, JsonUtility.ToJson(wrpper));
                    Debug.Log(dataPath + "にファイルを作成しました");
                }
            }
            GUILayout.EndHorizontal();
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(position.height - 40));
            for (int i = 0; i < textData.Count; i++)
            {
                GUILayout.BeginHorizontal();
                textData[i].dataType = (TextDataType)EditorGUILayout.EnumPopup(textData[i].dataType, GUILayout.Width(80));
                textData[i].text = EditorGUILayout.TextField(textData[i].text, GUILayout.ExpandWidth(true));
                if (GUILayout.Button("×", GUILayout.Width(20), GUILayout.Height(20)))
                {
                    textData.RemoveAt(i);
                }
                if (GUILayout.Button("↓", GUILayout.Width(20), GUILayout.Height(20)))
                {
                    textData.Insert(i + 1, new TextData { dataType = TextDataType.Text, text = "" });
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }
    }
}