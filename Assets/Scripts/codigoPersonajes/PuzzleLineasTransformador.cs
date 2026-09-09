using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class PuzzleLineasTransformador : MinijuegoBase
{
    [Header("Configuración de Nodos")]
    public Button[] nodosSecuencia; // Arrastra los botones en el orden correcto

    private int pasoEsperado = 0;

    public override void InicializarPuzzle(PlayerInput pInput)
    {
        base.InicializarPuzzle(pInput);
        pasoEsperado = 0;

        for (int i = 0; i < nodosSecuencia.Length; i++)
        {
            int index = i; // Copia local para la clausura del evento

            // Reasigna el texto del botón para indicar su número en la secuencia
            TextMeshProUGUI textoNodo = nodosSecuencia[i].GetComponentInChildren<TextMeshProUGUI>();
            if (textoNodo != null) textoNodo.text = (i + 1).ToString();

            // Configura el estado del botón y limpia eventos previos
            nodosSecuencia[i].interactable = true;
            nodosSecuencia[i].onClick.RemoveAllListeners();

            // Conecta el clic con la validación de su índice
            nodosSecuencia[i].onClick.AddListener(() => PresionarNodo(index));
        }
    }

    private void PresionarNodo(int indicePresionado)
    {
        // Si el botón presionado coincide con el paso actual de la secuencia
        if (indicePresionado == pasoEsperado)
        {
            nodosSecuencia[indicePresionado].interactable = false; // Desactiva el nodo completado
            pasoEsperado++;

            // Si se completaron todos los nodos en orden ascendente
            if (pasoEsperado >= nodosSecuencia.Length)
            {
                OnPuzzleExito?.Invoke();
            }
        }
        else
        {
            // Error en el orden: reinicia el minijuego o dispara fallo
            OnPuzzleFallo?.Invoke();
        }
    }
}