using GamesKeystoneFramework.Save;
using UnityEngine;

public class TestSaveLoad : MonoBehaviour
{
    // SaveDataBase<T> を継承したテスト用のデータクラス
    [System.Serializable]
    public class TestData : SaveDataBase<TestData>
    {
        public int score;
        public string playerName;
    }

    // テスト用のデータインスタンス
    public TestData data;

    void Start()
    {
        // データが未生成なら新規作成
        if (data == null)
        {
            data = new TestData();
        }

        // テスト用のデータを設定
        data.score = 100;
        data.playerName = "TestPlayer";

        // セーブ番号 1、ファイル名 "TestData" としてデータをセーブする
        data.Save(1, "TestData");
#if UNITY_EDITOR
        Debug.Log("【TestSaveLoad】データをセーブしました");
#endif

        // ロード処理
        TestData loadedData = data.Load(1, "TestData");
        if (loadedData != null)
        {
#if UNITY_EDITOR
            Debug.Log("【TestSaveLoad】ロード成功: score = " + loadedData.score + ", playerName = " + loadedData.playerName);
#endif
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("【TestSaveLoad】ロード失敗");
#endif
        }
    }
    public void AA()
    {

    }
    public void BB()
    {

    }
}
