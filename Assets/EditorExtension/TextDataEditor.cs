using GamesKeystoneFramework.Core.Text;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
namespace GamesKeystoneFramework.TextSystem
{
    public class TextDataEditor : EditorWindow
    {
        TextDataScriptable textDataScriptable;
        int indentation = 0;
        int selectNumber = 0;
        string[] options;
        [MenuItem("Window/GamesKeystoneFramework/TextDataEditor")]
        public static void ShowWindow()
        {
            GetWindow<TextDataEditor>("TextDataEditor").Show();
        }
        private void OnGUI()
        {
            GUILayout.BeginHorizontal();

            if (textDataScriptable != null)
                selectNumber = EditorGUILayout.Popup(selectNumber, options, GUILayout.Width(80));

            EditorGUI.BeginChangeCheck();
            textDataScriptable = (TextDataScriptable)EditorGUILayout.ObjectField(
                textDataScriptable,
                typeof(TextDataScriptable),
                false
                );

            #region 中身のない場合空白のデータで埋める
            if (textDataScriptable != null)
            {
                if (textDataScriptable.textDataList.Count == 0)
                {
                    Initialization();
                }
            }
            #endregion

            #region アタッチ時に初期化
            if (EditorGUI.EndChangeCheck())
            {
                selectNumber = 0;
                if (textDataScriptable != null)
                {
                    OptionReset();
                }
            }
            #endregion

            GUILayout.EndHorizontal();

            if (textDataScriptable == null)
            {
                GUILayout.Label("スクリプタブルオブジェクトをアタッチ");
            }
            else
            {
                #region ラベル編集
                GUILayout.Label("編集中はインスペクターから編集しないでください");
                GUILayout.Label("ラベル");
                EditorGUI.BeginChangeCheck();
                textDataScriptable.textDataList[selectNumber].textLabel =
                    EditorGUILayout.TextField(textDataScriptable.textDataList[selectNumber].textLabel,
                    GUILayout.ExpandWidth(false));
                if (EditorGUI.EndChangeCheck())
                    OptionReset();
                #endregion

                #region 本体
                for (int i = 0; i < textDataScriptable.textDataList[selectNumber].dataList.Count; i++)
                {
                    var dl = textDataScriptable.textDataList[selectNumber].dataList;
                    GUILayout.BeginHorizontal();
                    dl[i].dataType = (TextDataType)EditorGUILayout.EnumPopup(dl[i].dataType, GUILayout.Width(80));
                    if (dl[i].dataType == TextDataType.QEnd || dl[i].dataType == TextDataType.TextEnd)
                    {
                        GUI.enabled = false;
                        dl[i].text = EditorGUILayout.TextField($"{dl[i].dataType}には入力できません");
                    }
                    else
                    {
                        dl[i].text = EditorGUILayout.TextField(dl[i].text, GUILayout.ExpandWidth(true));
                    }
                    if (GUILayout.Button("×", GUILayout.Width(20), GUILayout.Height(20)))
                    {
                        dl.RemoveAt(i);
                    }
                    if (GUILayout.Button("↓", GUILayout.Width(20), GUILayout.Height(20)))
                    {
                        dl.Insert(i + 1, new TextData { dataType = TextDataType.Text, text = "" });
                    }
                    GUILayout.EndHorizontal();
                }
                #endregion
            }
        }
        void OptionReset()
        {
            options = Enumerable.Range(0, textDataScriptable.textDataList
                .Count())
                .Select(n => n.ToString())
                .ToArray();
            for (int i = 0; i < options.Length; i++)
            {
                StringBuilder sb = new();
                sb.Append(options[i]);
                sb.Append(" ");
                sb.Append(textDataScriptable.textDataList[i].textLabel);
                options[i] = sb.ToString();
            }
        }
        void Initialization()
        {
            for (int i = 0; i < 3; i++)
                textDataScriptable.textDataList[0].dataList.Add(new());
        }
    }
}