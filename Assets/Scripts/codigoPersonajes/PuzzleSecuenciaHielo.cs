using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PuzzleSecuenciaHielo : MinijuegoBase
{
    public Image[] iconosPantalla;
    public Sprite[] spritesDirecciones; // 0: Arriba, 1: Abajo, 2: Izquierda, 3: Derecha

    private int[] secuenciaCorrecta;
    private int pasoActual = 0;
    private bool resuelto = false;

    private bool inputListo = false;
    private float tiempoEsperaInicial = 0.2f; // Tiempo de gracia al abrir la UI

    public override void InicializarPuzzle(PlayerInput pInput)
    {
        base.InicializarPuzzle(pInput);
        secuenciaCorrecta = new int[iconosPantalla.Length];
        pasoActual = 0;
        resuelto = false;
        inputListo = false;

        // Genera la secuencia aleatoria
        for (int i = 0; i < iconosPantalla.Length; i++)
        {
            secuenciaCorrecta[i] = Random.Range(0, 4);
            iconosPantalla[i].sprite = spritesDirecciones[secuenciaCorrecta[i]];
            iconosPantalla[i].color = Color.white;
        }

        // Evita leer teclas presionadas durante los primeros frames
        Invoke(nameof(HabilitarInput), tiempoEsperaInicial);
    }

    private void HabilitarInput()
    {
        inputListo = true;
    }

    private void Update()
    {
        if (!inputListo || playerInputVinculado == null) return;

        // Detecta la pulsación en el instante exacto en que se presiona la dirección
        if (playerInputVinculado.actions["Move"].triggered)
        {
            Vector2 dir = playerInputVinculado.actions["Move"].ReadValue<Vector2>();

            // Ignora movimientos ligeros o imprecisos
            if (dir.magnitude < 0.5f) return;

            int direccionIngresada = -1;

            if (dir.y > 0.5f) direccionIngresada = 0;      // Arriba
            else if (dir.y < -0.5f) direccionIngresada = 1; // Abajo
            else if (dir.x < -0.5f) direccionIngresada = 2; // Izquierda
            else if (dir.x > 0.5f) direccionIngresada = 3;  // Derecha

            if (direccionIngresada == secuenciaCorrecta[pasoActual])
            {
                iconosPantalla[pasoActual].color = Color.cyan; // Dirección correcta
                pasoActual++;

                if (pasoActual >= secuenciaCorrecta.Length)
                {
                    resuelto = true;
                    Debug.Log("¡Puzzle de Hielo Completado!");
                    OnPuzzleExito?.Invoke();
                }
            }
            else
            {
                // 💡 REINICIO EN LUGAR DE FALLO: Si se equivoca, resetea la secuencia sin cerrar la UI
                Debug.Log("Secuencia incorrecta. Reiniciando intento...");
                pasoActual = 0;

                for (int i = 0; i < iconosPantalla.Length; i++)
                {
                    iconosPantalla[i].color = Color.white;
                }
            }
        }
    }
}