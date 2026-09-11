using UnityEngine;

public class ControladorTiempo:PlayerAlquimia
{
    public float rangoDeteccion = 1.5f;

    protected override void TryPickOrDrop()
    {

        if (estaEnMinijuego) return;
        if (holdPoint == null) return;

        Collider[] hits = Physics.OverlapSphere(holdPoint.position, rangoDeteccion);

        
        if (heldItem != null)
        {
            
            foreach (Collider hit in hits)
            {
                if (hit.TryGetComponent<MesaContenedora>(out var mesa) && !mesa.EstaOcupada)
                {
                    if (mesa.ColocarObjeto(heldItem))
                    {
                        Debug.Log("Objeto colocado en la mesa. Manos libres.");
                        heldItem = null;
                    }
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
                Debug.Log("Objeto tomado de la mesa: " + heldItem.name);
                return;
            }

            
            if (hit.TryGetComponent<Ingrediente>(out var ingrediente))
            {
                heldItem = ingrediente.gameObject;
                AgarrarEnMano(heldItem);
                Debug.Log("¡Ingrediente tomado del suelo exitosamente!: " + heldItem.name);
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
            col.enabled = false;

        if (obj.TryGetComponent<Rigidbody>(out var rb))
            rb.isKinematic = true;
    }

    private void SoltarAlSuelo()
    {
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
        if (heldItem != null)
        {
            
            ProcesarEvolucionEnMano(TipoProceso.Triturado);
            Debug.Log("¡Ingrediente transformado con éxito por el Poder del Tiempo!");
        }
        else
        {
            Debug.LogWarning("El minijuego se completó, pero 'heldItem' sigue siendo nulo para el script del jugador.");
        }
    }
}

