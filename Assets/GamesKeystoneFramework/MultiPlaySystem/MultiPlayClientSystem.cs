using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using UnityEngine;

namespace GamesKeystoneFramework.MultiPlaySystem
{
    public class MultiPlayClientSystem : MonoBehaviour
    {
        public List<Lobby> _lobbies = new();
    
        /// <summary>
        /// ロビーリストの取得に使う。
        /// </summary>
        /// <returns></returns>
        public async UniTask<bool> GetLobbyList()
        {
            try
            {
                var response = await LobbyService.Instance.QueryLobbiesAsync();
                _lobbies = response.Results;
                if (_lobbies.Count == 0)
                {
                    Debug.Log("Lobby Not Found");
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Get Lobby List Error{e.Message}");
                return false;
            }
        }
        /// <summary>
        /// ロビーコードを利用したロビー参加に使う
        /// </summary>
        /// <param name="lobbyId"></param>
        /// <returns></returns>
        public async UniTask<bool> JoinLobby(string lobbyId)
        {
            try
            {
                await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
                return true;
            }
            catch(Exception e)
            {
                Debug.LogError($"Join Lobby Error{e.Message}");
                return false;
            }
        }

        public async UniTask<bool> JoinRelay(string joinCode)
        {
            try
            {
                var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                unityTransport.SetRelayServerData(
                    allocation.RelayServer.IpV4,
                    (ushort)allocation.RelayServer.Port,
                    allocation.AllocationIdBytes,
                    allocation.Key,
                    allocation.ConnectionData,
                    allocation.HostConnectionData);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Join Relay Error{e.Message}");
                return false;
            }
        }
    }
}
