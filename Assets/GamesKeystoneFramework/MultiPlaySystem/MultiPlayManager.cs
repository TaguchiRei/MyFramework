using System;
using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class MultiPlayManager : MonoBehaviour
{
    public bool canMultiPlay;


    private void Awake()
    {
        canMultiPlay = false;
    }
    
    /// <summary>
    /// ホストする際にサービスを初期化する
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
    }
}
