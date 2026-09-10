using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
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

        Transform playerParent = player.transform.parent;
        playerParent.position = spawnPoints[players.Count - 1].position;
    }
}
