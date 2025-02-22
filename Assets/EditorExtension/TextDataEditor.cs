using System;
using GamesKeystoneFramework.Core.Text;
using System.Linq;
using System.Text;
using GamesKeystoneFramework.Text;
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
        private string _line;
        private string[] _options;
        private bool _collapsePattern;
        private bool _checkNull;
        private Vector2 _scrollPosition = Vector2.zero;
        private Color _textColor;
        private TextManagerBase _textManager;
        private GUIStyle _textStyle;

        private SerializedObject _textObject;
        private SerializedProperty _textDataProperty;
        private SerializedProperty _eventProperty;

        [MenuItem("Window/GamesKeystoneFramework/TextDataEditor")]
        public static void ShowWindow()
        {
            GetWindow<TextDataEditor>("TextDataEditor").Show();
        }

        private void OnEnable()
        {
            _checkNull = false;
            _selectNumber = 0;
            _indentation = 0;
            _collapsePattern = false;
        }

        private void OnGUI()
        {
            _line = $"<color=#{ColorUtility.ToHtmlStringRGBA(_textColor)}>｜</color>｜｜";
            var st = new StringBuilder();
            for (int i = 0; i < 5; i++)
            {
                st.Append(_line);
            }

            _line = st.ToString();

            GUILayout.BeginHorizontal();

            if (!_checkNull && _options != null)
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
                Debug.Log("Initialize");
                _selectNumber = 0;
                if (!_checkNull)
                {
                    _textObject = new SerializedObject(_textDataScriptable);
                    _textDataProperty = _textObject.FindProperty("TextDataList");
                    OptionReset();
                }

                if (_textDataScriptable == null)
                    Debug.Log("TextDataScriptable is null");
                if (_textDataProperty == null)
                    Debug.Log("TextDataProperty is null");
            }

            #endregion


            _checkNull = _textDataScriptable == null;

            #region 中身のない場合空白のデータで埋める

            if (!_checkNull)
            {
                if (_textDataScriptable.TextDataList.Count == 0)
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
            else
            {
                #region ラベル編集

                GUILayout.Label("編集中はインスペクターから編集しないでください");
                GUILayout.Label("ラベル");
                EditorGUI.BeginChangeCheck();
                _textDataScriptable.TextDataList[_selectNumber].TextLabel =
                    EditorGUILayout.TextField(_textDataScriptable.TextDataList[_selectNumber].TextLabel,
                        GUILayout.ExpandWidth(false));
                if (EditorGUI.EndChangeCheck())
                    OptionReset();

                #endregion

                #region 本体

                _textStyle = new(GUI.skin.label)
                {
                    richText = true
                };
                _collapsePattern = false;
                _indentation = 0;
                _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(position.height - 150));
                for (int i = 0; i < _textDataScriptable.TextDataList[_selectNumber].DataList.Count; i++)
                {
                    var dl = _textDataScriptable.TextDataList[_selectNumber].DataList;
                    GUILayout.BeginHorizontal();
                    dl[i].DataType = (TextDataType)EditorGUILayout.EnumPopup(dl[i].DataType, GUILayout.Width(80));
                    if (dl[i].DataType == TextDataType.QEnd || dl[i].DataType == TextDataType.TextEnd)
                    {
                        if (dl[i].DataType == TextDataType.QEnd)
                        {
                            _indentation -= 2;
                        }

                        GUI.enabled = false;
                        GUILayout.Label(_line, _textStyle, GUILayout.Width(_indentation * 20));
                        dl[i].Text = EditorGUILayout.TextField($"{dl[i].DataType}には入力できません");
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

                        dl[i].Text = EditorGUILayout.TextField(dl[i].Text, GUILayout.ExpandWidth(true));
                    }

                    if (dl[i].DataType == TextDataType.Text)
                    {
                        dl[i].UseEvent = EditorGUILayout.Toggle(dl[i].UseEvent, GUILayout.Width(20));
                        if (dl[i].UseEvent)
                        {
                            _eventProperty = _textDataProperty
                                .GetArrayElementAtIndex(_selectNumber)
                                .FindPropertyRelative("DataList")
                                .GetArrayElementAtIndex(i)
                                .FindPropertyRelative("Event");
                            EditorGUILayout.PropertyField(_eventProperty);
                        }
                    }
                    else
                    {
                        GUILayout.Space(20);
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

                #region 各種ボタン

                if (GUILayout.Button("保存", GUILayout.Width(40), GUILayout.Height(20)))
                {
                    Undo.RecordObject(_textDataScriptable, "ScriptableObject保存");
                    EditorUtility.SetDirty(_textDataScriptable);
                }

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

        void Initialization(int n = 0)
        {
            for (int i = 0; i < 3; i++)
                _textDataScriptable.TextDataList[n].DataList.Add(new());
        }
    }
}