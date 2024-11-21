using UnityEngine;

public class NamePlate : MonoBehaviour
{
    private Camera targetCamera;

    // ”становка камеры, на которую будет смотреть плашка
    public void SetTargetCamera(Camera camera)
    {
        targetCamera = camera;
    }

    private void LateUpdate()
    {
        if (targetCamera != null)
        {
            // ѕоворачиваем плашку к камере
            transform.rotation = Quaternion.LookRotation(transform.position - targetCamera.transform.position);
        }
    }
}
