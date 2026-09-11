using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PiromanticoMinijuego : MonoBehaviour
{

    [Header("Referencias")]
    public RectTransform trackArea;
    public RectTransform marker;
    public RectTransform successZone;

    [Header("Gameplay")]
    public float speed = 1.5f;
    public bool randomize;
    public Vector2 zoneSizeRange = new Vector2(0.18f, 0.32f);
    public Vector2 zoneCenterClamp = new Vector2(0.15f, 0.85f);

    [Header("Input")]
    [Tooltip("Designar una accion")]
    public InputActionReference stopAction;

    [Header("Eventos")]
    public UnityEvent Succes;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
