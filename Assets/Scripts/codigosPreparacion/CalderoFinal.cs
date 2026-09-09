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
    public GameObject panelDerrotaUI;

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

        // Formatea la lista de procesos requeridos en un texto visual (Ej: "Proceso: Triturado -> Calentado -> Congelado")
        if (textoProcesoRequerido != null && ordenActual.secuenciaRequerida != null)
        {
            string secuenciaTexto = string.Join(" -> ", ordenActual.secuenciaRequerida);
            textoProcesoRequerido.text = $"Proceso: {secuenciaTexto}";
        }

        if (imagenIconoOrden != null && ordenActual.iconoResultado != null)
            imagenIconoOrden.sprite = ordenActual.iconoResultado;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (juegoTerminado) return;

        if (other.TryGetComponent<Ingrediente>(out var ingrediente))
        {
            // Valida que el nombre coincida y que el historial del ingrediente sea idéntico a la secuencia de la orden
            if (ingrediente.nombreIngrediente == ordenActual.nombreReceta &&
                ingrediente.ValidarSecuencia(ordenActual.secuenciaRequerida))
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
                Debug.Log("Ingrediente o secuencia de procesos incorrecta. Se descartó la entrega.");
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

    public OrdenReceta ObtenerOrdenActual()
    {
        return ordenActual;
    }

    public void EvaluarFinDeTiempo()
    {
        if (juegoTerminado) return;

        juegoTerminado = true;

        if (puntajeTotal >= puntosParaGanar)
        {
            GanarPartida();
        }
        else
        {
            panelDerrotaUI.SetActive(true);
        }
    }
}