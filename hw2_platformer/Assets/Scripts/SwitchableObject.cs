using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class SwitchableObject : MonoBehaviour
{
    public int playerIndex = 0;
    public AnimatedObject animator;
    public GameObject[] objectsToDisable;

    public BoxCollider2D col;
    public int queue = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        (GameManagerBase.instance as PlatformerManager).AddSwitchableObject(this);
    }

    bool CheckForPlayers()
    {
        if (col == null)
            return false;

        print("CHECKING FOR PLAYERS.");

        Collider2D[] box = Physics2D.OverlapBoxAll(col.transform.position, col.size - new Vector2(0.0625f * 2, 0.0625f * 2), 0);
        foreach (Collider2D hit in box) print(hit.gameObject);

        return box.Any(x => x.gameObject.tag == "Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (queue != -1 && !CheckForPlayers())
        {
            QueuedSwitch(queue);
            queue = -1;
        }
    }

    public void Switch(int index)
    {
        queue = -1;
        animator.changeAnimation(playerIndex == index ? 0 : 1);

        if (CheckForPlayers() && index == playerIndex)
        {
            animator.sr.color = new Color(animator.sr.color.r, animator.sr.color.g, animator.sr.color.b, 120f/255f);
            queue = index;
            return;
        }

        QueuedSwitch(index);
    }

    public void QueuedSwitch(int index)
    {
        foreach (GameObject g in objectsToDisable)
            g.SetActive(index == playerIndex);

        animator.sr.color = new Color(animator.sr.color.r, animator.sr.color.g, animator.sr.color.b, 1);
    }

    void OnDrawGizmos()
    {
        if (col == null)
            return;

        Gizmos.color = new Color(128, 128, 128, 128);
        //Gizmos.DrawCube(col.bounds.center, col.bounds.size);
    }
}
