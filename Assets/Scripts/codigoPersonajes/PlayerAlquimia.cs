using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public abstract class PlayerAlquimia:MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    public float speed = 6f;
    public float gravity = -19.62f;
    protected Vector2 moveInput;
    private Vector3 velocityY;

    [Header("Interacción y Objetos")]
    public Transform holdPoint;          // Accesible desde el inspector y subclases
    protected GameObject heldItem;       // Accesible desde las clases hijas como Piromano

    private CharacterController controller;


    [Header("UI Privada del Jugador")]
    private RectTransform miPanelUI;
    protected PlayerInput myPlayerInput;

    public GameObject[] prefabsMinijuegos;
    protected bool estaEnMinijuego = false;


    protected virtual void Awake()
    {
        controller = GetComponent<CharacterController>();
        myPlayerInput = GetComponent<PlayerInput>();
    }

    public void AsignarPanelUI(RectTransform panel)
    {
        miPanelUI = panel;
    }

    protected virtual void Start()
    {
        // Si no se asignó un panel manualmente, lo busca en el UIManager usando su índice
        if (miPanelUI == null && myPlayerInput != null && UIPuzzleManager.Instance != null)
        {
            AsignarPanelUI(UIPuzzleManager.Instance.ObtenerPanelJugador(myPlayerInput.playerIndex));
        }
    }

    // Callbacks del Input System
    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();
    public void OnInteract(InputValue value)
    {
        if (value.isPressed)
        {
            Debug.Log("¡Botón de Interacción presionado correctamente!");
            TryPickOrDrop();
        }
    }
    public void OnAbility(InputValue value) { if (value.isPressed) ExecuteAbility(); }

    protected virtual void Update()
    {
        // Si está resolviendo el puzzle, congela el movimiento pero mantiene la gravedad
        if (!estaEnMinijuego)
        {
            Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y);
            if (movement.sqrMagnitude > 0.01f)
            {
                controller.Move(movement * speed * Time.deltaTime);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), Time.deltaTime * 12f);
            }
        }

        // Aplicación de gravedad
        if (controller.isGrounded && velocityY.y < 0) velocityY.y = -2f;
        velocityY.y += gravity * Time.deltaTime;
        controller.Move(velocityY * Time.deltaTime);
    }

    protected virtual void ExecuteAbility()
    {
        if (estaEnMinijuego) return;

        // Si existen puzzles y el panel UI privado está configurado, lanza el minijuego
        if (prefabsMinijuegos != null && prefabsMinijuegos.Length > 0 && miPanelUI != null)
        {
            LanzarMinijuego();
        }
        else
        {
            // Si no hay minijuegos, ejecuta directamente la habilidad de la subclase
            ExecuteAbilityLogic();
        }
    }

    private void LanzarMinijuego()
    {
        estaEnMinijuego = true;

        // Elige un puzzle al azar y lo coloca en su cuadrante
        int index = Random.Range(0, prefabsMinijuegos.Length);
        GameObject puzzleObj = Instantiate(prefabsMinijuegos[index], miPanelUI);

        MinijuegoBase minijuego = puzzleObj.GetComponent<MinijuegoBase>();

        if (minijuego != null)
        {
            minijuego.OnPuzzleExito += () =>
            {
                estaEnMinijuego = false;
                Destroy(puzzleObj);
                ExecuteAbilityLogic(); // ¡Dispara el efecto real del rol!
            };

            minijuego.OnPuzzleFallo += () =>
            {
                estaEnMinijuego = false;
                Destroy(puzzleObj);
            };

            minijuego.InicializarPuzzle(myPlayerInput);
        }
        else
        {
            Debug.LogWarning("El minijuego instanciado carece del componente MinijuegoBase.");
            estaEnMinijuego = false;
            Destroy(puzzleObj);
        }
    }

    protected abstract void TryPickOrDrop();

    // Método abstracto que escribirán Triturador, Piromano, Criogenico y Telecinetico
    protected abstract void ExecuteAbilityLogic();

}
