using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace GamesKeystoneFramework.MultiPlaySystem
{
    public class MultiPlayHostSystem : MonoBehaviour
    {
        [SerializeField] LobbyData lobbyData;

        /// <summary>
        /// 作成したロビーのjoinCode
        /// </summary>
        public string RelayJoinCode { get; private set; }

        private CreateLobbyOptions createLobbyOptions = new();

        private Allocation _allocation;

        /// <summary>
        /// ロビーを作成する際に使用するメソッド。
        /// </summary>
        public async UniTask<bool> CreateLobby()
        {
            try
            {
                //Relayの割り当て
                _allocation = await RelayService.Instance.CreateAllocationAsync(lobbyData.MaxPlayers);
                lobbyData.Data ??= new();

                //Lobbyの作成
                createLobbyOptions = new CreateLobbyOptions
                {
                    IsPrivate = lobbyData.IsPrivate,
                    IsLocked = lobbyData.IsLocked,
                    Data = lobbyData.Data,
                };
                
                //RelayJoinCode取得と追加
                RelayJoinCode = await RelayService.Instance.GetJoinCodeAsync(_allocation.AllocationId);
                if (!lobbyData.IsPrivate)
                {
                    Debug.Log($"Add JoinCode : {RelayJoinCode}");
                    createLobbyOptions.Data.Add("RelayJoinCode",
                        new DataObject(lobbyData.VisibilityOptions, RelayJoinCode));
                }

                //ロビー作成
                await LobbyService.Instance.CreateLobbyAsync
                    (lobbyData.LobbyName, lobbyData.MaxPlayers, createLobbyOptions);

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Create Lobby Error :{e}");
                return false;
            }
        }

        public async UniTask<bool> RelaySetting()
        {
            Debug.Log(createLobbyOptions.Data.Count + "--------------------------------");
            
            try
            {
                
                //Relayの接続設定
                var relayServerData = _allocation.ToRelayServerData("dtls");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
                Debug.Log(createLobbyOptions.Data.Count + "--------------------------------");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Relay Setting Error :{e.Message}");
                return false;
            }
        }

        public bool ConnectionHost()
        {
            //ホストとして接続
            try
            {
                NetworkManager.Singleton.StartHost();
                Debug.Log($"IsServer{NetworkManager.Singleton.IsServer}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Connection Host Error :{e.Message}");
                return false;
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
            public bool IsLocked;
            public DataObject.VisibilityOptions VisibilityOptions;
            public Dictionary<string, DataObject> Data;
        }
    }
}