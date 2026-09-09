using UnityEngine;
using UnityEngine.UI;

public class PuzzleTableroX : MinijuegoBase
{
    public Button[] botonesTablero;
    public Sprite spriteX;
    public Sprite spriteO;

    private bool[] estadosX;

    public virtual void InicializarPuzzle()
    {
        estadosX = new bool[botonesTablero.Length];

        // Generar estado aleatorio inicial
        for (int i = 0; i < botonesTablero.Length; i++)
        {
            int index = i;
            estadosX[i] = Random.value > 0.5f;
            ActualizarGrafico(index);
            botonesTablero[i].onClick.AddListener(() => AlternarCasilla(index));
        }
    }

    private void AlternarCasilla(int index)
    {
        estadosX[index] = !estadosX[index];
        ActualizarGrafico(index);
        ValidarVictoria();
    }

    private void ActualizarGrafico(int index)
    {
        botonesTablero[index].GetComponent<Image>().sprite = estadosX[index] ? spriteX : spriteO;
    }

    private void ValidarVictoria()
    {
        foreach (bool esX in estadosX)
        {
            if (!esX) return; // Todavía hay casillas que no son X
        }

        OnPuzzleExito?.Invoke(); // Se resuelve el puzzle con éxito
    }
}