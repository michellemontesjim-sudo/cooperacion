using UnityEngine;

public class MesaJugador : MonoBehaviour
{
    [Header("Asignación de Rol / Jugador")]
    [Tooltip("0: Tiempo, 1: Piromano, 2: Criogenico, 3: Transformador")]
    public int idJugadorAsignado = 0;

    private bool jugadorEstaEnMesa = false;

    private void OnTriggerEnter(Collider other)
    {
        // 💡 Busca el script en el objeto, en sus hijos o en sus padres
        PlayerAlquimia player = other.GetComponent<PlayerAlquimia>()
            ?? other.GetComponentInChildren<PlayerAlquimia>()
            ?? other.GetComponentInParent<PlayerAlquimia>();

        if (player != null)
        {
            if (player.playerIndex == idJugadorAsignado)
            {
                jugadorEstaEnMesa = true;
                player.MesaActual = this;
                Debug.Log($"[Mesa] ¡Mesa {idJugadorAsignado} habilitada para Jugador {player.playerIndex}!");
            }
            else
            {
                Debug.LogWarning($"[Mesa] Jugador {player.playerIndex} intentó usar la Mesa {idJugadorAsignado}.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerAlquimia player = other.GetComponent<PlayerAlquimia>()
            ?? other.GetComponentInChildren<PlayerAlquimia>()
            ?? other.GetComponentInParent<PlayerAlquimia>();

        if (player != null && player.playerIndex == idJugadorAsignado)
        {
            jugadorEstaEnMesa = false;
            player.MesaActual = null;
            Debug.Log($"[Mesa] Jugador {player.playerIndex} se alejó de su mesa.");
        }
    }

    public bool PuedoProcesar()
    {
        return jugadorEstaEnMesa;
    }
}