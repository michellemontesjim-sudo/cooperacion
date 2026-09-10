using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject[] playerPrefabs;
    private List<PlayerInput> players = new List<PlayerInput>();
    [SerializeField] private List<Transform> spawnPoints;

    private PlayerInputManager playerInputManager;


    public List<PlayerInput> Players { get => players; set => players = value; }

    private void Awake()
    {
        playerInputManager = FindAnyObjectByType<PlayerInputManager>();
    }



    private void OnEnable()
    {
        playerInputManager.onPlayerJoined += AddPlayer;
    }

    private void OnDisable()
    {
        playerInputManager.onPlayerJoined -= AddPlayer;
    }

    public void AddPlayer(PlayerInput player)
    {
        players.Add(player);
        if (players.Count < playerPrefabs.Length)
        {
            playerInputManager.playerPrefab = playerPrefabs[players.Count];
        }


        Transform playerParent = player.transform.parent;
        playerParent.position = spawnPoints[players.Count - 1].position;
    }
}
