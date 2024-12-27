using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class JSONEditorWindow : EditorWindow
{
    private string jsonFilePath = "Assets/Resources/data.json";
    private List<string> stringList = new List<string>();

    [MenuItem("Window/JSON Editor")]
    public static void ShowWindow()
    {
        JSONEditorWindow window = GetWindow<JSONEditorWindow>("JSON Editor");
        window.Show();
    }

    private void OnEnable()
    {
        LoadJSON();
    }

    private void OnGUI()
    {
        GUILayout.Label("Edit JSON Data", EditorStyles.boldLabel);

        // JSONファイルのパス表示
        jsonFilePath = EditorGUILayout.TextField("JSON File Path", jsonFilePath);

        // List<string> を表示
        for (int i = 0; i < stringList.Count; i++)
        {
            stringList[i] = EditorGUILayout.TextField($"Item {i + 1}", stringList[i]);
        }

        // アイテムの追加
        if (GUILayout.Button("Add Item"))
        {
            stringList.Add(string.Empty);
        }

        // 保存ボタン
        if (GUILayout.Button("Save"))
        {
            SaveJSON();
        }
    }

    private void LoadJSON()
    {
        if (File.Exists(jsonFilePath))
        {
            string json = File.ReadAllText(jsonFilePath);
            StringListWrapper wrapper = JsonUtility.FromJson<StringListWrapper>(json);
            stringList = wrapper.items ?? new List<string>();
        }
        else
        {
            Debug.LogWarning("JSON file not found. Creating a new one.");
            stringList = new List<string>();
        }
    }

    private void SaveJSON()
    {
        StringListWrapper wrapper = new StringListWrapper { items = stringList };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(jsonFilePath, json);
        AssetDatabase.Refresh();
    }

    [System.Serializable]
    public class StringListWrapper
    {
        public List<string> items;
    }
}
