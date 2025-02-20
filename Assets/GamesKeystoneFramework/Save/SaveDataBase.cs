using Cysharp.Threading.Tasks;
using System;
using System.IO;
using UnityEngine;

namespace GamesKeystoneFramework.Core.Save
{
    /// <summary>
    /// �Z�[�u�f�[�^�͂������p�������N���X�ɕϐ�������ĕۑ�����
    /// </summary>
    [Serializable]
    public abstract class SaveDataBase<T>
    {
        /// <summary>
        /// �Z�[�u����ۂ͂�����Ăяo��
        /// </summary>
        /// <param name="dataNumber"></param>
        /// <param name="fileName"></param>
        public void Save(int dataNumber, string fileName = "SaveData")
        {
            string path = Application.persistentDataPath + $"/{fileName + dataNumber}.json";
            if (File.Exists(path))
            {
#if UNITY_EDITOR
                Debug.Log("File Exists");
#endif
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log("File Not Exists");
#endif
            }
            File.WriteAllText(path, JsonUtility.ToJson(this));
            
        }
        public T Load(int dataNumber, string fileName = "SaveData")
        {
            string path = Application.persistentDataPath + $"/{fileName + dataNumber}.json";
            if (File.Exists(path))
            {
#if UNITY_EDITOR
                Debug.Log("File Exists");
#endif
                return JsonUtility.FromJson<T>(File.ReadAllText(path));
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log("File Not Exists");
#endif
                return default;
            }
        }
        /// <summary>
        /// �Z�[�u�f�[�^�̏��������s��
        /// </summary>
        /// <param name="dataNumber">�f�[�^�̔ԍ�</param>
        /// <param name="fileName"></param>
        public void ResetData(int dataNumber, string fileName = "SaveData")
        {
            string path = Application.persistentDataPath + $"/{fileName + dataNumber}.json";
            if (File.Exists(path))
            {
#if UNITY_EDITOR
                Debug.Log("File Exists");
#endif
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log("File Not Exists");
#endif
            }
            File.WriteAllText(path, JsonUtility.ToJson(default));
        }
    }
}