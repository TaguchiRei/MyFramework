using System;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;

public class MultiPlayManager : MonoBehaviour
{
    [Header("初期化完了しているかどうか")]
    public bool canMultiPlay;

    [Header("マルチプレイ参加可能人数")]
    [SerializeField] private int _numberObParticipants;
    
    private CreateLobbyOptions createLobbyOptions;
    
    /// <summary>
    /// ロビーのリスト
    /// </summary>
    private QueryLobbiesOptions queryLobbiesOptions;

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
    }

    /// <summary>
    /// ロビーを作成する際に使用するメソッド。要改善
    /// </summary>
    private async UniTask CreateLobby()
    {
        var allocation = await RelayService.Instance.CreateAllocationAsync(_numberObParticipants);

        var relayServerData = allocation.ToRelayServerData("dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>()
            .SetRelayServerData(new RelayServerData());
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        NetworkManager.Singleton.StartHost();
    }
}
