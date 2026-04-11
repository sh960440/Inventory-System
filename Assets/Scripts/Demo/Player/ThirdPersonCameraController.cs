using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCameraController : MonoBehaviour
{
    [Header("Gameplay")]
    [SerializeField] private Inventory inventory;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float zoomLerpSpeed = 10f;
    [SerializeField] private float minDistance = 3f;
    [SerializeField] private float maxDistance = 15f;

    private CinemachineOrbitalFollow _orbital;
    private CinemachineInputAxisController _axisController;
    private float _targetZoom;
    private float _currentZoom;

    private void Start()
    {
        _orbital = GetComponent<CinemachineOrbitalFollow>();
        _axisController = GetComponent<CinemachineInputAxisController>();

        _targetZoom = _currentZoom = _orbital.Radius;
    }

    private void Update()
    {
        if (inventory != null && inventory.IsOpen)
        {
            if (_axisController != null)
                _axisController.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        HandleRightMouseMode();
        HandleZoom();
    }

    private void HandleRightMouseMode()
    {
        bool rightMouse = Mouse.current != null && Mouse.current.rightButton.isPressed;

        // Only enable camera rotation when right mouse is held
        if (_axisController != null) 
            _axisController.enabled = rightMouse;

        Cursor.lockState = rightMouse ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !rightMouse;
    }

    private void HandleZoom()
    {
        if (Mouse.current == null || _orbital == null)
            return;

        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll != 0f)
        {
            _targetZoom = Mathf.Clamp(
                _orbital.Radius - scroll * zoomSpeed,
                minDistance,
                maxDistance);
        }

        _currentZoom = Mathf.Lerp(
            _currentZoom,
            _targetZoom,
            Time.deltaTime * zoomLerpSpeed);

        _orbital.Radius = _currentZoom;
    }
}