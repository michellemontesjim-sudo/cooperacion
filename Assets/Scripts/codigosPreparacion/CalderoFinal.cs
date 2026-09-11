using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class CalderoFinal : MonoBehaviour
{
    [Header("Recetas y Meta de Puntos")]
    public OrdenReceta[] recetasPosibles;
    public int puntosParaGanar = 500;

    [Header("Transición de Nivel (Victoria)")]
    public string nombreSiguienteNivel = "Nivel_02";
    public bool cambiarNivelAutomatico = true;
    public float tiempoEsperaCambioEscena = 3f;

    [Header("Reinicio de Nivel (Derrota / Fin de Tiempo)")] // 👈 Nuevas opciones en el Inspector
    public bool reiniciarAutomaticoEnDerrota = true;
    public float tiempoEsperaReiniciar = 3f;

    [Header("Interfaz de Usuario (UI)")]
    public TextMeshProUGUI textoNombreOrden;
    public TextMeshProUGUI textoProcesoRequerido;
    public Image imagenIconoOrden;
    public TextMeshProUGUI textoPuntajeTotal;
    public GameObject panelVictoriaUI;
    public GameObject panelDerrotaUI;

    private OrdenReceta ordenActual;
    private int puntajeTotal = 0;
    private bool juegoTerminado = false;

    private void Awake()
    {
        if (panelVictoriaUI != null) panelVictoriaUI.SetActive(false);
        if (panelDerrotaUI != null) panelDerrotaUI.SetActive(false);
    }

    private void Start()
    {
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
            if (ingrediente.nombreIngrediente == ordenActual.nombreReceta &&
                ingrediente.ValidarSecuencia(ordenActual.secuenciaRequerida))
            {
                puntajeTotal += ordenActual.puntosRecompensa;
                ActualizarTextoPuntos();

                Destroy(other.gameObject);

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
        if (juegoTerminado) return;
        juegoTerminado = true;

        Debug.Log("¡VICTORIA! Han completado los pedidos solicitados.");

        GuardarControlesJugadores();

        if (panelVictoriaUI != null)
            panelVictoriaUI.SetActive(true);

        if (cambiarNivelAutomatico)
        {
            Invoke(nameof(CargarSiguienteNivel), tiempoEsperaCambioEscena);
        }
    }

    // derrota por tiempo
    public void EvaluarFinDeTiempo()
    {
        if (juegoTerminado) return;

        if (puntajeTotal >= puntosParaGanar)
        {
            GanarPartida();
        }
        else
        {
            DerrotaPartida();
        }
    }

    private void DerrotaPartida()
    {
        if (juegoTerminado) return;
        juegoTerminado = true;

        Debug.Log("¡DERROTA! Se ha agotado el tiempo.");

        // guardar mandos para no perder asignaciones
        GuardarControlesJugadores();

        // pantalla derrota
        if (panelDerrotaUI != null)
        {
            panelDerrotaUI.SetActive(true);
        }

        // 3. reinicio de nivel
        if (reiniciarAutomaticoEnDerrota)
        {
            Invoke(nameof(ReiniciarNivelActual), tiempoEsperaReiniciar);
        }
    }

    public void ReiniciarNivelActual()
    {
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void GuardarControlesJugadores()
    {
        if (PlayerMan.Instance != null && DatosPartida.Instance != null)
        {
            DatosPartida.Instance.DispositivosGuardados = new List<InputDevice>(PlayerMan.Instance.DispositivosUnidos);
            Debug.Log($"[CalderoFinal] Se guardaron {DatosPartida.Instance.DispositivosGuardados.Count} controles.");
        }
    }

    public void CargarSiguienteNivel()
    {
        if (!string.IsNullOrEmpty(nombreSiguienteNivel))
        {
            SceneManager.LoadScene(nombreSiguienteNivel);
        }
        else
        {
            Debug.LogError("[CalderoFinal] No has asignado el nombre de la siguiente escena en el Inspector.");
        }
    }

    public OrdenReceta ObtenerOrdenActual()
    {
        return ordenActual;
    }
}