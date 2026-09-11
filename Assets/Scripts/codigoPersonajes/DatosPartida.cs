using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class DatosPartida:MonoBehaviour
{
    public static DatosPartida Instance;
    public List<InputDevice> DispositivosGuardados = new List<InputDevice>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
