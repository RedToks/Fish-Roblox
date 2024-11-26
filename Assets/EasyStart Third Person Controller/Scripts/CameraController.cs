using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Tooltip("Enable to move the camera by holding the right mouse button. Does not work with joysticks.")]
    public bool clickToMoveCamera = false;

    [Tooltip("Enable zoom in/out when scrolling the mouse wheel. Does not work with joysticks.")]
    public bool canZoom = true;

    [Space]
    [Tooltip("The higher it is, the faster the camera moves. It is recommended to increase this value for games that use joysticks.")]
    public float sensitivity = 5f;

    [Tooltip("Camera Y rotation limits. The X axis is the maximum it can go up and the Y axis is the maximum it can go down.")]
    public Vector2 cameraLimit = new Vector2(-45, 40);

    [Tooltip("Minimum and maximum zoom for the camera.")]
    public Vector2 zoomLimit = new Vector2(20, 60);

    [Tooltip("Initial camera height offset.")]
    public float initialHeightOffset = 5f;

    private float mouseX;
    private float mouseY;
    private float offsetDistanceY;

    private Transform player;
    private Camera mainCamera;  // Cached reference for Camera.main

    void Start()
    {
        // Find player and cache camera
        player = GameObject.FindWithTag("Player").transform;
        mainCamera = Camera.main;

        offsetDistanceY = initialHeightOffset;

        // Clamp initial mouseY to camera limits to avoid initial jerky movements
        mouseY = Mathf.Clamp(mouseY, cameraLimit.x, cameraLimit.y);

        // Lock and hide cursor if 'clickToMoveCamera' is not enabled
        if (!clickToMoveCamera)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Load settings only once during start
        SaveLoad saveLoad = FindObjectOfType<SaveLoad>();
        if (saveLoad != null)
        {
            GameSettings settings = saveLoad.LoadSettings();
            sensitivity = settings.sensitivity;
        }
    }

    void Update()
    {
        // Follow player, camera offset
        transform.position = player.position + Vector3.up * offsetDistanceY;

        HandleZoom();

        // Only allow camera movement if the right mouse button is pressed (clickToMoveCamera)
        if (clickToMoveCamera && Input.GetMouseButton(1))  // Use Input.GetMouseButton(1) instead of GetAxisRaw
        {
            HandleCameraMovement();
        }
        else if (!clickToMoveCamera)
        {
            HandleCameraMovement();
        }
    }

    private void HandleZoom()
    {
        if (canZoom)
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (scrollInput != 0)
            {
                mainCamera.fieldOfView -= scrollInput * sensitivity * 2;
                mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView, zoomLimit.x, zoomLimit.y);
            }
        }
    }

    private void HandleCameraMovement()
    {
        // Calculate mouse movement based on sensitivity
        mouseX += Input.GetAxis("Mouse X") * sensitivity;
        mouseY -= Input.GetAxis("Mouse Y") * sensitivity;

        // Apply camera limits
        mouseY = Mathf.Clamp(mouseY, cameraLimit.x, cameraLimit.y);

        // Update camera rotation
        transform.rotation = Quaternion.Euler(mouseY, mouseX, 0);
    }

    public void SetSensitivity(float newSensitivity)
    {
        sensitivity = newSensitivity;
    }
}
