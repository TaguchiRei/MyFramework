using GamesKeystoneFramework.Core.Text;
using System.Linq;
using System.Text;
using GamesKeystoneFramework.TextSystem;
using UnityEditor;
using UnityEngine;
using ColorUtility = UnityEngine.ColorUtility;

namespace editorExtension
{
    public class TextDataEditor : EditorWindow
    {
        private TextDataScriptable _textDataScriptable;
        private int _indentation;
        private int _selectNumber;
        private int _numberOfText = 20;
        private int _numberOfSelection = 5;
        private string _line;
        private string[] _options;
        private Vector2 _scrollPosition = Vector2.zero;
        private Color _textColor;
        private GUIStyle _textStyle;
        
        SerializedObject _textDataScriptableObject;
        SerializedProperty _textDataListProperty;
        SerializedProperty _textDataProperty;
        SerializedProperty _labelProperty;

        [MenuItem("Window/GamesKeystoneFramework/TextDataEditor")]
        public static void ShowWindow()
        {
            GetWindow<TextDataEditor>("TextDataEditor").Show();
        }

        private void OnEnable()
        {
            _selectNumber = 0;
            _indentation = 0;
            if (_textDataScriptableObject != null)
            {
                LoadScriptableObject();
            }
        }

        private void OnGUI()
        {
            if(_textDataScriptableObject != null)
                _textDataScriptableObject.Update();
            _line = $"<color=#{ColorUtility.ToHtmlStringRGBA(_textColor)}>｜</color>｜｜";
            var st = new StringBuilder();
            for (int i = 0; i < 5; i++)
            {
                st.Append(_line);
            }

            _line = st.ToString();

            GUILayout.BeginHorizontal();

            if (_textDataScriptable != null && _options != null)
                _selectNumber = EditorGUILayout.Popup(_selectNumber, _options, GUILayout.Width(80));

            EditorGUI.BeginChangeCheck();
            _textDataScriptable = (TextDataScriptable)EditorGUILayout.ObjectField(
                _textDataScriptable,
                typeof(TextDataScriptable),
                false
            );

            #region アタッチ時に初期化

            if (EditorGUI.EndChangeCheck())
            {
                _selectNumber = 0;
                if (_textDataScriptable != null)
                {
                    LoadScriptableObject();
                    OptionReset();
                }
            }

            #endregion

            #region 中身のない場合空白のデータで埋める

            if (_textDataScriptable != null)
            {
                if (_textDataScriptable.TextDataList == null)
                {
                    Initialization();
                }
            }

            #endregion


            GUILayout.EndHorizontal();

            if (_textDataScriptable == null)
            {
                GUILayout.Label("スクリプタブルオブジェクトをアタッチ");
            }
            else if (_textDataScriptable.TextDataList != null)
            {
                #region ラベル編集

                GUILayout.Label("編集中はインスペクターから編集しないでください");

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(_labelProperty, true);
                if (EditorGUI.EndChangeCheck())
                    OptionReset();

                GUILayout.BeginHorizontal();
                _numberOfText = EditorGUILayout.IntSlider("一行の文字数", _numberOfText, 10, 200);
                _numberOfSelection = EditorGUILayout.IntSlider("選択肢の文字数", _numberOfSelection, 5, 100);
                GUILayout.EndHorizontal();

                #endregion

                #region 本体

                _textStyle = new(GUI.skin.label)
                {
                    richText = true
                };
                _indentation = 0;
                _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(position.height - 150));
                for (int i = 0; i < _textDataScriptable.TextDataList[_selectNumber].DataList.Count; i++)
                {
                    var dl = _textDataScriptable.TextDataList[_selectNumber].DataList;

                    var data = _textDataProperty.GetArrayElementAtIndex(i);
                    var dataType = data.FindPropertyRelative("DataType");
                    var text = data.FindPropertyRelative("Text");
                    var useEvent = data.FindPropertyRelative("UseEvent");
                    var methodNum = data.FindPropertyRelative("MethodNumber");

                    if (dl[i].DataType == TextDataType.Text)
                    {
                        if (dl[i].Text.Length > _numberOfText)
                        {
                            GUILayout.Label($"　　　{i}行目文字数超過");
                        }
                    }
                    else if (dl[i].DataType == TextDataType.Branch)
                    {
                        if (dl[i].Text.Length > _numberOfSelection)
                        {
                            GUILayout.Label($"　　　{i}行目文字数超過");
                        }
                    }

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(i.ToString(), GUILayout.Width(30));
                    EditorGUILayout.PropertyField(dataType, GUIContent.none, GUILayout.Width(80));
                    if (dl[i].DataType == TextDataType.QEnd || dl[i].DataType == TextDataType.TextEnd)
                    {
                        if (dl[i].DataType == TextDataType.QEnd)
                        {
                            _indentation -= 2;
                        }

                        GUI.enabled = false;
                        GUILayout.Label(_line, _textStyle, GUILayout.Width(_indentation * 20));
                        EditorGUILayout.TextField($"{dl[i].DataType}には入力できません");
                        GUI.enabled = true;
                    }
                    else
                    {
                        if (dl[i].DataType == TextDataType.Branch)
                        {
                            _indentation--;
                        }

                        GUILayout.Label(_line, _textStyle, GUILayout.Width(_indentation * 20));
                        if (dl[i].DataType == TextDataType.Branch)
                        {
                            _indentation++;
                        }
                        else if (dl[i].DataType == TextDataType.Question)
                        {
                            _indentation += 2;
                        }

                        EditorGUILayout.PropertyField(text, GUIContent.none, GUILayout.ExpandWidth(true));
                    }

                    if (dl[i].UseEvent)
                    {
                        EditorGUILayout.PropertyField(methodNum, GUIContent.none, GUILayout.Width(40));
                    }

                    EditorGUILayout.PropertyField(useEvent, GUIContent.none, GUILayout.Width(20));


                    if (GUILayout.Button("×", GUILayout.Width(20), GUILayout.Height(20)))
                    {
                        dl.RemoveAt(i);
                        _textDataListProperty.DeleteArrayElementAtIndex(i);
                    }

                    if (GUILayout.Button("↓", GUILayout.Width(20), GUILayout.Height(20)))
                    {
                        dl.Insert(i + 1, new TextData { DataType = TextDataType.Text, Text = "" });
                        _textDataListProperty.InsertArrayElementAtIndex(i);
                        LoadScriptableObject();
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndScrollView();

                #endregion

                #region 各種ボタン

                _textColor = EditorGUILayout.ColorField(_textColor, GUILayout.Width(80));
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("会話パターンを追加", GUILayout.Width(120)))
                {
                    _textDataScriptable.TextDataList.Add(new());
                    Initialization(_textDataScriptable.TextDataList.Count - 1);
                    OptionReset();
                    _selectNumber = _textDataScriptable.TextDataList.Count - 1;
                }

                GUILayout.FlexibleSpace();
                if (GUILayout.Button("初期化", GUILayout.Width(50), GUILayout.Height(20)))
                {
                    _textDataScriptable.TextDataList[_selectNumber].DataList.Clear();
                    Initialization(_selectNumber);
                }

                if (GUILayout.Button("削除", GUILayout.Width(50), GUILayout.Height(20)))
                {
                    _textDataScriptable.TextDataList.RemoveAt(_selectNumber);
                    if (_textDataScriptable.TextDataList.Count == 0)
                    {
                        _textDataScriptable.TextDataList.Add(new());
                        Initialization();
                        _selectNumber = 0;
                        OptionReset();
                    }

                    OptionReset();
                }

                GUILayout.EndHorizontal();

                #endregion
                if (_textDataScriptableObject != null && _textDataScriptableObject.ApplyModifiedProperties())
                {
                    EditorUtility.SetDirty(_textDataScriptableObject.targetObject);
                    AssetDatabase.SaveAssets();
                }
            }
        }

