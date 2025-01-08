using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            GUILayout.EndHorizontal();
            GUILayout.Label("*00のように入力すると色を変えられます。終わるときは*のみ入力");
            GUILayout.Label("/024のように入力すると文字サイズを変えられます。終わるときは/のみ入力");

            if (textData.Count(x => x.dataType == TextDataType.Question) != textData.Count(x => x.dataType == TextDataType.QEnd))
            {
                var style = new GUIStyle(EditorStyles.label);
                style.richText = true;
                GUILayout.Label("<color=red><b>会話分岐の始点もしくは終点が足りません</b></color>", style);
            }
            else
            {
                GUILayout.Label(" ");
            }
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(position.height - 150));
            for (int i = 0; i < textData.Count; i++)
            {
                GUILayout.BeginHorizontal();
                textData[i].dataType = (TextDataType)EditorGUILayout.EnumPopup(textData[i].dataType, GUILayout.Width(80));
                if (textData[i].dataType == TextDataType.QEnd || textData[i].dataType == TextDataType.TextEnd)
                {
                    GUI.enabled = false;
                    textData[i].text = EditorGUILayout.TextField($"{textData[i].dataType}には入力できません", GUILayout.ExpandWidth(true));
                    GUI.enabled = true;
                }
                else
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
            GUILayout.BeginHorizontal();
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
        }
    }
}