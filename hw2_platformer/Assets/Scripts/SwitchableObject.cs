using UnityEngine;

[RequireComponent (typeof(SwitchableObjectEditor))]
public class SwitchableObject : MonoBehaviour
{
    public int playerIndex = 0;
    public AnimatedObject animator;
    public GameObject[] objectsToDisable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        (GameManagerBase.instance as PlatformerManager).AddSwitchableObject(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Switch(int index)
    {
        foreach (GameObject g in objectsToDisable)
            g.SetActive(index == playerIndex);

        animator.changeAnimation(playerIndex == index ? 0 : 1);
    }
}