        void OptionReset()
        {
            _options = Enumerable.Range(0, _textDataScriptable.TextDataList
                    .Count())
                .Select(n => n.ToString())
                .ToArray();
            for (int i = 0; i < _options.Length; i++)
            {
                StringBuilder sb = new();
                sb.Append(_options[i]);
                sb.Append(" ");
                sb.Append(_textDataScriptable.TextDataList[i].TextLabel);
                _options[i] = sb.ToString();
            }

            if (_selectNumber > _options.Length - 1)
            {
                _selectNumber = _options.Length - 1;
            }
        }

        void LoadScriptableObject()
        {
            _textDataScriptableObject = new(_textDataScriptable);
            _textDataListProperty = _textDataScriptableObject.FindProperty("TextDataList");
            _textDataProperty = _textDataListProperty
                .GetArrayElementAtIndex(_selectNumber)
                .FindPropertyRelative("DataList");
            _labelProperty = _textDataListProperty.GetArrayElementAtIndex(_selectNumber).FindPropertyRelative("TextLabel");
        }
        
        void Initialization(int n = 0)
        {
            Debug.Log(_textDataScriptable.TextDataList.Count + " " + n);
            _textDataScriptable.TextDataList[n].DataList = new();
            for (int i = 0; i < 3; i++)
                _textDataScriptable.TextDataList[n].DataList.Add(new());
        }
    }
}