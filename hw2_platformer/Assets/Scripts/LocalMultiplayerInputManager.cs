using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class LocalMultiplayerInputManager : MonoBehaviour
{
    static LocalMultiplayerInputManager instance;

    public List<Gamepad> inputGamepads;
    public List<GameObject> inputGamepadGameObjects;

    public GameObject playerPrefab;

    public GameObject inputWASD;
    public GameObject inputArrows;

    private void Awake()
    {
        if (instance != null)
        {
            print($"INSTANCE OF {this.GetType()} ALREADY EXISTS.");
            Destroy(this);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        inputGamepads = new List<Gamepad>();
        inputGamepadGameObjects = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        KeyboardManager();
        GamepadManager();
    }

    public void KeyboardManager()
    {
        if (Keyboard.current == null) return;

        if (inputWASD == null && Keyboard.current.wKey.wasPressedThisFrame)
        {
            var Player = PlayerInput.Instantiate(playerPrefab, controlScheme: "WASD", pairWithDevice: Keyboard.current);
            inputWASD = Player.gameObject;
        }

        if (inputArrows == null && Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            var Player = PlayerInput.Instantiate(playerPrefab, controlScheme: "Arrows", pairWithDevice: Keyboard.current);
            inputArrows = Player.gameObject;
        }
    }

    public void GamepadManager()
    {
        if (Gamepad.current == null) return;

        if (inputGamepads.Contains(Gamepad.current) && inputGamepadGameObjects[inputGamepads.IndexOf(Gamepad.current)] != null)
            return;

        if (!Gamepad.current.allControls.Any(control =>
            control is ButtonControl button &&
            button.wasPressedThisFrame)) return;

        var Player = PlayerInput.Instantiate(playerPrefab, controlScheme: "Gamepad", pairWithDevice: Gamepad.current);
        
        inputGamepads.Add(Gamepad.current);
        inputGamepadGameObjects.Add(Player.gameObject);
    }
}
