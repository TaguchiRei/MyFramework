using GamesKeystoneFramework.TextSystem;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace GamesKeystoneFramework.Core.Text
{
    [CreateAssetMenu(fileName = "TextData", menuName = "Scriptable Objects/TextDataObject")]
    public class TextDataScriptable : ScriptableObject
    {
        public List<TextDataList> TextDataList;
    }

    [Serializable]
    public class TextDataList
    {
        public string TextLabel = "default";
        public List<TextData> DataList = new();
    }
    [Serializable]
    public class TextData
    {
        public TextDataType DataType;
        public string Text;
        public UnityEvent Event;
    }
}
