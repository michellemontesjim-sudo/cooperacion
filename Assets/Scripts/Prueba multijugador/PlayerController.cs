using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector2 moveInput;

    //movement fields
    private Rigidbody rb;
    [SerializeField] private float speed = 10f;


    private void Awake()
    {

        rb = GetComponent<Rigidbody>();

    }


    private void FixedUpdate()
    {

        
        Vector3 targetVelocity = new Vector3(moveInput.x * speed, rb.linearVelocity.y, moveInput.y * speed);

        
        rb.linearVelocity = targetVelocity;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnMove(InputAction.CallbackContext context)
    {

        moveInput = context.ReadValue<Vector2>(); ;

    }
}
