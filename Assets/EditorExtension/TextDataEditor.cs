using System.Collections.Generic;
using System.Text;
using GamesKeystoneFramework.Core.Text;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using ColorUtility = UnityEngine.ColorUtility;

public class TextDataEditor : EditorWindow
{
    //編集するスクリプタブルオブジェクト
    private TextDataScriptable _textDataScriptable;

    /// <summary>
    /// どの会話データかを示す数値
    /// </summary>
    private int _selectionNumber;

    private List<string> _selectionList;
    private string[] _selectionArray;

    //editor側で保存する情報
    private int _textMaxLength = 20;
    private int _selectionMaxLength = 8;
    private int _indentation;
    private Vector2 _scrollPosition;
    private Color _lineColor;
    private Color _highlightsColor;
    private GUIStyle _style;

    private string _lineDesign;

    //editor側で保存するためのキー
    private const string LineColorPrefKey = "TextDataEditor_LineColor";
    private const string TextMaxLengthPrefKey = "TextDataEditor_TextMaxLength";
    private const string SelectionMaxLengthPrefKey = "TextDataEditor_SelectionMaxLength";


    //保存用のSerializedProperty等
    private SerializedObject _textDataScriptableSerializedObject;
    private SerializedProperty _textDataListProperty;
    private SerializedProperty _dataListProperty;
    private SerializedProperty _labelProperty;
    private List<SerializedProperty> _dataTypePropertyList;
    private List<SerializedProperty> _textPropertyList;
    private List<SerializedProperty> _useEventPropertyList;
    private List<SerializedProperty> _methodNumPropertyList;


    [MenuItem("Window/GamesKeystoneFramework/TextDataEditor")]
    public static void ShowWindow()
    {
        GetWindow<TextDataEditor>("TextDataEditor").Show();
    }

    private void OnEnable()
    {
        _style = new(GUI.skin.label)
        {
            richText = true
        };

        var st = new StringBuilder();
        for (int i = 0; i < 5; i++)
        {
            st.Append($"<color=#{ColorUtility.ToHtmlStringRGBA(_lineColor)}>｜</color>｜｜");
        }

        _lineDesign = st.ToString();
        _selectionNumber = 0;
    }

