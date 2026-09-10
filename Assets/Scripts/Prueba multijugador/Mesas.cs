using UnityEngine;

public class Mesas : MonoBehaviour
{

    public string mesaName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(mesaName))
        {
            Debug.Log(mesaName + " entered the trigger");
        }

    }
}
