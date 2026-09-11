using UnityEngine;

public class GeneradorIngredientes : MonoBehaviour
{
    [Header("Prefab del Ingrediente a entregar")]
    public GameObject prefabIngrediente;


    public GameObject EntregarIngrediente(Transform holdPoint)
    {
        if (prefabIngrediente == null)
        {
            Debug.LogWarning($"El generador {gameObject.name} no tiene asignado un Prefab de Ingrediente.");
            return null;
        }

        // Instancia el objeto como hijo de la mano del jugador
        GameObject nuevoIngrediente = Instantiate(prefabIngrediente, holdPoint.position, holdPoint.rotation, holdPoint);
        nuevoIngrediente.transform.localPosition = Vector3.zero;
        nuevoIngrediente.transform.localRotation = Quaternion.identity;

        // Desactiva f�sicas mientras est� sostenido
        if (nuevoIngrediente.TryGetComponent<Rigidbody>(out var rb)) rb.isKinematic = true;
        if (nuevoIngrediente.TryGetComponent<Collider>(out var col)) col.enabled = false;

        return nuevoIngrediente;
    }
}