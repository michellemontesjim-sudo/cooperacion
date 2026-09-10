using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PuzzleCalorPiromano : MinijuegoBase
{
    public Slider barraCalor;
    public float enfriamientoVelocidad = 0.4f;
    public float incrementoPorClic = 0.15f;
    public float zonaCalienteMin = 0.65f;
    public float zonaCalienteMax = 0.85f;
    public float tiempoRequerido = 2f;

    private float tiempoAcumulado = 0f;
    private bool resuelto = false; // 👈 1. Nueva bandera

    public override void InicializarPuzzle(PlayerInput pInput)
    {
        base.InicializarPuzzle(pInput);
        resuelto = false;
        tiempoAcumulado = 0f;
        if (barraCalor != null) barraCalor.value = 0f;
    }

    private void Update()
    {
        if (resuelto || barraCalor == null) return; // 👈 2. Evita ejecuciones extra al ganar

        // Disminuye la temperatura gradualmente
        barraCalor.value -= enfriamientoVelocidad * Time.deltaTime;

        // Aumenta la temperatura al presionar repetidamente
        if (playerInputVinculado != null && playerInputVinculado.actions["Interact"].triggered)
        {
            barraCalor.value += incrementoPorClic;
        }

        // Evalúa si la barra está en el punto correcto
        if (barraCalor.value >= zonaCalienteMin && barraCalor.value <= zonaCalienteMax)
        {
            tiempoAcumulado += Time.deltaTime;
            if (tiempoAcumulado >= tiempoRequerido)
            {
                resuelto = true; // 👈 3. Bloquea llamadas repetidas
                Debug.Log("¡Puzzle de Calor Completado!");
                OnPuzzleExito?.Invoke();
            }
        }
        else
        {
            tiempoAcumulado = Mathf.Max(0f, tiempoAcumulado - Time.deltaTime);
        }
    }
}