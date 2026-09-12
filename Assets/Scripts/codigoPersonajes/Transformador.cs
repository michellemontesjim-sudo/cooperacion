using System.Collections.Generic;
using UnityEngine;

public class Transformador : PlayerAlquimia
{
    public float rangoDeteccion = 1.5f;

    [Header("Prefab de Receta Fallida")]
    public GameObject prefabBasura;

    protected override void TryPickOrDrop()
    {
        if (estaEnMinijuego) return;

        if (holdPoint == null)
        {
            Debug.LogError("[Transformador] ¡EL HOLD POINT NO ESTÁ ASIGNADO EN EL INSPECTOR!");
            return;
        }

        Collider[] hits = Physics.OverlapSphere(holdPoint.position, rangoDeteccion);
        Debug.Log($"[Transformador] Objetos detectados en el área de interacción: {hits.Length}");


        if (heldItem != null)
        {
            foreach (Collider hit in hits)
            {
                if (hit.TryGetComponent<MesaContenedora>(out var mesa) && !mesa.EstaOcupada)
                {
                    if (mesa.ColocarObjeto(heldItem)) heldItem = null;
                    return;
                }
            }
            SoltarAlSuelo();
            return;
        }


        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent<MesaContenedora>(out var mesa) && mesa.EstaOcupada)
            {
                heldItem = mesa.TomarObjeto();
                AgarrarEnMano(heldItem);
                Debug.Log("[Transformador] Objeto tomado de la MESA exitosamente.");
                return;
            }

            if (hit.TryGetComponent<GeneradorIngredientes>(out var generador))
            {
                GameObject nuevo = generador.EntregarIngrediente(holdPoint);
                if (nuevo != null)
                {
                    heldItem = nuevo;
                    AgarrarEnMano(heldItem);
                    Debug.Log("[Transformador] Objeto tomado de la CAJA exitosamente.");
                }
                return;
            }

            if (hit.TryGetComponent<Ingrediente>(out var ingrediente))
            {
                heldItem = ingrediente.gameObject;
                AgarrarEnMano(heldItem);
                Debug.Log("[Transformador] Objeto tomado del SUELO exitosamente.");
                return;
            }
        }

        Debug.LogWarning("[Transformador] Se presionó Interactuar, pero no se encontró ningún ingrediente, mesa ni caja válida.");
    }

    private void AgarrarEnMano(GameObject obj)
    {
        if (obj == null) return;

        obj.transform.SetParent(holdPoint);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        if (obj.TryGetComponent<Collider>(out var col))
            col.enabled = false;

        if (obj.TryGetComponent<Rigidbody>(out var rb))
            rb.isKinematic = true;
    }

    private void SoltarAlSuelo()
    {
        if (heldItem == null) return;

        heldItem.transform.SetParent(null);
        heldItem.transform.position = holdPoint.position;

        if (heldItem.TryGetComponent<Collider>(out var col))
            col.enabled = true;

        if (heldItem.TryGetComponent<Rigidbody>(out var rb))
            rb.isKinematic = false;

        heldItem = null;
    }

    protected override void ExecuteAbilityLogic()
    {
        if (heldItem == null || !heldItem.TryGetComponent<Ingrediente>(out var ingrediente))
        {
            Debug.LogWarning("[Transformador] No tienes un ingrediente válido en la mano para transmutar.");
            return;
        }

        CalderoFinal caldero = FindObjectOfType<CalderoFinal>();
        if (caldero == null)
        {
            Debug.LogError("[Transformador] No se encontró el objeto 'CalderoFinal' en la escena.");
            return;
        }

        OrdenReceta ordenActual = caldero.ObtenerOrdenActual();
        if (ordenActual == null)
        {
            Debug.LogWarning("[Transformador] El caldero no tiene ninguna orden activa en este momento.");
            return;
        }

        GameObject objetoViejo = heldItem;

        // si la secuencia del ingrediente coincide con la receta pedida
        if (ingrediente.ValidarSecuencia(ordenActual.secuenciaRequerida))
        {
            GameObject nuevoResultado = Instantiate(ordenActual.prefabResultadoFinal, holdPoint.position, holdPoint.rotation);
            if (nuevoResultado.TryGetComponent<Ingrediente>(out var resultadoIngrediente))
            {
                resultadoIngrediente.historialProcesos =
                    new List<TipoProceso>(ingrediente.historialProcesos);

                resultadoIngrediente.nombreIngrediente = ingrediente.nombreIngrediente;
            }
            Destroy(objetoViejo);
            ActualizarObjetoEnMano(nuevoResultado);
            Debug.Log("¡Transmutación Exitosa! El ingrediente es correcto.");
        }
        else
        {
            if (prefabBasura != null)
            {
                GameObject basura = Instantiate(prefabBasura, holdPoint.position, holdPoint.rotation);
                Destroy(objetoViejo);
                ActualizarObjetoEnMano(basura);
                Debug.LogWarning("Secuencia incorrecta. La transmutación generó basura.");
            }
            else
            {
                Debug.LogError("[Transformador] Falta asignar el 'Prefab Basura' en el Inspector.");
            }
        }
    }
}