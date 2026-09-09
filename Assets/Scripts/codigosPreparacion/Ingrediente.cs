using UnityEngine;

public enum TipoProceso { Ninguno, Triturado, Calentado, Congelado }

public class Ingrediente : MonoBehaviour
{
    public string nombreIngrediente = "Gema Cruda";
    public TipoProceso estadoActual = TipoProceso.Ninguno;

    [Header("Evoluciones (Siguiente Prefab)")]
    public GameObject prefabAlTriturar; // Ej: Polvo de Gema
    public GameObject prefabAlCalentar; // Ej: Esencia Ferviente
    public GameObject prefabAlCongelar; // Ej: Cristal Helmado

    public GameObject AplicarProceso(TipoProceso nuevoProceso)
    {
        GameObject prefabSiguiente = null;

        switch (nuevoProceso)
        {
            case TipoProceso.Triturado: prefabSiguiente = prefabAlTriturar; break;
            case TipoProceso.Calentado: prefabSiguiente = prefabAlCalentar; break;
            case TipoProceso.Congelado: prefabSiguiente = prefabAlCongelar; break;
        }

        // Si existe una transformación válida para este estado actual
        if (prefabSiguiente != null)
        {
            GameObject nuevoObjeto = Instantiate(prefabSiguiente, transform.position, transform.rotation);
            Destroy(gameObject); // Elimina el ingrediente base
            return nuevoObjeto;
        }

        Debug.Log($"El ingrediente {nombreIngrediente} no reacciona al proceso {nuevoProceso}");
        return gameObject;
    }
}