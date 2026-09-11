using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyUIManager : MonoBehaviour
{
    [System.Serializable]
    public struct TarjetaJugador
    {
        public GameObject panelPresionaUnirse; // Objeto visual "Presiona A"
        public GameObject panelJugadorListo;   // Objeto visual "¡CONECTADO!"
        public TextMeshProUGUI textoControl;  // Muestra "Teclado P1", "Gamepad", etc.
    }

    [Header("Tarjetas de UI (0:Tiempo, 1:Piromano, 2:Criogenico, 3:Transformador)")]
    public TarjetaJugador[] tarjetas;

    [Header("Boton de Inicio")]
    public Button botonIniciarJuego;

    private void Start()
    {
        // Al arrancar la escena, todas las tarjetas piden unirse
        for (int i = 0; i < tarjetas.Length; i++)
        {
            tarjetas[i].panelPresionaUnirse.SetActive(true);
            tarjetas[i].panelJugadorListo.SetActive(false);
        }

        if (botonIniciarJuego != null)
            botonIniciarJuego.interactable = false;
    }

    // Llama a este método desde el evento OnPlayerJoined de tu LobbySelectionManager
    public void ActualizarTarjetaConectada(int index, string esquemaControl)
    {
        if (index < tarjetas.Length)
        {
            tarjetas[index].panelPresionaUnirse.SetActive(false);
            tarjetas[index].panelJugadorListo.SetActive(true);

            if (tarjetas[index].textoControl != null)
            {
                tarjetas[index].textoControl.text = $"Control: {esquemaControl}";
            }
        }

        // Si ya hay al menos 1 jugador (o la cantidad mínima), habilita el botón
        if (botonIniciarJuego != null)
            botonIniciarJuego.interactable = true;
    }
}