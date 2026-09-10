using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OrdenReceta
{
    public string nombreReceta;
    public List<TipoProceso> secuenciaRequerida; // Ej: [Triturado, Calentado, Congelado]
    public GameObject prefabResultadoFinal;      // El objeto transformado que se entregará
    public Sprite iconoResultado;
    public int puntosRecompensa;
}