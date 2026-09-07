using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CalderoFinal : MonoBehaviour
{
    [Header("Recetas Disponibles en el Nivel")]
    public OrdenReceta[] recetasPosibles;

    [Header("Interfaz de Usuario (UI)")]
    public TextMeshProUGUI textoNombreOrden;
    public TextMeshProUGUI textoProcesoRequerido;
    public Image imagenIconoOrden;
    public TextMeshProUGUI textoPuntajeTotal;

    private OrdenReceta ordenActual;
    private int puntajeTotal = 0;

    private void Start()
    {
        GenerarNuevaOrden();
    }

    public void GenerarNuevaOrden()
    {
        if (recetasPosibles == null || recetasPosibles.Length == 0) return;

        // Selecciona una receta al azar de la lista
        int randomIndex = Random.Range(0, recetasPosibles.Length);
        ordenActual = recetasPosibles[randomIndex];

        // Actualizar la interfaz gráfica
        if (textoNombreOrden != null)
            textoNombreOrden.text = ordenActual.nombreReceta;

        if (textoProcesoRequerido != null)
            textoProcesoRequerido.text = $"Proceso: {ordenActual.estadoRequerido}";

        if (imagenIconoOrden != null && ordenActual.iconoResultado != null)
            imagenIconoOrden.sprite = ordenActual.iconoResultado;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Ingrediente>(out var ingrediente))
        {
            // Validar si el ingrediente entregado coincide con el nombre y el estado pedido
            if (ingrediente.nombreIngrediente == ordenActual.nombreReceta && ingrediente.estadoActual == ordenActual.estadoRequerido)
            {
                puntajeTotal += ordenActual.puntosRecompensa;

                if (textoPuntajeTotal != null)
                    textoPuntajeTotal.text = $"Puntos: {puntajeTotal}";

                Debug.Log($"¡Entrega Correcta! +{ordenActual.puntosRecompensa} Puntos.");

                Destroy(other.gameObject);
                GenerarNuevaOrden(); // Pide la siguiente orden inmediatamente
            }
            else
            {
                Debug.Log("Receta equivocada o proceso incompleto. Ingrediente descartado.");
                Destroy(other.gameObject);
            }
        }
    }
}