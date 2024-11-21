using UnityEngine;

public class NamePlate : MonoBehaviour
{
    private Camera targetCamera;

    // ��������� ������, �� ������� ����� �������� ������
    public void SetTargetCamera(Camera camera)
    {
        targetCamera = camera;
    }

    private void LateUpdate()
    {
        if (targetCamera != null)
        {
            // ������������ ������ � ������
            transform.rotation = Quaternion.LookRotation(transform.position - targetCamera.transform.position);
        }
    }
}
