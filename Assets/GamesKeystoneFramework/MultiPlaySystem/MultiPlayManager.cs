using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace GamesKeystoneFramework.MultiPlaySystem
{
    public abstract class MultiPlayManagerBase : MonoBehaviour
    {
        [Header("マルチプレイオブジェクトをこのオブジェクトの子要素にする")]
        [SerializeField] protected GameObject MultiPlayObjectGroup;
        
        [SerializeField] LobbyData lobbyData;
        
        /// <summary>
        /// 初期化完了しているか
        /// </summary>
        public static bool CanMultiPlay;

        
        /// <summary>
        /// 初期化を行う
        /// </summary>
        /// <returns></returns>
        protected async UniTask<bool> ServiceInitialize()
        {
            CanMultiPlay = false;
            try
            {
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
                Debug.LogError($"Services Initialize Error: {e}");
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
                Debug.LogError($"Get Lobby List Error: {e}");
                return (false,null);
            }
        }
        
        //--------------------クライアントサイド------------------------
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
