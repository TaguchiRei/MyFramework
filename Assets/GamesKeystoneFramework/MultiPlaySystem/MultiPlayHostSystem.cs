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
        [Header("マルチプレイ参加可能人数"), SerializeField]
        private int _numberObParticipants;

        [SerializeField] LobbyData lobbyData;

        public string joinCode { get; private set; }

        private CreateLobbyOptions createLobbyOptions;

        /// <summary>
        /// ロビーを作成する際に使用するメソッド。
        /// </summary>
        private async UniTask<bool> CreateLobby()
        {
            
            try
            {
                //Relayの割り当て
                var allocation = await RelayService.Instance.CreateAllocationAsync(_numberObParticipants);

                //joinCode取得
                joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

                //Lobbyの作成
                createLobbyOptions = new CreateLobbyOptions
                {
                    IsPrivate = lobbyData.IsPrivate,
                    IsLocked = lobbyData.IsLocked,
                    Data = lobbyData.Data,
                };
                //joinCode追加
                if (!lobbyData.IsPrivate)
                {
                    createLobbyOptions.Data.Add("JoinCode", new DataObject(lobbyData.VisibilityOptions, joinCode));
                }
                
                //ロビー作成
                await LobbyService.Instance.CreateLobbyAsync
                (lobbyData.LobbyName, _numberObParticipants, createLobbyOptions);
                
                //Relayの接続設定
                var relayServerData = allocation.ToRelayServerData("dtls");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
                
                //ホストとして接続
                NetworkManager.Singleton.StartHost();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Create Lobby Error{e}");
                return false;
            }
        }
        

        /// <summary>
        /// ロビー作成時に設定する項目をまとめた構造体
        /// </summary>
        [Serializable]
        struct LobbyData
        {
            public string LobbyName;
            public string MaxPlayers;
            public bool IsPrivate;
            public bool IsLocked;
            public DataObject.VisibilityOptions VisibilityOptions;
            public Dictionary<string, DataObject> Data;
        }
    }
}