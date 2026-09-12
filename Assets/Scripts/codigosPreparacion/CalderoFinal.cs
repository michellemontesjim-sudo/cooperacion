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
    public int resetasSimultaneas = 2;
    public int puntosParaGanar = 500;

    [Header("Transición de Nivel (Victoria)")]
    public string nombreSiguienteNivel = "Nivel_02";
    public bool cambiarNivelAutomatico = true;
    public float tiempoEsperaCambioEscena = 3f;

    [Header("Reinicio de Nivel (Derrota / Fin de Tiempo)")]
    public bool reiniciarAutomaticoEnDerrota = true;
    public float tiempoEsperaReiniciar = 3f;

    [Header("Interfaz de Usuario (UI)")]
    public TextMeshProUGUI textoProcesoRequerido1;
    public TextMeshProUGUI textoProcesoRequerido2;
    public Image imagenIconoOrden1;
    public Image imagenIconoOrden2;
    public TextMeshProUGUI textoPuntajeTotal;
    public GameObject panelVictoriaUI;
    public GameObject panelDerrotaUI;

    private OrdenReceta ordenActual1;
    private OrdenReceta ordenActual2;
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

        ordenActual1 = recetasPosibles[Random.Range(0, recetasPosibles.Length)];
        ordenActual2 = recetasPosibles[Random.Range(1, recetasPosibles.Length)];

        if (resetasSimultaneas >= 1)
        {

            if (textoProcesoRequerido1 != null && ordenActual1.secuenciaRequerida != null)
            {
                string secuenciaTexto1 = string.Join(" - ", ordenActual1.secuenciaRequerida);
                textoProcesoRequerido1.text = secuenciaTexto1;
            }

            if (textoProcesoRequerido2 != null && ordenActual2.secuenciaRequerida != null)
            {
                string secuenciaTexto2 = string.Join(" - ", ordenActual2.secuenciaRequerida);
                textoProcesoRequerido2.text = secuenciaTexto2;
            }

            if (imagenIconoOrden1 != null && ordenActual1.iconoResultado != null)
                imagenIconoOrden1.sprite = ordenActual1.iconoResultado;

            if (imagenIconoOrden2 != null && ordenActual2.iconoResultado != null)
                imagenIconoOrden2.sprite = ordenActual2.iconoResultado;
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (juegoTerminado) return;

        if (other.TryGetComponent<Ingrediente>(out var ingrediente))
        {
            if (CoincideConOrden(ingrediente, ordenActual1))
            {
                puntajeTotal += ordenActual1.puntosRecompensa;
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
            else if (CoincideConOrden(ingrediente, ordenActual2))
            {
                puntajeTotal += ordenActual2.puntosRecompensa;
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

    private bool CoincideConOrden(Ingrediente ingrediente, OrdenReceta orden)
    {
        return ingrediente.nombreIngrediente == orden.nombreReceta &&
               ingrediente.ValidarSecuencia(orden.secuenciaRequerida);
    }

    private void ActualizarTextoPuntos()
    {
        if (textoPuntajeTotal != null)
            textoPuntajeTotal.text = puntajeTotal.ToString();
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
        return ordenActual1;
    }

    public OrdenReceta ObtenerOrdenActual2()
    {
        return ordenActual2;
    }
}
