using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{

    public static PlayerManager Instance;

    [SerializeField] private GameObject[] playerPrefabs;
    private List<PlayerInput> players = new List<PlayerInput>();
    private PlayerInputManager playerInputManager;

    [Header("Ajustes del Lobby")]
    public int minJugadoresParaIniciar = 1;
    public string nombreEscenaNivel1 = "Nivel1";

    [Header("Referencia UI")]
    public LobbyUIManager uiManager;


    public List<PlayerInput> Players { get => players; set => players = value; }

    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
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

        int index = player.playerIndex;

        DontDestroyOnLoad(player.gameObject);
        players.Add(player);
        if (players.Count < playerPrefabs.Length)
        {
            playerInputManager.playerPrefab = playerPrefabs[players.Count];
        }
        if (uiManager != null)
        {
            uiManager.ActualizarTarjetaConectada(index, player.currentControlScheme);
        }



    }

    public List<PlayerInput> ObtenerJugadores() => players;

    public void IniciarNivel()
    {
        if (players.Count >= minJugadoresParaIniciar)
        {
            if (playerInputManager != null) playerInputManager.DisableJoining();
            SceneManager.LoadScene(nombreEscenaNivel1);
        }
    }
}
