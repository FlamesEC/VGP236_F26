using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class LocalPlayerObject : MonoBehaviour
{
    public delegate void Move(InputAction.CallbackContext ctx);
    public delegate void Jump(InputAction.CallbackContext ctx);
    public delegate void Switch(InputAction.CallbackContext ctx);

    public Move movePlayer;
    public Jump jumpPlayer;
    public Switch switchPlayer;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void InputMove(InputAction.CallbackContext ctx)
    {
        if (movePlayer == null)
            return;

        movePlayer(ctx);
    }

    public void InputJump(InputAction.CallbackContext ctx)
    {
        if (jumpPlayer == null)
            return;

        jumpPlayer(ctx);
    }

    public void InputSwitch(InputAction.CallbackContext ctx)
    {
        if (switchPlayer == null)
            return;

        switchPlayer(ctx);
    }

    public void DisconnectInput()
    {
        LocalMultiplayerInputManager.instance.DisconnectPlayer(this);
    }
}
