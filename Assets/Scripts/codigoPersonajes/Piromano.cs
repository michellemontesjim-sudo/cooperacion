using UnityEngine;

public class Piromano : PlayerAlquimia
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
                return;
            }

            
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
            ProcesarEvolucionEnMano(TipoProceso.Calentado);
            Debug.Log("¡Ingrediente transformado por el Pirómano!");
        }
        else
        {
            Debug.LogWarning("El minijuego finalizó, pero no se encontró ingrediente en la mano.");
        }
    }
}
