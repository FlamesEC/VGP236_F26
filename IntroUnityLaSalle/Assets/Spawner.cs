using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private int myInt = 5;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Debug.Log("Spawnerrr");
        }
    }
}
