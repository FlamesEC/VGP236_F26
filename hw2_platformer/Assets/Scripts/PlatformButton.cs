using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlatformButton : MonoBehaviour
{
    public List<PlatformerPlayer> players;
    public List<GameObject> pressedObjects;
    public List<GameObject> unpressedObjects;
    public BoxCollider2D col;
    bool lastGetPressed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        players = new List<PlatformerPlayer>();

        foreach (GameObject obj in pressedObjects)
            obj.SetActive(GetPressed());
        foreach (GameObject obj in unpressedObjects)
            obj.SetActive(!GetPressed());
    }

    Collider2D[] CheckForPlayers()
    {
        if (col == null)
            return new Collider2D[0];

        print("CHECKING FOR PLAYERS.");

        Collider2D[] box = Physics2D.OverlapBoxAll(col.transform.position, col.size - new Vector2(0.0625f * 2, 0.0625f * 2), 0);

        return box;
    }

    public bool GetPressed()
    {
        return players.Any(x => x.GetOnFloor());
    }

    // Update is called once per frame
    void Update()
    {
        if (lastGetPressed == GetPressed())
            return;

        foreach (GameObject obj in pressedObjects)
            obj.SetActive(GetPressed());
        foreach (GameObject obj in unpressedObjects)
            obj.SetActive(!GetPressed());

        lastGetPressed = GetPressed();

        if (!GetPressed())
        {
            Collider2D[] cols = CheckForPlayers();
            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i].gameObject.tag != "Player")
                    continue;

                cols[i].gameObject.GetComponent<PlatformerPlayer>().KillPlayer();
            }
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
