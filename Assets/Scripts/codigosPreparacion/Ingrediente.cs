using System.Collections.Generic;
using UnityEngine;

public enum TipoProceso { Ninguno, Triturado, Calentado, Congelado }

public class Ingrediente : MonoBehaviour
{
    public string nombreIngrediente = "Gema Entera";

    [Header("Historial de Procesos")]
    public List<TipoProceso> historialProcesos = new List<TipoProceso>();

    [Header("Prefabs de Siguiente Estado")]
    public GameObject prefabAlTriturar;  // Ej: Prefab de "Pedazos de Gema"
    public GameObject prefabAlCalentar;  // Ej: Prefab de "Gema Fundida"
    public GameObject prefabAlCongelar;  // Ej: Prefab de "Gema Congelada"

    [Header("Efectos de Audio 3D")]
    public AudioClip sonidoEvolucion;
    public AudioClip sonidoSuelto;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1.0f; 
        audioSource.playOnAwake = true;
        audioSource.loop = true;
    }

    private void Start()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void RegistrarProceso(TipoProceso nuevoProceso)
    {
        historialProcesos.Add(nuevoProceso);
    }

    
    public bool ValidarSecuencia(List<TipoProceso> secuenciaEsperada)
    {
        if (secuenciaEsperada == null) return false;
        if (historialProcesos.Count != secuenciaEsperada.Count) return false;

        for (int i = 0; i < historialProcesos.Count; i++)
        {
            if (historialProcesos[i] != secuenciaEsperada[i]) return false;
        }
        return true;
    }

    /// <summary>
    /// Transforma el modelo 3D preservando y actualizando el historial acumulado.
    /// </summary>
    public GameObject Evolucionar(TipoProceso proceso)
    {
        GameObject siguientePrefab = null;

        switch (proceso)
        {
            case TipoProceso.Triturado: siguientePrefab = prefabAlTriturar; break;
            case TipoProceso.Calentado: siguientePrefab = prefabAlCalentar; break;
            case TipoProceso.Congelado: siguientePrefab = prefabAlCongelar; break;
        }

        // Si no hay un prefab asignado para este proceso, mantiene el objeto actual y solo registra el paso
        if (siguientePrefab == null)
        {
            RegistrarProceso(proceso);
            return gameObject;
        }

        // 1. Instancia el nuevo modelo 3D en la misma posición
        GameObject nuevoObjeto = Instantiate(siguientePrefab, transform.position, transform.rotation);

        // 2. Copia el historial previo al nuevo objeto y le agrega el proceso actual
        if (nuevoObjeto.TryGetComponent<Ingrediente>(out var nuevoIngredienteScript))
        {
            nuevoIngredienteScript.historialProcesos = new List<TipoProceso>(this.historialProcesos);
            nuevoIngredienteScript.RegistrarProceso(proceso);
        }

        // 3. Destruye la versión anterior
        Destroy(gameObject);

        return nuevoObjeto;
    }
}