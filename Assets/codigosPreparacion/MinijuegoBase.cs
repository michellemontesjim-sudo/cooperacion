using UnityEngine;
using UnityEngine.InputSystem;
using System;

public abstract class MinijuegoBase : MonoBehaviour
{
    public Action OnPuzzleExito;
    public Action OnPuzzleFallo;

    protected PlayerInput playerInputVinculado;

    public virtual void InicializarPuzzle(PlayerInput pInput)
    {
        playerInputVinculado = pInput;
    }
}