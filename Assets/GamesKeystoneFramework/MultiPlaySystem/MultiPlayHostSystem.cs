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
        public string JoinCode { get; private set; }

        private CreateLobbyOptions createLobbyOptions;

        /// <summary>
        /// ロビーを作成する際に使用するメソッド。
        /// </summary>
        private async UniTask<bool> CreateLobby()
        {
            try
            {
                //Relayの割り当て
                var allocation = await RelayService.Instance.CreateAllocationAsync(lobbyData.MaxPlayers);

                //joinCode取得
                JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

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
                    createLobbyOptions.Data.Add("JoinCode", new DataObject(lobbyData.VisibilityOptions, JoinCode));
                }
                
                //ロビー作成
                await LobbyService.Instance.CreateLobbyAsync
                (lobbyData.LobbyName, lobbyData.MaxPlayers, createLobbyOptions);
                
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