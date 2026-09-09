using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class PuzzleSecuenciaHielo:MinijuegoBase
{
    public Image[] iconosPantalla;
    public Sprite[] spritesDirecciones; // 0: Arriba, 1: Abajo, 2: Izquierda, 3: Derecha

    private int[] secuenciaCorrecta;
    private int pasoActual = 0;

    public override void InicializarPuzzle(PlayerInput pInput)
    {
        base.InicializarPuzzle(pInput);
        secuenciaCorrecta = new int[iconosPantalla.Length];

        // Genera la secuencia aleatoria
        for (int i = 0; i < iconosPantalla.Length; i++)
        {
            secuenciaCorrecta[i] = Random.Range(0, 4);
            iconosPantalla[i].sprite = spritesDirecciones[secuenciaCorrecta[i]];
            iconosPantalla[i].color = Color.white;
        }
    }

    private void Update()
    {
        if (playerInputVinculado == null) return;

        // Lee la entrada de movimiento del jugador
        if (playerInputVinculado.actions["Move"].triggered)
        {
            Vector2 dir = playerInputVinculado.actions["Move"].ReadValue<Vector2>();
            int direccionIngresada = -1;

            if (dir.y > 0.5f) direccionIngresada = 0;      // Arriba
            else if (dir.y < -0.5f) direccionIngresada = 1; // Abajo
            else if (dir.x < -0.5f) direccionIngresada = 2; // Izquierda
            else if (dir.x > 0.5f) direccionIngresada = 3;  // Derecha

            if (direccionIngresada != -1)
            {
                if (direccionIngresada == secuenciaCorrecta[pasoActual])
                {
                    iconosPantalla[pasoActual].color = Color.cyan; // Marca avance
                    pasoActual++;

                    if (pasoActual >= secuenciaCorrecta.Length)
                    {
                        OnPuzzleExito?.Invoke();
                    }
                }
                else
                {
                    OnPuzzleFallo?.Invoke(); // Error en la secuencia
                }
            }
        }
    }
}
