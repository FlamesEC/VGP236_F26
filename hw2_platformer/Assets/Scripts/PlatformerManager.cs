using UnityEngine;

public class PlatformerManager : GameManagerBase
{
    public PlatformerPlayer playerOne;
    public PlatformerPlayer playerTwo;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void ConnectPlayer(LocalPlayerObject p)
    {
        print("CONNECTING PLAYER...");

        if (playerOne.inputObject == null)
            playerOne.AddInput(p);
        else if (playerTwo.inputObject == null)
            playerTwo.AddInput(p);
    }

    public override void DisconnectPlayer(LocalPlayerObject p)
    {
        if (p == playerOne.inputObject)
            playerOne.RemoveInput();
        else if (p == playerTwo.inputObject)
            playerTwo.RemoveInput();
    }
}
