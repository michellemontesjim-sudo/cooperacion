using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OrdenReceta
{
    public string nombreReceta;
    public List<TipoProceso> secuenciaRequerida;
    public GameObject prefabResultadoFinal;
    public Sprite iconoResultado;
    public int puntosRecompensa;
}