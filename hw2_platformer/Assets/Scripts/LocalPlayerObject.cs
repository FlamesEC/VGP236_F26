using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class LocalPlayerObject : MonoBehaviour
{
    public delegate void Move(InputAction.CallbackContext ctx);
    public delegate void Jump(InputAction.CallbackContext ctx);

    public Move movePlayer;
    public Jump jumpPlayer;

    public void InputMove(InputAction.CallbackContext ctx)
    {
        if (movePlayer == null)
            return;

        movePlayer(ctx);
    }

    public void InputJump(InputAction.CallbackContext ctx)
    {
        if (movePlayer == null)
            return;

        jumpPlayer(ctx);
    }

    public void DisconnectInput()
    {
        LocalMultiplayerInputManager.instance.DisconnectPlayer(this);
    }
}
