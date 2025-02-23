using System;
using System.Collections.Generic;
using GamesKeystoneFramework.Core.Text;
using UnityEditor;
using UnityEngine;

public class TextDataEditor : EditorWindow
{
    //編集するスクリプタブルオブジェクト
    private TextDataScriptable _textDataScriptable;
    /// <summary>
    /// どの会話データかを示す数値
    /// </summary>
    private int _selectionNumber;
    private List<string> _selectionList;
    
    //editor側で保存する情報
    private Vector2 _scrollPosition;
    private Color _lineColor;
    private Color _highlightsColor;
    private string _lineDesign;
    //editor側で保存するためのキー
    private const string LineColorPrefKey = "TextDataEditor_LineColor";
    
    
    //保存用のSerializedProperty等
    SerializedObject _tDSSerializedObject;
    SerializedProperty _textDataListProperty;
    SerializedProperty _textDataProperty;
    SerializedProperty _textProperty;
    SerializedProperty _labelProperty;
    
    
    

    [MenuItem("Window/GamesKeystoneFramework/TextDataEditor")]
    public static void ShowWindow()
    {
        GetWindow<TextDataEditor>("TextDataEditor").Show();
    }

    private void OnEnable()
    {
        _lineDesign = $"<color=#{ColorUtility.ToHtmlStringRGBA(_lineColor)}>｜</color>｜｜";
        
    }

    private void OnGUI()
    {
        //エラー回避
        
        //編集用スクリプト
        _textDataScriptable = (TextDataScriptable)EditorGUILayout.ObjectField("会話データをアタッチ",_textDataScriptable, typeof(TextDataScriptable), false);
    }

    private void LoadTextDataScriptable()
    {
        
    }

    void LoadTextDataList()
    {
        
    }
}
