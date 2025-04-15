using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using GamesKeystoneFramework.Core.Text;
namespace GamesKeystoneFramework.Text
{
    public class TextDataReaderL : MonoBehaviour
    {
        [HideInInspector]public List<TextData> textData = new();
        
        public void LoadTextData(string fileName)
        {
            string path = Path.Combine(Application.dataPath, "TextData", fileName + ".json");
            textData = JsonUtility.FromJson<TextWrpper>(File.ReadAllText(path)).wrpperData;
        }
    }

    [Serializable]
    class TextWrpper
    {
        public List<TextData> wrpperData;
    }


}