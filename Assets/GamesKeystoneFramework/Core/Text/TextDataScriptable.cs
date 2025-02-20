using GamesKeystoneFramework.TextSystem;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GamesKeystoneFramework.Core.Text
{
    [CreateAssetMenu(fileName = "TextData", menuName = "Scriptable Objects/TextDataObject")]
    public class TextDataScriptable : ScriptableObject
    {
        public List<TextDataList> textDataList;
    }

    [Serializable]
    public class TextDataList
    {
        public string textLabel = "default";
        public List<TextData> dataList = new();
    }
    [Serializable]
    public class TextData
    {
        public TextDataType dataType;
        public string text;
    }
}
