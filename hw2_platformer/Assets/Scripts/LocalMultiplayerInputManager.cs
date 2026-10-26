using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class LocalMultiplayerInputManager : MonoBehaviour
{
    public static LocalMultiplayerInputManager instance;

    public List<Gamepad> inputGamepads;
    public List<GameObject> inputGamepadGameObjects;

    public GameObject playerPrefab;
    public PlayerInputManager inputManager;

    public GameObject inputWASD;
    public GameObject inputArrows;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (instance != null)
        {
            print($"INSTANCE OF {this.GetType()} ALREADY EXISTS.");
            Destroy(gameObject);
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
        if (AmountOfPlayers() >= inputManager.maxPlayerCount)
            return;

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

            if (GameManagerBase.instance != null)
                GameManagerBase.instance.ConnectPlayer(inputWASD.GetComponent<LocalPlayerObject>());
        }

        if (inputArrows == null && Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            var Player = PlayerInput.Instantiate(playerPrefab, controlScheme: "Arrows", pairWithDevice: Keyboard.current);
            inputArrows = Player.gameObject;

            if (GameManagerBase.instance != null)
                GameManagerBase.instance.ConnectPlayer(inputArrows.GetComponent<LocalPlayerObject>());
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

        if (GameManagerBase.instance != null)
            GameManagerBase.instance.ConnectPlayer(Player.gameObject.GetComponent<LocalPlayerObject>());
    }

    public int AmountOfPlayers()
    {
        int players = 0;

        if (inputWASD != null) players++;
        if (inputArrows != null) players++;

        for (int i = 0; i < inputGamepads.Count; i++)
        {
            if (inputGamepads[i] != null)
                players++;
        }

        return players;
    }

    public void DisconnectPlayer(LocalPlayerObject p)
    {
        GameObject g = p.gameObject;

        for (int i = 0; i < inputGamepads.Count; i++)
        {
            if (inputGamepadGameObjects[i] != g)
                continue;

            inputGamepadGameObjects.RemoveAt(i);
            inputGamepads.RemoveAt(i);

            break;
        }

        if (GameManagerBase.instance != null)
            GameManagerBase.instance.DisconnectPlayer(p);

        Destroy(g);
    }
}
