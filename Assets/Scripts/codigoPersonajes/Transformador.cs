using UnityEngine;

public class Transformador:PlayerAlquimia
{
    public float rangoDeteccion = 1.5f;

    [Header("Prefab de Receta Fallida")]
    public GameObject prefabBasura;
    protected override void TryPickOrDrop()
    {
        if (holdPoint == null) return;

        Collider[] hits = Physics.OverlapSphere(holdPoint.position, rangoDeteccion);

        // -------------------------------------------------------------
        // CASO 1: LLEVAS UN OBJETO EN LA MANO
        // -------------------------------------------------------------
        if (heldItem != null)
        {
            // A. Buscar primero si hay una mesa libre enfrente
            foreach (Collider hit in hits)
            {
                if (hit.TryGetComponent<MesaContenedora>(out var mesa) && !mesa.EstaOcupada)
                {
                    if (mesa.ColocarObjeto(heldItem))
                    {
                        heldItem = null;
                    }
                    return;
                }
            }

            // B. Si no hay mesa libre enfrente, soltarlo directamente al suelo
            SoltarAlSuelo();
            return;
        }

        // -------------------------------------------------------------
        // CASO 2: TIENES LAS MANOS LIBRES
        // -------------------------------------------------------------
        foreach (Collider hit in hits)
        {
            // A. Intentar tomar de una mesa ocupada
            if (hit.TryGetComponent<MesaContenedora>(out var mesa) && mesa.EstaOcupada)
            {
                heldItem = mesa.TomarObjeto();
                AgarrarEnMano(heldItem);
                return;
            }

            // B. ¡AQUÍ VA LA CAJA! Genera e instala un ingrediente nuevo en su mano
            if (hit.TryGetComponent<GeneradorIngredientes>(out var generador))
            {
                heldItem = generador.EntregarIngrediente(holdPoint);
                return; // Interacción completada con éxito
            }

            // B. Intentar tomar un ingrediente suelto del suelo
            if (hit.TryGetComponent<Ingrediente>(out var ingrediente))
            {
                heldItem = ingrediente.gameObject;
                AgarrarEnMano(heldItem);
                return;
            }
        }
    }

    private void AgarrarEnMano(GameObject obj)
    {
        obj.transform.SetParent(holdPoint);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        if (obj.TryGetComponent<Collider>(out var col))
            col.enabled = false; // Se desactiva para no colisionar con el jugador mientras camina

        if (obj.TryGetComponent<Rigidbody>(out var rb))
            rb.isKinematic = true;
    }

    private void SoltarAlSuelo()
    {
        heldItem.transform.SetParent(null);
        heldItem.transform.position = holdPoint.position;

        if (heldItem.TryGetComponent<Collider>(out var col))
            col.enabled = true; // Se reactiva para detectar interacciones futuras

        if (heldItem.TryGetComponent<Rigidbody>(out var rb))
            rb.isKinematic = false; // Reactiva físicas para caer al suelo

        heldItem = null;
    }

    protected override void ExecuteAbilityLogic()
    {
        if (heldItem == null || !heldItem.TryGetComponent<Ingrediente>(out var ingrediente)) return;

        CalderoFinal caldero = FindObjectOfType<CalderoFinal>();
        if (caldero == null) return;

        OrdenReceta ordenActual = caldero.ObtenerOrdenActual();

        // Evalúa si el historial del ingrediente coincide con la secuencia de la orden
        if (ingrediente.ValidarSecuencia(ordenActual.secuenciaRequerida))
        {
            GameObject nuevoResultado = Instantiate(ordenActual.prefabResultadoFinal, holdPoint.position, holdPoint.rotation);
            Destroy(heldItem);
            ActualizarObjetoEnMano(nuevoResultado);
            Debug.Log("¡Transmutación Exitosa! El ingrediente es correcto.");
        }
        else
        {
            GameObject basura = Instantiate(prefabBasura, holdPoint.position, holdPoint.rotation);
            Destroy(heldItem);
            ActualizarObjetoEnMano(basura);
            Debug.LogWarning("Secuencia incorrecta. La transmutación falló.");
        }
    }
}
