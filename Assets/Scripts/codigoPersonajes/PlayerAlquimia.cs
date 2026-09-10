using System;
using NUnit.Framework;
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

    [Header("UI Privada y Minijuego del Rol")]
    public GameObject prefabMinijuegoUnico;
    protected bool estaEnMinijuego = false;

    
    protected virtual void Awake()
    {
        controller = GetComponent<CharacterController>();
        myPlayerInput = GetComponent<PlayerInput>();
    }

    // En PlayerAlquimia.cs o en un componente selector
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
        if (UIPuzzleManager.Instance != null)
        {
            miPanelUI = UIPuzzleManager.Instance.ObtenerPanelJugador(playerIndex);
        }
    }

    /*protected virtual void Start()
    {
        // Si no se asignó un panel manualmente, lo busca en el UIManager usando su índice
        if (miPanelUI == null && myPlayerInput != null && UIPuzzleManager.Instance != null)
        {
            AsignarPanelUI(UIPuzzleManager.Instance.ObtenerPanelJugador(myPlayerInput.playerIndex));
        }
    }*/

    // Callbacks del Input System
    public void OnMove(InputValue value)
    {
        // Si está en el minijuego, fuerza el vector a cero e ignora la tecla
        if (estaEnMinijuego)
        {
            moveInput = Vector2.zero;
            return;
        }

        moveInput = value.Get<Vector2>();
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

        // Si existe el puzzle y el panel UI privado está configurado, lanza el minijuego
        if (prefabMinijuegoUnico != null && miPanelUI != null)
        {
            LanzarMinijuego();
        }
        else
        {
            // Si no hay minijuego, ejecuta directamente la habilidad de la subclase
            ExecuteAbilityLogic();
        }
    }

    private void LanzarMinijuego()
    {
        // Si no tienes nada en la mano, ni siquiera abras el minijuego
        if (heldItem == null)
        {
            Debug.LogWarning("[PlayerAlquimia] No puedes usar la habilidad: ¡Las manos están vacías!");
            return;
        }
        // Si miPanelUI no se asignó en el Lobby, lo reconecta con la escena de Juego actual
        if (miPanelUI == null && myPlayerInput != null)
        {
            ConfigurarCuadranteUI(myPlayerInput.playerIndex);
        }

        // Si aún así no existe el panel en la escena, aborta para no romper el juego
        if (miPanelUI == null)
        {
            Debug.LogError($"[PlayerAlquimia] El Jugador {myPlayerInput?.playerIndex} no tiene un Canvas UI asignado en Nivel1.");
            return;
        }

        if (prefabMinijuegoUnico == null) return;

        estaEnMinijuego = true;

        // 1. Instancia física en la escena activa
        GameObject puzzleObj = Instantiate(prefabMinijuegoUnico);

        // 2. Asigna el padre usando puzzleObj (NUNCA prefabMinijuegoUnico)
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
            // Genera la nueva versión física del ingrediente
            GameObject resultado = ingrediente.Evolucionar(proceso);

            // Si cambió a un Prefab diferente, acóplalo de nuevo a la mano
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

    // Método abstracto que escribirán Triturador, Piromano, Criogenico y Telecinetico
    protected abstract void ExecuteAbilityLogic();

}
