using UnityEngine;
using UnityEngine.InputSystem;

public class AIAgentTarget : MonoBehaviour
{
    private PlayerAssetInput input;
    private InputAction clickAction;

    private void Awake()
    {
        input = new PlayerAssetInput();
        clickAction = input.Player.Attack;
        clickAction.performed += OnMouseClick;
    }

    private void OnEnable()
    {
        input.Enable();
        clickAction.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
        clickAction.Disable();
    }

    void OnMouseClick(InputAction.CallbackContext ctx)
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray camRay = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit camRayHitInfo;

        if (!Physics.Raycast(camRay, out camRayHitInfo, 100.0f))
            return;

        if (camRayHitInfo.collider == null || camRayHitInfo.collider.tag != "Untagged")
            return;

        transform.position = camRayHitInfo.point;
    }
}
