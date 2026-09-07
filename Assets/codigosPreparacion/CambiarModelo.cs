using UnityEngine;

public class CambiarModelo : MonoBehaviour
{
    [Header("Asigna el Prefab de la mesa desde el Inspector")]
    [SerializeField] private GameObject prefabMesa;

    public void ReemplazarPorMesa()
    {
        // Guardar la transformación del objeto actual
        Vector3 posicion = transform.position;
        Quaternion rotacion = transform.rotation;
        Transform padre = transform.parent;

        // Instanciar la mesa en la misma ubicación
        Instantiate(prefabMesa, posicion, rotacion, padre);

        // Eliminar el objeto actual (el cubo)
        Destroy(gameObject);
    }
}