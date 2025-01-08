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
        /// データを呼び出すためのデリゲート。
        /// ここにSendメソッドを集めておけば楽に集められる
        /// </summary>
        public abstract List<Func<List<Data>>> CalledData { get; set; }

        /// <summary>
        /// 実際に保存するデータを入れる。
        /// </summary>
        public abstract List<Data> DataContents { get; set; }

        /// <summary>
        /// データをすべて取得します。必要な構造体が一つなら引数に構造体のIDを入力してください
        /// </summary>
        /// <returns></returns>
        public List<Data> Call()
        {
            return DataContents;
        }

        /// <summary>
        /// データを一つ指定して取得します。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Data Call(int id)
        {
            if(!DataContents.Any(d => d.ID == id))
            {
#if UNITY_EDITOR
                Debug.Log($"そのIDは存在しません\nID:{id}");
#endif
                return default;
            }
            return DataContents.FirstOrDefault(x => x.ID == id);
        }

        /// <summary>
        /// セーブするデータを送信します。引数にはData構造体のリストを入れてください
        /// </summary>
        /// <param name="sendSaveData">Data構造体のリスト(DataContents)</param>
        public void Send(List<Data> sendSaveData)
        {
            foreach (var data in sendSaveData)
            {
                if (DataContents.Count != 0 && DataContents.Any(d => d.ID == data.ID))
                {
                    DataContents[DataContents.FindIndex(d => d.ID == data.ID)] = data;
#if UNITY_EDITOR
                    Debug.Log($"データの入れ替えに成功しました\nID:{data.ID}");
#endif
                }
                else
                {
                    DataContents.Add(data);
#if UNITY_EDITOR
                    Debug.Log($"データの追加に成功しました\nID:{data.ID}");
#endif
                }
            }
#if UNITY_EDITOR
            Debug.Log($"データのリスト化が完了しました");
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
                Debug.LogWarning($"{path}にファイルが存在しません。");
#endif
                return;
            }
            WrapperClass wrapper = JsonUtility.FromJson<WrapperClass>(File.ReadAllText(path));
            DataContents.Clear();
            DataContents.AddRange(wrapper.wrapperList);

        }

        /// <summary>
        /// データをjsonに保存できる形にする
        /// </summary>
        [Serializable]
        public class WrapperClass
        {
            public List<Data> wrapperList;
        }
    }
}

