using UnityEngine;

public class Criogenico : PlayerAlquimia
{
    public float rangoDeteccion = 1.5f;

    protected override void TryPickOrDrop()
    {
        if (estaEnMinijuego) return;
        if (holdPoint == null) return;

        Collider[] hits = Physics.OverlapSphere(holdPoint.position, rangoDeteccion);


        // objeto en mano

        if (heldItem != null)
        {
            // busca si hay mesa
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

            // si no hay mesa se suelta en el suelo
            SoltarAlSuelo();
            return;
        }


        // en manos libres

        foreach (Collider hit in hits)
        {
            // toma objeto de mesa ocupada
            if (hit.TryGetComponent<MesaContenedora>(out var mesa) && mesa.EstaOcupada)
            {
                heldItem = mesa.TomarObjeto();
                AgarrarEnMano(heldItem);
                return;
            }

            // toma objeto del suelo
            if (hit.TryGetComponent<Ingrediente>(out var ingrediente))
            {
                heldItem = ingrediente.gameObject;
                AgarrarEnMano(heldItem);
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
        }
    }

    private void AgarrarEnMano(GameObject obj)
    {
        obj.transform.SetParent(holdPoint);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        if (obj.TryGetComponent<Collider>(out var col))
            col.enabled = false; //se desactiva para no colisionar con el jugador

        if (obj.TryGetComponent<Rigidbody>(out var rb))
            rb.isKinematic = true;
    }

    private void SoltarAlSuelo()
    {
        heldItem.transform.SetParent(null);
        heldItem.transform.position = holdPoint.position;

        if (heldItem.TryGetComponent<Collider>(out var col))
            col.enabled = true; // se reactiva para detectar interacciones futuras

        if (heldItem.TryGetComponent<Rigidbody>(out var rb))
            rb.isKinematic = false; // reactiva f�sicas para caer al suelo

        heldItem = null;
    }

    protected override void ExecuteAbilityLogic()
    {
        ProcesarEvolucionEnMano(TipoProceso.Congelado);
    }
}
