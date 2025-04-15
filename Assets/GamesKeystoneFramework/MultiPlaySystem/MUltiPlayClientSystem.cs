using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace GamesKeystoneFramework.MultiPlaySystem
{
    public class MUltiPlayClientSystem : MonoBehaviour
    {
        private List<Lobby> _lobbies;
    
        /// <summary>
        /// ロビーリストの取得に使う。
        /// </summary>
        /// <returns></returns>
        private async UniTask<bool> GetLobbyList()
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
                return false;
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
        /// <param name="lobbyCode"></param>
        /// <returns></returns>
        private async UniTask<bool> JoinLobby(string lobbyCode)
        {
            try
            {
                await LobbyService.Instance.JoinLobbyByCodeAsync("LobbyCode");
                return true;
            }
            catch(Exception e)
            {
                Debug.LogError($"Join Lobby Error{e.Message}");
                return false;
            }
        }
    }
}
