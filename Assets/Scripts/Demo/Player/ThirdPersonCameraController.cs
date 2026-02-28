using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCameraController : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float zoomLerpSpeed = 10f;
    [SerializeField] private float minDistance = 3f;
    [SerializeField] private float maxDistance = 15f;

    private CinemachineOrbitalFollow orbital;
    private CinemachineInputAxisController axisController;

    private float targetZoom;
    private float currentZoom;

    void Start()
    {
        orbital = GetComponent<CinemachineOrbitalFollow>();
        axisController = GetComponent<CinemachineInputAxisController>();

        targetZoom = currentZoom = orbital.Radius;
    }

    void Update()
    {
        HandleRightMouseMode();
        HandleZoom();
    }

    void HandleRightMouseMode()
    {
        bool rightMouse = Mouse.current.rightButton.isPressed;

        // Only enable camera rotation when right mouse is held
        axisController.enabled = rightMouse;

        Cursor.lockState = rightMouse ?
            CursorLockMode.Locked :
            CursorLockMode.None;

        Cursor.visible = !rightMouse;
    }

    void HandleZoom()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll != 0)
        {
            targetZoom = Mathf.Clamp(
                orbital.Radius - scroll * zoomSpeed,
                minDistance,
                maxDistance);
        }

        currentZoom = Mathf.Lerp(
            currentZoom,
            targetZoom,
            Time.deltaTime * zoomLerpSpeed);

        orbital.Radius = currentZoom;
    }
}