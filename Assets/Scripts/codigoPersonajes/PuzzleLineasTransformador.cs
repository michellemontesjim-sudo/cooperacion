using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PuzzleLineasTransformador : MinijuegoBase
{
    [Header("Configuración de Nodos")]
    public Button[] nodosSecuencia;

    private int pasoEsperado = 0;
    private PlayerInput miPlayerInput;

    public override void InicializarPuzzle(PlayerInput pInput)
    {
        base.InicializarPuzzle(pInput);
        miPlayerInput = pInput;
        pasoEsperado = 0;

        for (int i = 0; i < nodosSecuencia.Length; i++)
        {
            int index = i;
            TextMeshProUGUI textoNodo = nodosSecuencia[i].GetComponentInChildren<TextMeshProUGUI>();
            if (textoNodo != null) textoNodo.text = (i + 1).ToString();

            nodosSecuencia[i].interactable = true;
            nodosSecuencia[i].onClick.RemoveAllListeners();
            nodosSecuencia[i].onClick.AddListener(() => PresionarNodo(index));
        }

        // Enfocar el primer nodo activo al iniciar
        EnfocarSiguienteNodoDisponible();
    }

    private void Update()
    {
        if (miPlayerInput != null && miPlayerInput.actions != null)
        {
            if (miPlayerInput.actions["Interact"].triggered)
            {
                GameObject seleccionado = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;

                if (seleccionado != null)
                {
                    for (int i = 0; i < nodosSecuencia.Length; i++)
                    {
                        if (nodosSecuencia[i].gameObject == seleccionado && nodosSecuencia[i].interactable)
                        {
                            PresionarNodo(i);
                            break;
                        }
                    }
                }
            }
        }
    }

    private void PresionarNodo(int indicePresionado)
    {
        if (indicePresionado == pasoEsperado)
        {
            // Desactiva el nodo completado
            nodosSecuencia[indicePresionado].interactable = false;
            pasoEsperado++;

            if (pasoEsperado >= nodosSecuencia.Length)
            {
                OnPuzzleExito?.Invoke();
            }
            else
            {
                // 💡 Restablece el foco al siguiente nodo activo para evitar que el EventSystem quede nulo
                EnfocarSiguienteNodoDisponible();
            }
        }
        else
        {
            OnPuzzleFallo?.Invoke();
        }
    }

    private void EnfocarSiguienteNodoDisponible()
    {
        if (EventSystem.current == null) return;

        // Busca el primer nodo en la lista que continúe activo
        for (int i = 0; i < nodosSecuencia.Length; i++)
        {
            if (nodosSecuencia[i] != null && nodosSecuencia[i].interactable)
            {
                EventSystem.current.SetSelectedGameObject(nodosSecuencia[i].gameObject);
                break;
            }
        }
    }
}