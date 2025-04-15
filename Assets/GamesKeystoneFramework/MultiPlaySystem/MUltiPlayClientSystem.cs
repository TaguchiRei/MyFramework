using System;
using Cysharp.Threading.Tasks;
using Unity.Services.Lobbies;
using UnityEngine;

public class MUltiPlayClientSystem : MonoBehaviour
{
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
