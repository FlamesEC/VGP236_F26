using UnityEngine;

[RequireComponent(typeof(SwitchableObject))]
[ExecuteInEditMode]
public class SwitchableObjectEditor : MonoBehaviour
{
    PlatformerManager manager;
    SwitchableObject switchable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (manager == null)
            manager = FindAnyObjectByType<PlatformerManager>();

        switchable = gameObject.GetComponent<SwitchableObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (manager == null)
            return;

        switchable.playerIndex = Mathf.Clamp(switchable.playerIndex, 0, manager.players.Length - 1);

        switchable.animator.sr.color = manager.players[switchable.playerIndex].color;
    }
}
