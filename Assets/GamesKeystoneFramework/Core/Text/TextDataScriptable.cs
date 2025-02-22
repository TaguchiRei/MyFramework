using GamesKeystoneFramework.TextSystem;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GamesKeystoneFramework.Core.Text
{
    [CreateAssetMenu(fileName = "TextData", menuName = "Scriptable Objects/TextDataObject")]
    public class TextDataScriptable : ScriptableObject
    {
        public List<TextDataList> TextDataList = new();
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
        [FormerlySerializedAs("_useEvent")] public bool UseEvent;
        public UnityEvent Event;
    }
}
