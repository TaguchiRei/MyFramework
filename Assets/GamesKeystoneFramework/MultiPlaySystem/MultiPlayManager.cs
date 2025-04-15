using System;
using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using UnityEngine;

public class MultiPlayManager : MonoBehaviour
{
    [Header("初期化完了しているかどうか")]
    public bool canMultiPlay;
    
    /// <summary>
    /// ロビーのリスト
    /// </summary>
    private QueryLobbiesOptions queryLobbiesOptions;
    
    [HideInInspector] public MultiPlayManager Instance;
    
    /// <summary>
    /// シーン開始時に必ずこのメソッドを動かすこと
    /// </summary>
    private void SingletonInitialize()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    
    /// <summary>
    /// ホストする際にサービスを初期化する。
    /// 一度だけ使用する
    /// 通信エラー発生時はfalseを返す
    /// </summary>
    private async UniTask<bool> ServicesInitialize()
    {
        //ゲーム側のサービス初期化
        try
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                //アカウントが無かった場合新たに作成
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }
        catch(Exception e)
        {
            Debug.LogError($"Services Initialize Error{e.Message}");
            canMultiPlay = false;
            return false;
        }
        
        Debug.Log("Services initialized");
        canMultiPlay = true;
        return true;
    }
}
