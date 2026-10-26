using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EndObject : MonoBehaviour
{
    public GameObject gradient;
    public float speedModifier;
    public float weightModifier;
    Vector3 gradientDefScale;

    public List<PlatformerPlayer> players;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gradientDefScale = gradient.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        gradient.transform.localScale = new Vector3(gradientDefScale.x, gradientDefScale.y + (Mathf.Sin(Time.time * speedModifier) * weightModifier), gradientDefScale.z);

        int numberOfPlayers = players.Sum(x => x.GetOnFloor() ? 1 : 0);

        if (numberOfPlayers >= LocalMultiplayerInputManager.instance.inputManager.maxPlayerCount)
        {
            (GameManagerBase.instance as PlatformerManager).EndLevel();
            this.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player")
            return;
        
        players.Add(collision.gameObject.GetComponent<PlatformerPlayer>());
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag != "Player")
            return;

        players.Remove(collision.gameObject.GetComponent<PlatformerPlayer>());
    }
}
