using UnityEngine;

public class ControladorTiempo:PlayerAlquimia
{
    public float rangoDeteccion = 1.5f;

    /*private void Start()
    {
        // Si hay un objeto en el holdPoint al iniciar la escena, lo vinculamos a heldItem
        if (holdPoint != null && holdPoint.childCount > 0 && heldItem == null)
        {
            heldItem = holdPoint.GetChild(0).gameObject;

            // Preparamos sus componentes para que no interfieran
            if (heldItem.TryGetComponent<Collider>(out var col)) col.enabled = false;
            if (heldItem.TryGetComponent<Rigidbody>(out var rb)) rb.isKinematic = true;
        }
    }*/

    protected override void TryPickOrDrop()
    {

        if (estaEnMinijuego) return;
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
                        Debug.Log("Objeto colocado en la mesa. Manos libres.");
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
                Debug.Log("Objeto tomado de la mesa: " + heldItem.name);
                return;
            }

            // B. Intentar tomar un ingrediente suelto del suelo
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
        if (heldItem != null)
        {
            // Llama a tu función de evolución pasando el tipo de proceso correspondiente
            ProcesarEvolucionEnMano(TipoProceso.Triturado);
            Debug.Log("¡Ingrediente transformado con éxito por el Poder del Tiempo!");
        }
        else
        {
            Debug.LogWarning("El minijuego se completó, pero 'heldItem' sigue siendo nulo para el script del jugador.");
        }
    }
}

