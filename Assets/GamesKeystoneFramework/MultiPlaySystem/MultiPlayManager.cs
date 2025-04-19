using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace GamesKeystoneFramework.MultiPlaySystem
{
    public abstract class MultiPlayManagerBase : MonoBehaviour
    {
        [Header("マルチプレイオブジェクトをこのオブジェクトの子要素にする")]
        [SerializeField] protected GameObject MultiPlayObjectGroup;
        
        [SerializeField] protected LobbyData lobbyData;
        
        /// <summary>
        /// 初期化完了しているか
        /// </summary>
        public static bool CanMultiPlay;
        
        protected List<Lobby> LobbyList;

        private UnityTransport _unityTransport;

        protected Lobby JoinedLobby;
        
        /// <summary>
        /// 初期化を行う
        /// </summary>
        /// <returns></returns>
        protected async UniTask<bool> ServiceInitialize()
        {
            CanMultiPlay = false;
            try
            {
                _unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                Debug.Log("Unity Transport Set");
                await UnityServices.InitializeAsync();
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }
                CanMultiPlay = true;
                Debug.Log("Service Initialized");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Services Initialize Error : {e}");
                return false;
            }
        }

        /// <summary>
        /// ロビーリストを取得する
        /// </summary>
        /// <returns></returns>
        protected async UniTask<(bool,List<Lobby>)> GetAllLobbyList()
        {
            try
            {
                var lobbyList = await LobbyService.Instance.QueryLobbiesAsync();
                return (true, lobbyList.Results);
            }
            catch (Exception e)
            {
                Debug.LogError($"Get Lobby List Error : {e}");
                return (false,null);
            }
        }
        /// <summary>
        /// ロビーがまだ存在するかを調べる
        /// </summary>
        /// <param name="lobbyId"></param>
        /// <returns></returns>
        protected async UniTask<bool> LobbyCheck(string lobbyId)
        {
            try
            { 
                await LobbyService.Instance.GetLobbyAsync(lobbyId);
                Debug.Log("Lobby Found");
                return true;
            }
            catch (Exception e)
            {
                Debug.Log("Lobby Not Found");
                return false;
            }
        }
        
        //--------------------クライアントサイド------------------------

        /// <summary>
        /// LobbyIDを利用してロビーに参加する
        /// </summary>
        /// <param name="lobbyId"></param>
        /// <returns></returns>
        protected async UniTask<bool> JoinLobbyFromLobbyID(string lobbyId)
        {
            try
            {
                var check = await LobbyCheck(lobbyId);
                if (!check) return false;
                
                JoinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
                Debug.Log("Join Success");
                return true;

            }
            catch(Exception e)
            {
                Debug.LogError($"Join Lobby Error : {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// ロビーリストからロビーに参加する
        /// </summary>
        /// <param name="LobbyNumber"></param>
        /// <returns></returns>
        protected async UniTask<bool> JoinLobbyFromLobbyList(int LobbyNumber)
        {
            try
            {
                var lobbyId = LobbyList[LobbyNumber].Id;
                var check = await LobbyCheck(lobbyId);
                if (!check) return false;
                
                JoinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
                Debug.Log("Join Success");
                return true;

            }
            catch (Exception e)
            {
                Debug.LogError($"Join Lobby Error : {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Relayに参加するために使用。Lobby参加後に使用
        /// Relayに参加するために使用。Lobby参加後に使用
        /// </summary>
        /// <returns></returns>
        protected async UniTask<bool> JoinRelay()
        {
            try
            {
                var joinCode = JoinedLobby.Data["RelayJoinCode"].Value;
                var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                Debug.Log("join Allocation");
                _unityTransport.SetRelayServerData(
                    allocation.RelayServer.IpV4,
                    (ushort)allocation.RelayServer.Port,
                    allocation.AllocationIdBytes,
                    allocation.Key,
                    allocation.ConnectionData,
                    allocation.HostConnectionData);

                NetworkManager.Singleton.StartClient();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Join Relay Error : {e.Message}");
                return false;
            }
        }
    }
    
    /// <summary>
    /// ロビー作成時に設定する項目をまとめた構造体
    /// </summary>
    [Serializable]
    public struct LobbyData
    {
        public string LobbyName;
        public int MaxPlayers;
        public bool IsPrivate;
        public DataObject.VisibilityOptions VisibilityOptions;
        public Dictionary<string, DataObject> Data;
    }
}
