using System;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class MultiPlayHostSystem : MonoBehaviour
{
    [Header("マルチプレイ参加可能人数")]
    [SerializeField] private int _numberObParticipants;

    private string joinCode;
    /// <summary>
    /// ロビーを作成する際に使用するメソッド。要改善
    /// </summary>
    private async UniTask<bool> CreateLobby()
    {
        try
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(_numberObParticipants);
            var relayServerData = allocation.ToRelayServerData("dtls");
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Create Lobby Error{e}");
            return false;
        }
    }
}
