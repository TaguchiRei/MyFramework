using System.Collections.Generic;
using System;

namespace GamesKeystoneFramework.Core
{
    /// <summary>
    /// セーブするフォルダをenumで選択。
    /// </summary>
    public enum SaveSubject
    {
        GameData = 0,
        PlayerData = 1,
        EnemyData = 2,
        SaveData1 = 3,
        SaveData2 = 4,
        SaveData3 = 5,
        Conversation1 = 6,
        Conversation2 = 7,
        Conversation3 = 8,
        Other = 9,
    }
    /// <summary>
    /// 保存したいデータは必ずここを利用して作ること。
    /// IDは重複して付けてはいけない。
    /// </summary>
    [Serializable]
    public struct Data
    {
        public SaveSubject subject;
        public int ID;
        public List<int> IntValue;
        public List<string> StringValue;
    }

    /// <summary>
    /// セーブマネージャーの作成に使う
    /// </summary>
    /// <typeparam name="T">Dataを指定</typeparam>
    interface ISaveInterface <T>
    {

        public List<Func<List<T>>> CalledData { get; set; }
        List<T> DataContents { get; set; }
    }

    /// <summary>
    ///　セーブデータを持つオブジェクトに付与する
    /// </summary>
    /// <typeparam name="T"></typeparam>
    interface IHaveSaveData<T>
    {
        List<T> DataContents { get; set; }
        public List<T> Send()
        {
            return DataContents;
        }
    }
}
