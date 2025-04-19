using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GamesKeystoneFramework.Attributes;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace GamesKeystoneFramework.MultiPlaySystem
{
    public abstract class MultiPlayManager : MonoBehaviour
    {
        [SerializeField] private GameObject MultiPlayObjectGroup;
        [Header("初期化完了しているかどうか")]
        public static bool CanMultiPlay;
        
        [SerializeField, ReadOnlyInInspector] LobbyData lobbyData;
    
        /// <summary>
        /// ロビーのリスト
        /// </summary>
        public static QueryLobbiesOptions QueryLobbiesOptions;

        private Lobby _joinedLobby;
        
        /// <summary>
        /// ホストする際にサービスを初期化する。
        /// 一度だけ使用する
        /// 通信エラー発生時はfalseを返す
        /// </summary>
        public async UniTask<bool> ServicesInitialize()
        {
            //ゲーム側のサービス初期化
            try
            {
                await UnityServices.InitializeAsync();
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    //アカウントが無かった場合新たに作成
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }
            }
            catch(Exception e)
            {
                Debug.LogError($"Services Initialize Error{e.Message}");
                CanMultiPlay = false;
                return false;
            }
        
            Debug.Log("Services initialized");
            CanMultiPlay = true;
            return true;
        }

        /// <summary>
        /// 通信を切断する際に使用する
        /// </summary>
        /// <returns></returns>
        public async UniTask<bool> DisConnect()
        {
            if(!NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsServer)return false;
            try
            {
                NetworkManager.Singleton.Shutdown();
                await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Disconnect Error : {e.Message}");
                return false;
            }
        }

        public async UniTask<bool> HostConnect()
        {
            if(!CanMultiPlay) return false;
            
            var 
        }

        public GameObject InstantiateMultiObject(GameObject obj,Vector3 position)
        {
            if (obj == null)
            {
                Debug.Log("InstantiateMultiObject: obj is null");
                return null;
            }
            
            var spawnObj = Instantiate(obj,position,quaternion.identity);
            var objNetworkObj = spawnObj.GetComponent<NetworkObject>();
            if (objNetworkObj != null)
            {
                objNetworkObj.Spawn(true);
            }
            else
            {
                Debug.Log("NetworkObject Is Not found");
                Destroy(spawnObj);
                return null;
            }

            return spawnObj;
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
