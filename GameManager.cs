using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ApiClient api;
    [SerializeField] private List<PlayerController> players;
    public string gameId;

    private void Start()
    {
        // Subscribe to the API's data received event
        api.OnDataReceived += OnDataReceived;

        // Start coroutines to automatically update and send player data
        StartCoroutine(UpdatePlayersDataRoutine());
        StartCoroutine(SendPlayersDataRoutine());
    }

    private IEnumerator UpdatePlayersDataRoutine()
    {
        while (true)
        {
            // Update data for all players
            for (int i = 0; i < players.Count; i++)
            {
                GetPlayerData(i);
            }
            yield return new WaitForSeconds(2f); // Wait for 2 seconds before the next update
        }
    }

    private IEnumerator SendPlayersDataRoutine()
    {
        while (true)
        {
            // Send data for all players
            SendAllPlayers();
            yield return new WaitForSeconds(2f); // Wait for 2 seconds before the next send
        }
    }

    public void GetPlayerData(int playerId)
    {
        // Validate playerId to ensure it's within range
        if (playerId < 0 || playerId >= players.Count)
        {
            Debug.LogWarning($"GetPlayerData: playerId {playerId} is out of range.");
            return;
        }

        // Request player data from the server
        StartCoroutine(api.GetPlayerData(gameId, playerId.ToString()));
    }

    public void OnDataReceived(int playerId, ServerData data)
    {
        // Validate playerId to ensure it's within range
        if (playerId < 0 || playerId >= players.Count)
        {
            Debug.LogWarning($"OnDataReceived: playerId {playerId} is out of range.");
            return;
        }

        // Update the player's position based on the received data
        Vector3 position = new Vector3(data.posX, data.posY, data.posZ);
        players[playerId].MovePlayer(position);
    }
    
    public void SendPlayerPosition(int playerId)
    {
        // Validate playerId to ensure it's within range
        if (playerId < 0 || playerId >= players.Count)
        {
            Debug.LogWarning($"SendPlayerPosition: playerId {playerId} is out of range.");
            return;
        }

        // Get the player's current position and send it to the server
        Vector3 position = players[playerId].GetPosition();
        ServerData data = new ServerData
        {
            posX = position.x,
            posY = position.y,
            posZ = position.z
        };
        StartCoroutine(api.PostPlayerData(gameId, playerId.ToString(), data));
    }

    public void DisconnectPlayer(int playerId)
    {
        // Validate playerId to ensure it's within range
        if (playerId < 0 || playerId >= players.Count)
        {
            Debug.LogWarning($"DisconnectPlayer: playerId {playerId} is out of range.");
            return;
        }

        // Deactivate the player's game object
        players[playerId].gameObject.SetActive(false);
        Debug.Log($"Player {playerId} disconnected.");
    }

    public void ReconnectPlayer(int playerId)
    {
        // Validate playerId to ensure it's within range
        if (playerId < 0 || playerId >= players.Count)
        {
            Debug.LogWarning($"ReconnectPlayer: playerId {playerId} is out of range.");
            return;
        }

        // Reactivate the player's game object and request their data from the server
        players[playerId].gameObject.SetActive(true);
        GetPlayerData(playerId);

        Debug.Log($"Player {playerId} reconnected.");
    }

    public void SendAllPlayers()
    {
        // Send the position of all players to the server
        for (int i = 0; i < players.Count; i++)
        {
            SendPlayerPosition(i);
        }
    }
}
