using UnityEngine;

[System.Serializable]
public struct OrdenReceta
{
    public string nombreReceta;             // Ej: "Cristal Helmado"
    public TipoProceso estadoRequerido;     // Ej: TipoProceso.Congelado
    public Sprite iconoResultado;           // Sprite para la interfaz gráfica
    public int puntosRecompensa;            // Ej: 100
}