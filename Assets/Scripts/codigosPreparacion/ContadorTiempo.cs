using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ContadorTiempo : MonoBehaviour
{
    [Header("Configuración de Tiempo")]
    public float tiempoLimiteSegundos = 180f; // Ej: 3 minutos (180s)
    private float tiempoRestante;
    private bool cuentaActiva = false;

    [Header("Interfaz de Usuario")]
    public TextMeshProUGUI textoTiempo;
    public Image barraFillTiempo; // Opcional: Barra visual circular o recta
    public Color colorAlerta = Color.red;

    [Header("Referencias del Nivel")]
    public CalderoFinal caldero;

    private void Start()
    {
        tiempoRestante = tiempoLimiteSegundos;
        cuentaActiva = true;
    }

    private void Update()
    {
        if (!cuentaActiva) return;

        if (tiempoRestante > 0)
        {
            tiempoRestante -= Time.deltaTime;
            ActualizarUI();
        }
        else
        {
            tiempoRestante = 0;
            cuentaActiva = false;
            ActualizarUI();
            TiempoAgotado();
        }
    }

    private void ActualizarUI()
    {
        // Convierte el float a formato de minutos y segundos (MM:SS)
        int minutos = Mathf.FloorToInt(tiempoRestante / 60);
        int segundos = Mathf.FloorToInt(tiempoRestante % 60);

        if (textoTiempo != null)
        {
            textoTiempo.text = string.Format("{0:00}:{1:00}", minutos, segundos);

            // Alerta visual cuando quedan menos de 30 segundos
            if (tiempoRestante <= 30f)
            {
                textoTiempo.color = colorAlerta;
            }
        }

        // Si usas barra visual con Image Type = Filled
        if (barraFillTiempo != null)
        {
            barraFillTiempo.fillAmount = tiempoRestante / tiempoLimiteSegundos;
        }
    }

    private void TiempoAgotado()
    {
        Debug.Log("¡El tiempo ha terminado!");
        if (caldero != null)
        {
            caldero.EvaluarFinDeTiempo();
        }
    }
}