    private void OnGUI()
    {

        EditorGUI.BeginChangeCheck();
        _textDataScriptable = (TextDataScriptable)EditorGUILayout.ObjectField("会話データをアタッチ", _textDataScriptable,
            typeof(TextDataScriptable), false);
        if (EditorGUI.EndChangeCheck())
        {
            //エラー回避
            if (_textDataScriptable == null)
                return;
            SelecterReset();
            _textDataScriptableSerializedObject = null;
        }

        //エラー回避
        if (_textDataScriptable == null)
        {
            return;
        }

        GUILayout.BeginHorizontal();
        _selectionNumber = EditorGUILayout.Popup(_selectionNumber, _selectionArray, GUILayout.Width(100));
        if (GUILayout.Button("読み込み"))
        {
            LoadData();
            if (_textDataScriptableSerializedObject == null)
            {
                Debug.Log("データを正常にロードできませんでした");
                return;
            }
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        _textMaxLength = EditorGUILayout.IntSlider("テキストの長さ", _textMaxLength, 1, 300);
        _selectionMaxLength = EditorGUILayout.IntSlider("選択肢の長さ", _selectionMaxLength, 1, 300);
        GUILayout.EndHorizontal();

        if (_textDataScriptableSerializedObject == null)
        {
            GUILayout.Label("データを読み込んでください");
            return;
        }

        _textDataScriptableSerializedObject.Update();
        _indentation = 0;

        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(position.height - 180));
        for (int i = 0; i < _dataTypePropertyList.Count; i++)
        {
            var dataType = (TextDataType)_dataTypePropertyList[i].intValue;
            var text = _textPropertyList[i].stringValue;
            var useEvent = _useEventPropertyList[i].boolValue;
            var methodNum = _methodNumPropertyList[i].intValue;

            #region 文字数超過チェック

            if (dataType == TextDataType.Text)
            {
                if (text.Length > _textMaxLength)
                {
                    GUILayout.Label($"　　　{i}行目文字数超過");
                }
            }
            else if (dataType == TextDataType.Branch)
            {
                if (text.Length > _selectionMaxLength)
                {
                    GUILayout.Label($"　　　{i}行目文字数超過");
                }
            }

            #endregion

            #region 処理前の_indentation調整

            if (dataType != TextDataType.Text)
            {
                if (dataType == TextDataType.Branch)
                {
                    _indentation--;
                }
                else if (dataType == TextDataType.QEnd)
                {
                    _indentation -= 2;
                }
            }

            #endregion

            GUILayout.BeginHorizontal();
            GUILayout.Label(i.ToString(), GUILayout.Width(30));
            EditorGUILayout.PropertyField(_dataTypePropertyList[i], GUIContent.none, GUILayout.Width(80));
            GUILayout.Label(_lineDesign, _style, GUILayout.Width(_indentation * 20));
            if (dataType == TextDataType.QEnd || dataType == TextDataType.TextEnd)
            {
                GUI.enabled = false;
                GUILayout.TextField($"{dataType}には入力できません");
                GUI.enabled = true;
            }
            else
            {
                EditorGUILayout.PropertyField(_textPropertyList[i], GUIContent.none, GUILayout.ExpandWidth(true));
            }

            if (useEvent)
            {
                EditorGUILayout.PropertyField(_methodNumPropertyList[i], GUIContent.none, GUILayout.Width(30));
            }

            EditorGUILayout.PropertyField(_useEventPropertyList[i], GUIContent.none, GUILayout.Width(20));

            if (GUILayout.Button("×", GUILayout.Width(20)))
            {
                if (_textDataScriptable.TextDataList[_selectionNumber].DataList.Count != 1)
                {
                    _textDataScriptable.TextDataList[_selectionNumber].DataList.RemoveAt(i);
                    LoadData();
                    GUILayout.EndHorizontal();
                    GUILayout.EndScrollView();
                    return;
                }
            }

            if (GUILayout.Button("↓", GUILayout.Width(20)))
            {
                _textDataScriptable.TextDataList[_selectionNumber].DataList.Insert(i + 1, new TextData());
                LoadData();
                GUILayout.EndHorizontal();
                GUILayout.EndScrollView();
                return;
            }

            GUILayout.EndHorizontal();

            #region 処理後の_indentation調整

            if (dataType == TextDataType.Question)
            {
                _indentation += 2;
            }
            else if (dataType == TextDataType.Branch)
            {
                _indentation++;
            }

            #endregion
        }

        EditorGUILayout.EndScrollView();

        _textDataScriptableSerializedObject.ApplyModifiedProperties();
    }


    void SelecterReset()
    {
        _selectionList = new List<string>();
        for (int i = 0; i < _textDataScriptable.TextDataList.Count; i++)
        {
            _selectionList.Add(_textDataScriptable.TextDataList[i].TextLabel);
        }

        _selectionArray = _selectionList.ToArray();
    }

    /// <summary>
    /// シリアライズドオブジェクトを設定、textDataListとtextData
    /// </summary>
    private void LoadData()
    {
        _textDataScriptableSerializedObject = new SerializedObject(_textDataScriptable);
        _textDataListProperty = _textDataScriptableSerializedObject.FindProperty("TextDataList");
        _dataListProperty = _textDataListProperty.GetArrayElementAtIndex(_selectionNumber)
            .FindPropertyRelative("DataList");
        _labelProperty = _textDataListProperty.GetArrayElementAtIndex(_selectionNumber)
            .FindPropertyRelative("TextLabel");
        if (_dataTypePropertyList == null)
        {
            _dataTypePropertyList = new();
            _textPropertyList = new();
            _useEventPropertyList = new();
            _methodNumPropertyList = new();
        }
        else
        {
            _dataTypePropertyList.Clear();
            _textPropertyList.Clear();
            _useEventPropertyList.Clear();
            _methodNumPropertyList.Clear();
        }

        for (int i = 0; i < _textDataScriptable.TextDataList[_selectionNumber].DataList.Count; i++)
        {
            var textData = _dataListProperty.GetArrayElementAtIndex(i);
            _dataTypePropertyList.Add(textData.FindPropertyRelative("DataType"));
            _textPropertyList.Add(textData.FindPropertyRelative("Text"));
            _useEventPropertyList.Add(textData.FindPropertyRelative("UseEvent"));
            _methodNumPropertyList.Add(textData.FindPropertyRelative("MethodNumber"));
        }
    }
}