using UnityEngine;

public class FishObject : MonoBehaviour
{
    public FishData fishData;

    public void InitializeFish(FishData fishData)
    {
        this.fishData = fishData;
        ApplyFishData();
    }

    private void ApplyFishData()
    {
        if (fishData != null)
        {
            // ������������� ������ ����
            float size = fishData.RandomSize;
            transform.localScale = new Vector3(size, size, size);

            // ������������� ����� ���� (����� ��������������� ������)
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                // ����� ���� ����� �������������� ��������� ��� ��� � �������
                rb.mass = size * size * size;  // ��������������� ������

                // ��������� ������ ��� �������������� ��������
                rb.useGravity = true; // ����� ���� �� ��������
                rb.isKinematic = false; // �� ��������� ����������

                // ����������� �������������
                rb.drag = 3f;  // ������������� �������
                rb.angularDrag = 3f;  // ������������� ��������
            }

            // ������������� ���������
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.material = fishData.RandomMaterial;
            }
        }
    }
}