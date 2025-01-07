using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
namespace GamesKeystoneFramework.TextSystem
{
    public class TextDataReader : MonoBehaviour
    {
        [HideInInspector]public List<TextData> textData = new();
        
        public void LoadTextData(string fileName)
        {
            string path = Path.Combine(Application.dataPath, "TextData", fileName + ".json");
            textData = JsonUtility.FromJson<TextWrpper>(File.ReadAllText(path)).wrpperData;
        }
    }
    [Serializable]
    public class TextData
    {
        public TextDataType dataType;
        public string text;
    }
    [Serializable]
    class TextWrpper
    {
        public List<TextData> wrpperData;
    }
    public enum TextDataType
    {
        Text,
        Question,
        Branch,
        QEnd,
        TextEnd,
    }

}