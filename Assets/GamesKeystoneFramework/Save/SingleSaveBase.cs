using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using GamesKeystoneFramework.Core;

namespace GamesKeystoneFramework.SaveSystem.singleSave
{
    public abstract class SingleSaveBase : MonoBehaviour, ISaveInterface<Data>
    {
        /// <summary>
        /// �f�[�^���Ăяo�����߂̃f���Q�[�g�B
        /// ������Send���\�b�h���W�߂Ă����Ίy�ɏW�߂���
        /// </summary>
        public abstract Func<List<Data>> CalledData { get; set; }

        /// <summary>
        /// ���ۂɕۑ�����f�[�^������B
        /// </summary>
        public abstract List<Data> DataContents { get; set; }

        /// <summary>
        /// �f�[�^�����ׂĎ擾���܂��B�K�v�ȍ\���̂���Ȃ�����ɍ\���̂�ID����͂��Ă�������
        /// </summary>
        /// <returns></returns>
        public List<Data> Call()
        {
            return DataContents;
        }

        /// <summary>
        /// �f�[�^����w�肵�Ď擾���܂��B
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Data Call(int id)
        {
            if(!DataContents.Any(d => d.ID == id))
            {
#if UNITY_EDITOR
                Debug.Log($"����ID�͑��݂��܂���\nID:{id}");
#endif
                return default;
            }
            return DataContents.FirstOrDefault(x => x.ID == id);
        }

        /// <summary>
        /// �Z�[�u����f�[�^�𑗐M���܂��B�����ɂ�Data�\���̂̃��X�g�����Ă�������
        /// </summary>
        /// <param name="sendSaveData">Data�\���̂̃��X�g(DataContents)</param>
        public void Send(List<Data> sendSaveData)
        {
            foreach (var data in sendSaveData)
            {
                if (DataContents.Count != 0 && DataContents.Any(d => d.ID == data.ID))
                {
                    DataContents[DataContents.FindIndex(d => d.ID == data.ID)] = data;
#if UNITY_EDITOR
                    Debug.Log($"�f�[�^�̓���ւ��ɐ������܂���\nID:{data.ID}");
#endif
                }
                else
                {
                    DataContents.Add(data);
#if UNITY_EDITOR
                    Debug.Log($"�f�[�^�̒ǉ��ɐ������܂���\nID:{data.ID}");
#endif
                }
            }
#if UNITY_EDITOR
            Debug.Log($"�f�[�^�̃��X�g�����������܂���");
#endif
        }

        public void Save(bool readable = false)
        {
            WrapperClass wrapper = new() { wrapperList = DataContents };
            File.WriteAllText(Application.persistentDataPath + $"/{SaveSubject.GameData}", JsonUtility.ToJson(wrapper, readable));
        }

        public void Load()
        {
            var path = Application.persistentDataPath + $"/{SaveSubject.GameData}";
            if (!File.Exists(path))
            {
#if UNITY_EDITOR
                Debug.LogWarning($"{path}�Ƀt�@�C�������݂��܂���B");
#endif
                return;
            }
            WrapperClass wrapper = JsonUtility.FromJson<WrapperClass>(File.ReadAllText(path));
            DataContents.Clear();
            DataContents.AddRange(wrapper.wrapperList);

        }

        /// <summary>
        /// �f�[�^��json�ɕۑ��ł���`�ɂ���
        /// </summary>
        [Serializable]
        public class WrapperClass
        {
            public List<Data> wrapperList;
        }
    }
}

