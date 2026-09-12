using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PuzzleSecuenciaHielo : MinijuegoBase
{
    public Image[] iconosPantalla;
    public Sprite[] spritesDirecciones;

    private int[] secuenciaCorrecta;
    private int pasoActual = 0;
    private bool resuelto = false;

    private bool inputListo = false;
    private float tiempoEsperaInicial = 0.2f;

    public override void InicializarPuzzle(PlayerInput pInput)
    {
        pInput.SwitchCurrentActionMap("MiniGames");
        base.InicializarPuzzle(pInput);
        secuenciaCorrecta = new int[iconosPantalla.Length];
        pasoActual = 0;
        resuelto = false;
        inputListo = false;


        for (int i = 0; i < iconosPantalla.Length; i++)
        {
            secuenciaCorrecta[i] = Random.Range(0, 4);
            iconosPantalla[i].sprite = spritesDirecciones[secuenciaCorrecta[i]];
            iconosPantalla[i].color = Color.white;
        }


        Invoke(nameof(HabilitarInput), tiempoEsperaInicial);
    }

    private void HabilitarInput()
    {
        inputListo = true;
    }

    private void Update()
    {
        if (!inputListo || playerInputVinculado == null) return;

        //detecta pulsación en la tecla
        if (playerInputVinculado.actions["Flechas"].triggered)
        {
            Vector2 dir = playerInputVinculado.actions["Flechas"].ReadValue<Vector2>();

            // ignora movimientos imprecisos
            if (dir.magnitude < 0.5f) return;

            int direccionIngresada = -1;

            if (dir.y > 0.5f) direccionIngresada = 0;
            else if (dir.y < -0.5f) direccionIngresada = 1;
            else if (dir.x < -0.5f) direccionIngresada = 2;
            else if (dir.x > 0.5f) direccionIngresada = 3;

            if (direccionIngresada == secuenciaCorrecta[pasoActual])
            {
                iconosPantalla[pasoActual].color = Color.cyan;
                pasoActual++;

                if (pasoActual >= secuenciaCorrecta.Length)
                {
                    resuelto = true;
                    playerInputVinculado.SwitchCurrentActionMap("GamePlay");
                    Debug.Log("¡Puzzle de Hielo Completado!");
                    OnPuzzleExito?.Invoke();
                }
            }
            else
            {

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