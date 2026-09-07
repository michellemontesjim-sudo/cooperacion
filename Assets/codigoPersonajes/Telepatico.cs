using UnityEngine;

public class Telepatico: PlayerAlquimia
{
    public float rangoDeteccion = 1.5f;
    public float alcanceTelequinesis = 6f;
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
        if (heldItem == null)
        {
            if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out RaycastHit hit, alcanceTelequinesis))
            {
                if (hit.collider.TryGetComponent<Ingrediente>(out var ing))
                {
                    heldItem = ing.gameObject;
                    heldItem.transform.SetParent(holdPoint);
                    heldItem.transform.localPosition = Vector3.zero;
                    if (heldItem.TryGetComponent<Rigidbody>(out var rb)) rb.isKinematic = true;
                    Debug.Log("¡Objeto atraído con Telequinesis!");
                }
            }
        }
    }
}
