using UnityEngine;
using UnityEngine.InputSystem;


public class CameraController : MonoBehaviour
{
    public Transform pivot;
    public float sensitivity= 0.2f;
    public float gamepadSensitivity = 120f;
    private Vector2 lookInput;
    private bool lookFromGamepad;
    private float yaw= 0f;
    private float pitch = 0f;

    public void OnLook (InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
        lookFromGamepad = context.control != null && context.control.device is Gamepad;

        if (GameManager.Instance != null && GameManager.Instance.JuegoTerminado)
            return;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.JuegoTerminado)
            return;

        if (PauseMenu.JuegoEsPausado) return;
        
        if (float.IsNaN(lookInput.x) || float.IsNaN(lookInput.y) ||
            float.IsInfinity(lookInput.x) || float.IsInfinity(lookInput.y))
        {
            lookInput = Vector2.zero;
        }

        

        float inputScale = lookFromGamepad
            ? gamepadSensitivity * Time.deltaTime
            : sensitivity;
        yaw += lookInput.x * inputScale;
        pitch-= lookInput.y * inputScale;

        pitch = Mathf.Clamp(pitch, -20f, 60f);

        pivot.localRotation = Quaternion.Euler(pitch,yaw,0);
    }


}
