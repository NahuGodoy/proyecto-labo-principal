using UnityEngine;
using UnityEngine.InputSystem;


public class CameraController : MonoBehaviour
{
    public Transform pivot;
    public float sensitivity= 0.2f;
    private Vector2 lookInput;
    private float yaw= 0f;
    private float pitch = 0f;

    public void OnLook (InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();

    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            UnlockCursor();
        }
        else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            LockCursor();
        }

        if (float.IsNaN(lookInput.x) || float.IsNaN(lookInput.y) ||
            float.IsInfinity(lookInput.x) || float.IsInfinity(lookInput.y))
        {
            lookInput = Vector2.zero;
        }

        yaw += lookInput.x * sensitivity;
        pitch-= lookInput.y * sensitivity;

        pitch = Mathf.Clamp(pitch, -45f, 60f);

        pivot.localRotation = Quaternion.Euler(pitch,yaw,0);
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
