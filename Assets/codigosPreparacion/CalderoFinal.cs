using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CalderoFinal : MonoBehaviour
{
    [Header("Recetas y Meta de Puntos")]
    public OrdenReceta[] recetasPosibles;
    public int puntosParaGanar = 500; // Meta mínima para ganar

    [Header("Interfaz de Usuario (UI)")]
    public TextMeshProUGUI textoNombreOrden;
    public TextMeshProUGUI textoProcesoRequerido;
    public Image imagenIconoOrden;
    public TextMeshProUGUI textoPuntajeTotal;
    public GameObject panelVictoriaUI; // Panel desplegable de victoria

    private OrdenReceta ordenActual;
    private int puntajeTotal = 0;
    private bool juegoTerminado = false;

    private void Start()
    {
        if (panelVictoriaUI != null) panelVictoriaUI.SetActive(false);
        ActualizarTextoPuntos();
        GenerarNuevaOrden();
    }

    public void GenerarNuevaOrden()
    {
        if (juegoTerminado || recetasPosibles == null || recetasPosibles.Length == 0) return;

        int randomIndex = Random.Range(0, recetasPosibles.Length);
        ordenActual = recetasPosibles[randomIndex];

        if (textoNombreOrden != null)
            textoNombreOrden.text = ordenActual.nombreReceta;

        if (textoProcesoRequerido != null)
            textoProcesoRequerido.text = $"Proceso: {ordenActual.estadoRequerido}";

        if (imagenIconoOrden != null && ordenActual.iconoResultado != null)
            imagenIconoOrden.sprite = ordenActual.iconoResultado;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (juegoTerminado) return;

        if (other.TryGetComponent<Ingrediente>(out var ingrediente))
        {
            // Valida el nombre y el estado procesado
            if (ingrediente.nombreIngrediente == ordenActual.nombreReceta && ingrediente.estadoActual == ordenActual.estadoRequerido)
            {
                puntajeTotal += ordenActual.puntosRecompensa;
                ActualizarTextoPuntos();

                Destroy(other.gameObject);

                // Comprobación de Meta
                if (puntajeTotal >= puntosParaGanar)
                {
                    GanarPartida();
                }
                else
                {
                    GenerarNuevaOrden();
                }
            }
            else
            {
                Debug.Log("Ingrediente o proceso incorrecto. Se descartó la entrega.");
                Destroy(other.gameObject);
            }
        }
    }

    private void ActualizarTextoPuntos()
    {
        if (textoPuntajeTotal != null)
            textoPuntajeTotal.text = $"Puntos: {puntajeTotal} / {puntosParaGanar}";
    }

    private void GanarPartida()
    {
        juegoTerminado = true;
        Debug.Log("¡VICTORIA! Han completado los pedidos solicitados.");

        if (panelVictoriaUI != null)
        {
            panelVictoriaUI.SetActive(true);
        }
    }
}