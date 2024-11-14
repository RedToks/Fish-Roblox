using UnityEngine;

public class FishObject : MonoBehaviour
{
    public FishData fishData;

    public void InitializeFish(FishData fishData)
    {
        if (fishData == null)
        {
            Debug.LogError("fishData is null! Cannot apply fish data.");
            return;
        }


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
            if (renderers.Length == 0)
            {
                Debug.LogWarning("No renderers found on fish object!");
            }

            foreach (Renderer renderer in renderers)
            {
                if (fishData.RandomMaterial != null)
                {
                    Debug.Log("Applying material: " + fishData.RandomMaterial.name);
                    renderer.material = fishData.RandomMaterial;
                }
                else
                {
                    Debug.LogWarning("RandomMaterial is null!");
                }
            }
        }
    }
}