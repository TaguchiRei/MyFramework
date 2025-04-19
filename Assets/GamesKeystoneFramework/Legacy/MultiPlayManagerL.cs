using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using UnityEngine;

namespace GamesKeystoneFramework.MultiPlaySystem
{
    public class MultiPlayManagerL : MonoBehaviour
    {
        [SerializeField] private GameObject MultiPlayObjectGroup;
        [Header("初期化完了しているかどうか")]
        public static bool CanMultiPlay;
    
        /// <summary>
        /// ロビーのリスト
        /// </summary>
        public static QueryLobbiesOptions QueryLobbiesOptions;
    
         public static MultiPlayManagerL Instance;
         
        

         /// <summary>
        /// シーン開始時に必ずこのメソッドを動かすこと
        /// </summary>
        public void SingletonInitialize()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
    
    
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
}
