using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
public class PlayerMan : MonoBehaviour
{
    [Header("Arrastra los 4 personajes colocados en la Jerarquía")]
    [Tooltip("0: Tiempo, 1: Piromano, 2: Criogenico, 3: Transformador")]
    public PlayerAlquimia[] personajesEnEscena;

    private List<InputDevice> dispositivosUnidos = new List<InputDevice>();
    private int jugadoresUnidos = 0;

    public static PlayerMan Instance;
    public List<InputDevice> DispositivosUnidos => dispositivosUnidos;


    private void Awake()
    {
        Instance = this;

        // Desactiva personajes al iniciar la escena
        foreach (var p in personajesEnEscena)
        {
            if (p != null) p.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        // Si viene de un nivel anterior y hay controles guardados, unirlos
        if (DatosPartida.Instance != null && DatosPartida.Instance.DispositivosGuardados.Count > 0)
        {
            foreach (var dispositivo in DatosPartida.Instance.DispositivosGuardados)
            {
                UnirJugador(dispositivo);
            }
        }
    }

    private void Update()
    {
        if (jugadoresUnidos >= personajesEnEscena.Length) return;

        
        foreach (var gamepad in Gamepad.all)
        {
            if ((gamepad.startButton.wasPressedThisFrame || gamepad.buttonSouth.wasPressedThisFrame)
                && !dispositivosUnidos.Contains(gamepad))
            {
                UnirJugador(gamepad);
            }
        }

        
        if (Keyboard.current != null)
        {
            bool teclaPresionada = Keyboard.current.spaceKey.wasPressedThisFrame ||
                                  Keyboard.current.enterKey.wasPressedThisFrame ||
                                  Keyboard.current.qKey.wasPressedThisFrame;

            if (teclaPresionada && !dispositivosUnidos.Contains(Keyboard.current))
            {
                UnirJugador(Keyboard.current);
            }
        }
    }

    private void UnirJugador(InputDevice dispositivo)
    {
        if (jugadoresUnidos >= personajesEnEscena.Length) return;

        PlayerAlquimia personaje = personajesEnEscena[jugadoresUnidos];
        if (personaje == null) return;

        dispositivosUnidos.Add(dispositivo);
        personaje.gameObject.SetActive(true);

        PlayerInput pInput = personaje.GetComponent<PlayerInput>();
        if (pInput != null)
        {
            
            pInput.user.UnpairDevices();

            
            InputUser.PerformPairingWithDevice(dispositivo, pInput.user);

            
            string scheme = (dispositivo is Keyboard) ? "Player1" : "Control";
            pInput.SwitchCurrentControlScheme(scheme, dispositivo);
        }

        personaje.ConfigurarCuadranteUI(jugadoresUnidos);
        Debug.Log($"¡Jugador {jugadoresUnidos + 1} ({personaje.gameObject.name}) asignado a: {dispositivo.displayName}!");
        jugadoresUnidos++;
    }
}