using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector2 moveInput;

    private PlayerManager playerManager;

    //movement fields
    private Rigidbody rb;
    [SerializeField] private float speed = 10f;


    private void Awake()
    {

        playerManager = FindAnyObjectByType<PlayerManager>();
        rb = GetComponent<Rigidbody>();

    }


    private void FixedUpdate()
    {


        Vector3 targetVelocity = new Vector3(moveInput.x * speed, rb.linearVelocity.y, moveInput.y * speed);


        rb.linearVelocity = targetVelocity;

        RotatePlayerToMovementDirection(targetVelocity);

    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void RotatePlayerToMovementDirection(Vector3 targetVelocity)
    {
        Vector3 horizontalVelocity = new Vector3(targetVelocity.x, 0f, targetVelocity.z);

        if (horizontalVelocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(horizontalVelocity);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * 10f));
        }
    }



    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Piromantico"))
        {
            Debug.Log("Piromantico entered the trigger");
        }
        else if (other.CompareTag("Criomantico"))
        {
            Debug.Log("Criomantico entered the trigger");
        }
        else if (other.CompareTag("Cronomantico"))
        {
            Debug.Log("Cronomantico entered the trigger");
        }
        else if (other.CompareTag("Transmutador"))
        {
            Debug.Log("Transmutador entered the trigger");
        }

    }

    public void OnMove(InputAction.CallbackContext context)
    {

        moveInput = context.ReadValue<Vector2>(); ;

    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Interact button pressed");
        }
    }

    public void OnAbility(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Ability button pressed");
        }
    }
}
