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

    float mouseX;
    float mouseY;
    float offsetDistanceY;

    Transform player;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        offsetDistanceY = initialHeightOffset;

        // Инициализация значений для предотвращения изначального сильного подъема камеры
        mouseY = Mathf.Clamp(mouseY, cameraLimit.x, cameraLimit.y);

        // Lock and hide cursor with option isn't checked
        if (!clickToMoveCamera)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
        }

        // Load sensitivity from saved settings (if available)
        SaveLoad saveLoad = FindObjectOfType<SaveLoad>();
        if (saveLoad != null)
        {
            GameSettings settings = saveLoad.LoadSettings();
            sensitivity = settings.sensitivity;
        }
    }


    void Update()
    {
        // Follow player - camera offset
        transform.position = player.position + new Vector3(0, offsetDistanceY, 0);

        // Set camera zoom when mouse wheel is scrolled
        if (canZoom && Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            Camera.main.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * sensitivity * 2;
            // Ограничиваем значение поля зрения с помощью Mathf.Clamp
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, zoomLimit.x, zoomLimit.y);
        }

        // Checker for right click to move camera
        if (clickToMoveCamera && Input.GetAxisRaw("Fire2") == 0)
            return;

        // Calculate new position
        mouseX += Input.GetAxis("Mouse X") * sensitivity;
        mouseY += Input.GetAxis("Mouse Y") * sensitivity;

        // Apply camera limits
        mouseY = Mathf.Clamp(mouseY, cameraLimit.x, cameraLimit.y);

        transform.rotation = Quaternion.Euler(-mouseY, mouseX, 0);
    }
}