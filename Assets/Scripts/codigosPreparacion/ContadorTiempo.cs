using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ContadorTiempo : MonoBehaviour
{
    [Header("Configuración de Tiempo")]
    public float tiempoLimiteSegundos = 180f;
    private float tiempoRestante;
    private bool cuentaActiva = false;

    [Header("Interfaz de Usuario")]
    public TextMeshProUGUI textoTiempo;
    public Image barraFillTiempo;
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
        // formato minutos y segundos
        int minutos = Mathf.FloorToInt(tiempoRestante / 60);
        int segundos = Mathf.FloorToInt(tiempoRestante % 60);

        if (textoTiempo != null)
        {
            textoTiempo.text = string.Format("{0:00}:{1:00}", minutos, segundos);

            
            if (tiempoRestante <= 30f)
            {
                textoTiempo.color = colorAlerta;
            }
        }

        
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