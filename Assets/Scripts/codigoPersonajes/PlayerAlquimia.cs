using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;


public abstract class PlayerAlquimia:MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    public float speed = 6f;
    public float gravity = -19.62f;
    protected Vector2 moveInput;
    private Vector3 velocityY;

    [Header("Interacción y Objetos")]
    public Transform holdPoint;
    protected GameObject heldItem;

    private CharacterController controller;


    [Header("UI Privada del Jugador")]
    private RectTransform miPanelUI;
    protected PlayerInput myPlayerInput;

    [Header("UI Privada y Minijuego del Rol")]
    public GameObject prefabMinijuegoUnico;
    protected bool estaEnMinijuego = false;

    public int indiceJugadorCustom = -1;
    public int playerIndex => indiceJugadorCustom != -1 ? indiceJugadorCustom : (myPlayerInput != null ? myPlayerInput.playerIndex : -1);
    public MesaJugador MesaActual { get; set; }
    protected virtual void Awake()
    {
        //busca los componentes en el objeto contenedor
        controller = GetComponentInParent<CharacterController>();
        myPlayerInput = GetComponentInParent<PlayerInput>();

        if (controller == null)
            Debug.LogError($"[{gameObject.name}] No se encontró CharacterController en el objeto padre.");
    }

    
    protected virtual void Start()
    {
        if (myPlayerInput != null)
        {
            // Activa el componente del rol asignado a este índice
            int index = myPlayerInput.playerIndex;
            ConfigurarCuadranteUI(index);
        }

       
    }

    public void AsignarPanelUI(RectTransform panel)
    {
        miPanelUI = panel;
    }

    public void ConfigurarCuadranteUI(int playerIndex)
    {
        this.indiceJugadorCustom = playerIndex; //guarda indice real

        if (UIPuzzleManager.Instance != null)
        {
            miPanelUI = UIPuzzleManager.Instance.ObtenerPanelJugador(playerIndex);
        }
    }

    

    // Callbacks del Input System
    public void OnMove(InputValue value)
    {
        
        if (estaEnMinijuego)
        {
            moveInput = Vector2.zero;
            return;
        }

        Vector2 inputVector = value.Get<Vector2>();
        moveInput = inputVector;
    }
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
        // está resolviendo el puzzle congela el movimiento
        if (!estaEnMinijuego)
        {
            Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y);
            if (movement.sqrMagnitude > 0.01f)
            {
                controller.Move(movement * speed * Time.deltaTime);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), Time.deltaTime * 12f);
            }
        }

        
        if (controller.isGrounded && velocityY.y < 0) velocityY.y = -2f;
        velocityY.y += gravity * Time.deltaTime;
        controller.Move(velocityY * Time.deltaTime);
    }

    protected virtual void ExecuteAbility()
    {
        if (estaEnMinijuego) return;

        // verificación mesa asignada
        if (MesaActual == null || !MesaActual.PuedoProcesar())
        {
            Debug.LogWarning($"[{gameObject.name}] Solo puedes usar tu habilidad/minijuego frente a tu propia mesa.");
            return;
        }

        // lanza minijuego si existe puzzle
        if (prefabMinijuegoUnico != null && miPanelUI != null)
        {
            LanzarMinijuego();
        }
        else
        {
            
            ExecuteAbilityLogic();
        }
    }

    private void LanzarMinijuego()
    {
        // no se abre el puzzle sin nada en mano
        if (heldItem == null)
        {
            Debug.LogWarning("[PlayerAlquimia] No puedes usar la habilidad: ¡Las manos están vacías!");
            return;
        }
        // miPanelUI no se asignó en el Lobby lo reconecta con la escena de Juego actual
        if (miPanelUI == null && myPlayerInput != null)
        {
            ConfigurarCuadranteUI(myPlayerInput.playerIndex);
        }

        // si no existe aborta para no romper juego
        if (miPanelUI == null)
        {
            Debug.LogError($"[PlayerAlquimia] El Jugador {myPlayerInput?.playerIndex} no tiene un Canvas UI asignado en Nivel1.");
            return;
        }

        if (prefabMinijuegoUnico == null) return;

        estaEnMinijuego = true;

        
        GameObject puzzleObj = Instantiate(prefabMinijuegoUnico);

        // asigna el padre usando puzzleObj
        puzzleObj.transform.SetParent(miPanelUI, false);

        MinijuegoBase minijuego = puzzleObj.GetComponent<MinijuegoBase>();

        if (minijuego != null)
        {
            minijuego.OnPuzzleExito += () =>
            {
                estaEnMinijuego = false;
                Destroy(puzzleObj);
                ExecuteAbilityLogic();
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
            estaEnMinijuego = false;
            Destroy(puzzleObj);
        }
    }

    protected void ActualizarObjetoEnMano(GameObject nuevoObjeto)
    {
        if (nuevoObjeto != heldItem)
        {
            heldItem = nuevoObjeto;
            heldItem.transform.SetParent(holdPoint);
            heldItem.transform.localPosition = Vector3.zero;
            heldItem.transform.localRotation = Quaternion.identity;

            if (heldItem.TryGetComponent<Rigidbody>(out var rb)) rb.isKinematic = true;
            if (heldItem.TryGetComponent<Collider>(out var col)) col.enabled = false;
        }
    }

    protected void ProcesarEvolucionEnMano(TipoProceso proceso)
    {
        if (heldItem == null) return;

        if (heldItem.TryGetComponent<Ingrediente>(out var ingrediente))
        {
            // nueva física ingrediente
            GameObject resultado = ingrediente.Evolucionar(proceso);

            // si se cambió el prefab que se acople
            if (resultado != heldItem)
            {
                heldItem = resultado;
                heldItem.transform.SetParent(holdPoint);
                heldItem.transform.localPosition = Vector3.zero;
                heldItem.transform.localRotation = Quaternion.identity;

                if (heldItem.TryGetComponent<Rigidbody>(out var rb)) rb.isKinematic = true;
                if (heldItem.TryGetComponent<Collider>(out var col)) col.enabled = false;
            }
        }
    }

    protected abstract void TryPickOrDrop();

    
    protected abstract void ExecuteAbilityLogic();

}
