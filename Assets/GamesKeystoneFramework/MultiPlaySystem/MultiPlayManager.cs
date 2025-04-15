using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using UnityEngine;

public class MultiPlayManager : MonoBehaviour
{
    [Header("初期化完了しているかどうか")]
    public bool canMultiPlay;

    private CreateLobbyOptions createLobbyOptions;
    
    /// <summary>
    /// ロビーのリスト
    /// </summary>
    private QueryLobbiesOptions queryLobbiesOptions;
    
    protected virtual void Awake()
    {
        _ = ServicesInitialize();
    }

    /// <summary>
    /// ホストする際にサービスを初期化する。
    /// 一度だけ使用
    /// </summary>
    private async UniTask ServicesInitialize()
    {
        //ゲーム側のサービス初期化
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            //アカウントが無かった場合新たに作成
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        Debug.Log("Services initialized");
        canMultiPlay = true;
    }
}
