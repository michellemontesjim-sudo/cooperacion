using UnityEngine;

public class UIPuzzleManager : MonoBehaviour 
{
    public static UIPuzzleManager Instance;

    [Header("Cuadrantes UI Privados (0 = P1, 1 = P2, 2 = P3, 3 = P4)")]
    public RectTransform[] panelesUIJugadores;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public RectTransform ObtenerPanelJugador(int playerIndex)
    {
        if (panelesUIJugadores != null && playerIndex >= 0 && playerIndex < panelesUIJugadores.Length)
        {
            return panelesUIJugadores[playerIndex];
        }

        Debug.LogWarning($"No se encontró un cuadrante UI para el índice de jugador: {playerIndex}");
        return null;
    }
}
