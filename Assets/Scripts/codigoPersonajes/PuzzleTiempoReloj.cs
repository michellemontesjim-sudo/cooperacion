using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzleTiempoReloj : MinijuegoBase
{
    [Header("Referencias UI del Reloj")]
    public RectTransform agujaReloj;

    [Header("Ajustes del Puzzle")]
    public float velocidadGiro = 250f;
    public float anguloObjetivoMin = 45f;
    public float anguloObjetivoMax = 80f;

    private bool resuelto = false;

    public override void InicializarPuzzle(PlayerInput pInput)
    {
        base.InicializarPuzzle(pInput);
        resuelto = false;
    }

    private void Update()
    {
        if (resuelto || agujaReloj == null) return;

        agujaReloj.Rotate(0f, 0f, -velocidadGiro * Time.deltaTime);

        // detecta si Interact se presionó en el frame
        if (playerInputVinculado != null && playerInputVinculado.actions["Interact"].triggered)
        {
            
            float anguloZ = (360f - agujaReloj.localEulerAngles.z) % 360f;

            
            if (anguloZ >= anguloObjetivoMin && anguloZ <= anguloObjetivoMax)
            {
                Debug.Log("¡Puzzle Completado!");
                resuelto = true;
                OnPuzzleExito?.Invoke(); // llama a ExecuteAbilityLogic()
            }
            else
            {
                Debug.Log("Puzzle Fallado");
                resuelto = true;
                OnPuzzleFallo?.Invoke();
            }
        }
    }
}