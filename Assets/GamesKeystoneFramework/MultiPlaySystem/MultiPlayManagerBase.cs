using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GamesKeystoneFramework.Attributes;
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
        
        [Header("ホストする際のロビー作成時に使用するデータ")]
        [SerializeField] LobbyData lobbyData;
        
        /// <summary>
        /// 初期化完了しているか
        /// </summary>
        public static bool CanMultiPlay;
        [ReadOnly] public ConnectionStatus ConnectionStatus;
        
        /// <summary>
        /// 取得済みロビーリスト
        /// </summary>
        protected List<Lobby> LobbyList;
        /// <summary>
        /// 参加済みロビー
        /// </summary>
        protected Lobby JoinedLobby;

        private UnityTransport _unityTransport;
        
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
        //--------------------ホストサイド------------------------

        /// <summary>
        /// ロビーを作成する。必ずlobbyDataを設定してから実行すること
        /// </summary>
        /// <returns></returns>
        public async UniTask<bool> ConnectionHost()
        {
            //------------アロケーション取得----------------
            Allocation allocation;
            try
            {
                allocation = await RelayService.Instance.CreateAllocationAsync(lobbyData.MaxPlayers);
            }
            catch (Exception e)
            {
                Debug.LogError($"Get Allocation Error : {e.Message}");
                return false;
            }

            //--------------リレー設定---------------------
            string relayJoinCode;
            try
            {
                relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

                var relayServerData = allocation.ToRelayServerData("dtls");
                
                _unityTransport.SetRelayServerData(relayServerData);
            }
            catch(Exception e)
            {
                Debug.LogError($"Get JoinCode Error : {e.Message}");
                return false;
            }
            
            //--------------ロビー作成---------------------
            
            try
            {
                var createLobbyOptions = new CreateLobbyOptions
                {
                    IsPrivate = lobbyData.IsPrivate,
                    Data = lobbyData.Data,
                };
                if (!lobbyData.IsPrivate)
                {
                    Debug.Log($"Add JoinCode :{relayJoinCode}");
                    createLobbyOptions.Data.Add("RelayJoinCode",new DataObject(lobbyData.VisibilityOptions,relayJoinCode));
                }

                await LobbyService.Instance.CreateLobbyAsync(
                    lobbyData.LobbyName,
                    lobbyData.MaxPlayers,
                    createLobbyOptions);
            }
            catch (Exception e)
            {
                Debug.LogError($"Create Lobby Error : {e.Message}");
                return false;
            }

            //-------------ホスト接続--------------
            try
            {
                NetworkManager.Singleton.StartClient();
                Debug.Log($"IsServer{NetworkManager.Singleton.IsServer}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Host Connection Error : {e.Message}");
                return false;
            }
            return true;
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

    public enum ConnectionStatus
    {
        OffLine,
        ConnectedLobby,
        ConnectedRelay
    }
}
