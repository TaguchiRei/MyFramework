using System.Collections.Generic;
using System;

namespace GamesKeystoneFramework.Core
{
    /// <summary>
    /// �Z�[�u����t�H���_��enum�őI���B
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
        other = 9,
    }
    /// <summary>
    /// �ۑ��������f�[�^�͕K�������𗘗p���č�邱�ƁB
    /// ID�͏d�����ĕt���Ă͂����Ȃ��B
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
    /// �Z�[�u�}�l�[�W���[�̍쐬�Ɏg��
    /// </summary>
    /// <typeparam name="T">Data���w��</typeparam>
    interface ISaveInterface <T>
    {

        public Func<List<T>> CalledData { get; set; }
        List<T> DataContents { get; set; }
    }

    /// <summary>
    ///�@�Z�[�u�f�[�^�����I�u�W�F�N�g�ɕt�^����
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
