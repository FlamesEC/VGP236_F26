using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlatformerManager : GameManagerBase
{
    [Serializable]
    public class PlayerScene
    {
        public PlatformerPlayer player;
        public Color color;
    }

    public PlayerScene[] players;

    public int currentPlayerSwitch = 0;
    public List<SwitchableObject> switchObjects;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].player.playerIndex = i;
            players[i].color = players[i].player.animator.sr.color;
        }

        if (switchObjects == null)
            switchObjects = new List<SwitchableObject>();

        SpawnPlayers();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EndLevel()
    {
        StartCoroutine(EndLevelCoroutine());
    }

    IEnumerator EndLevelCoroutine()
    {
        for (int i = 0; i < players.Length; i++)
            players[i].player.enabled = false;

        yield return new WaitForSeconds(0.075f);

        PlatformerLevelData l = FindAnyObjectByType<PlatformerLevelData>();
        l.transitionAnimator.Play(l.transitionStart.name);
        
        yield return new WaitForSeconds(l.transitionStart.length);
        yield return new WaitForEndOfFrame();

        ChangeLevel();
    }

    public async void ChangeLevel()
    {
        switchObjects.Clear();
        
        foreach (PlayerScene player in players)
            player.player.gameObject.SetActive(false);

        int sceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (sceneIndex >= SceneManager.sceneCountInBuildSettings) sceneIndex = 0;

        currentPlayerSwitch = 0;

        await SceneManager.LoadSceneAsync(sceneIndex);

        SpawnPlayers();
    }

    public void SpawnPlayers()
    {
        PlatformerLevelData p = FindAnyObjectByType<PlatformerLevelData>();

        for (int i = 0; i < players.Length; i++)
        {
            players[i].player.transform.position = p.spawns[i].position;
            players[i].player.gameObject.SetActive(true);
            players[i].player.enabled = true;
        }
    }

    public override void ConnectPlayer(LocalPlayerObject p)
    {
        print("CONNECTING PLAYER...");

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].player.inputObject != null)
                continue;

            players[i].player.AddInput(p);
            break;
        }
    }

    public override void DisconnectPlayer(LocalPlayerObject p)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].player.inputObject != p)
                continue;

            players[i].player.RemoveInput();
            break;
        }
    }

    public void PlayerSwitchInput(int index)
    {
        print("SWITCHING TO " + index.ToString());

        currentPlayerSwitch = index;

        foreach (SwitchableObject s in switchObjects)
            s.Switch(index);
    }

    public void AddSwitchableObject(SwitchableObject obj)
    {
        if (switchObjects  == null)
            switchObjects = new List<SwitchableObject>();

        switchObjects.Add(obj);
        obj.animator.sr.color = players[obj.playerIndex].color;
        obj.Switch(currentPlayerSwitch);
    }

    private void OnDestroy()
    {
        foreach (PlayerScene p in players)
        {
            if (p != null && p.player != null && p.player.gameObject != null)
                Destroy(p.player.gameObject);
        }
    }
}
