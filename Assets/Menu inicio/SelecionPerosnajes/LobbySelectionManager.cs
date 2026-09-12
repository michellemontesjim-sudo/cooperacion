using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LobbySelectionManager : MonoBehaviour
{
    public static LobbySelectionManager Instance;

    [Header("Prefabs de Roles (0: Tiempo, 1: Piromano, 2: Criogenico, 3: Transformador)")]
    public GameObject[] prefabsRoles;

    [Header("Ajustes del Lobby")]
    public int minJugadoresParaIniciar = 1;
    public string nombreEscenaNivel1 = "Nivel1";

    [Header("Referencia UI")]
    public LobbyUIManager uiManager;

    private List<PlayerInput> jugadores = new List<PlayerInput>();
    private PlayerInputManager inputManager;

    private void Awake()
    {
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

        inputManager = GetComponent<PlayerInputManager>();
    }

    private void Start()
    {
        // Asigna el prefab del Jugador 1 (Tiempo) al iniciar
        ActualizarPrefabSiguienteJugador();
    }

    private void Update()
    {
        // Si a�n falta por unirse el Jugador 2 (�ndice 1) y presiona la tecla M en el teclado
        if (Keyboard.current != null && Keyboard.current.mKey.wasPressedThisFrame)
        {
            // UnirJugadorSecundarioTeclado();
        }
    }

    // private void UnirJugadorSecundarioTeclado()
    // {
    //     // Solo une a J2 si no se ha unido todav�a
    //     if (jugadores.Count == 1 && prefabsRoles.Length > 1)
    //     {
    //         // Instancia a J2 forzando que comparta el teclado con J1
    //         PlayerInput j2Input = PlayerInput.Instantiate(
    //             prefabsRoles[1],
    //             playerIndex: 1,
    //             controlScheme: "Player2",
    //             splitScreenIndex: -1,
    //             pairWithDevice: Keyboard.current // Comp�rtelo con el teclado activo
    //         );

    //         // Importante: No llamar a OnPlayerJoined manualmente aqu�, 
    //         // PlayerInputManager o el evento lo registrar� si est� configurado,
    //         // pero si usas Instantiate manual, lo a�adimos a la lista:
    //         if (!jugadores.Contains(j2Input))
    //         {
    //             DontDestroyOnLoad(j2Input.gameObject);
    //             jugadores.Add(j2Input);

    //             if (uiManager != null)
    //                 uiManager.ActualizarTarjetaConectada(1, "Player2");

    //             ActualizarPrefabSiguienteJugador();
    //         }
    //     }
    // }

    private void OnEnable()
    {
        inputManager.onPlayerJoined += OnPlayerJoined;
    }

    private void OnDisable()
    {
        inputManager.onPlayerJoined -= OnPlayerJoined;
    }

    private void OnPlayerJoined(PlayerInput pInput)
    {

        int index = pInput.playerIndex;

        // // Bloquear el Control Scheme espec�fico seg�n el jugador que se une
        // if (index == 0)
        // {
        //     pInput.SwitchCurrentControlScheme("Keyboard");
        // }
        // else if (index >= 1)
        // {
        //     pInput.SwitchCurrentControlScheme("Control");
        // }

        DontDestroyOnLoad(pInput.gameObject);
        jugadores.Add(pInput);

        if (uiManager != null)
        {
            uiManager.ActualizarTarjetaConectada(index, pInput.currentControlScheme);
        }

        ActualizarPrefabSiguienteJugador();
    }

    private void ActualizarPrefabSiguienteJugador()
    {
        if (inputManager != null && prefabsRoles != null && jugadores.Count < prefabsRoles.Length)
        {
            inputManager.playerPrefab = prefabsRoles[jugadores.Count];
        }
    }

    public List<PlayerInput> ObtenerJugadores() => jugadores;

    public void IniciarNivel()
    {
        if (jugadores.Count >= minJugadoresParaIniciar)
        {
            if (inputManager != null) inputManager.DisableJoining();
            SceneManager.LoadScene(nombreEscenaNivel1);
        }
    }
}