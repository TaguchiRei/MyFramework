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
        bool _collapsePattern = false;
        Vector2 scrollPosition = Vector2.zero;
        TextManagerBaseL textManager;
        [MenuItem("Window/GamesKeystoneFramework/TextDataEditor")]
        public static void ShowWindow()
        {
            GetWindow<TextDataEditor>("TextDataEditor").Show();
        }
        private void OnGUI()
        {
            GUILayout.BeginHorizontal();

            if (textDataScriptable != null && options != null)
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
                if (textDataScriptable.TextDataList.Count == 0)
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
                textDataScriptable.TextDataList[selectNumber].TextLabel =
                    EditorGUILayout.TextField(textDataScriptable.TextDataList[selectNumber].TextLabel,
                    GUILayout.ExpandWidth(false));
                if (EditorGUI.EndChangeCheck())
                    OptionReset();
                #endregion

                #region 本体
                _collapsePattern = false;
                indentation = 0;
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(position.height - 150));
                for (int i = 0; i < textDataScriptable.TextDataList[selectNumber].DataList.Count; i++)
                {
                    var dl = textDataScriptable.TextDataList[selectNumber].DataList;
                    GUILayout.BeginHorizontal();
                    dl[i].DataType = (TextDataType)EditorGUILayout.EnumPopup(dl[i].DataType, GUILayout.Width(80));
                    if (dl[i].DataType == TextDataType.QEnd || dl[i].DataType == TextDataType.TextEnd)
                    {
                        GUI.enabled = false;
                        if (dl[i].DataType == TextDataType.QEnd)
                        {
                            indentation--;
                        }
                        dl[i].Text = EditorGUILayout.TextField($"{dl[i].DataType}には入力できません");
                        GUI.enabled = true;
                    }
                    else
                    {
                        if (dl[i].DataType == TextDataType.Question)
                        {
                            indentation++;
                        }
                        GUILayout.Space(indentation * 20);
                        dl[i].Text = EditorGUILayout.TextField(dl[i].Text, GUILayout.ExpandWidth(true));
                    }
                    if (GUILayout.Button("×", GUILayout.Width(20), GUILayout.Height(20)))
                    {
                        dl.RemoveAt(i);
                    }
                    if (GUILayout.Button("↓", GUILayout.Width(20), GUILayout.Height(20)))
                    {
                        dl.Insert(i + 1, new TextData { DataType = TextDataType.Text, Text = "" });
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
                #endregion

                #region 初期化
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("会話パターンを追加", GUILayout.Width(120)))
                {
                    textDataScriptable.TextDataList.Add(new());
                    OptionReset();
                    Initialization(textDataScriptable.TextDataList.Count);
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("初期化", GUILayout.Width(50), GUILayout.Height(20)))
                {
                    textDataScriptable.TextDataList[selectNumber].DataList.Clear();
                    Initialization(selectNumber);
                }

                if (GUILayout.Button("削除", GUILayout.Width(50), GUILayout.Height(20)))
                {
                    textDataScriptable.TextDataList.RemoveAt(selectNumber);
                    OptionReset();
                }
                GUILayout.EndHorizontal();
                #endregion
            }
        }
        void OptionReset()
        {
            options = Enumerable.Range(0, textDataScriptable.TextDataList
                .Count())
                .Select(n => n.ToString())
                .ToArray();
            for (int i = 0; i < options.Length; i++)
            {
                StringBuilder sb = new();
                sb.Append(options[i]);
                sb.Append(" ");
                sb.Append(textDataScriptable.TextDataList[i].TextLabel);
                options[i] = sb.ToString();
            }
            if (selectNumber > options.Length - 1)
            {
                selectNumber = options.Length - 1;
            }
        }
        void Initialization(int n = 0)
        {
            textDataScriptable.TextDataList.Add(new());
            for (int i = 0; i < 3; i++)
                textDataScriptable.TextDataList[n].DataList.Add(new());
        }
    }
}