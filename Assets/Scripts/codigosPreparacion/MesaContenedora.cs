using UnityEngine;

public class MesaContenedora:MonoBehaviour
{
    public Transform puntoColocacion;
    private GameObject objetoDepositado;

    public bool EstaOcupada => objetoDepositado != null;

    public bool ColocarObjeto(GameObject objeto)
    {
        if (EstaOcupada) return false;

        objetoDepositado = objeto;
        objetoDepositado.transform.SetParent(puntoColocacion);
        objetoDepositado.transform.localPosition = Vector3.zero;
        objetoDepositado.transform.localRotation = Quaternion.identity;

        if (objetoDepositado.TryGetComponent<Rigidbody>(out var rb))
            rb.isKinematic = true;

        return true;
    }

    public GameObject TomarObjeto()
    {
        if (!EstaOcupada) return null;

        GameObject temp = objetoDepositado;
        objetoDepositado = null;
        return temp;
    }
}
