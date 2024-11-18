using System.Collections;
using UnityEngine;

public class FishObject : MonoBehaviour
{
    public FishData fishData;
    private Coroutine destroyCoroutine;
    private const float timeBeforeDestruction = 30f; // ����� �� �������� ���� (30 ������)

    private void Start()
    {
        // ��������� Coroutine ��� ������������ ������� ����� ����
        destroyCoroutine = StartCoroutine(DestroyAfterTime());
    }

    private IEnumerator DestroyAfterTime()
    {
        float remainingTime = timeBeforeDestruction;

        while (remainingTime > 0)
        {
            Debug.Log($"�������� �������: {remainingTime:F1} ������");
            remainingTime -= 1f;  // ��������� ����� �� 1 �������
            yield return new WaitForSeconds(1f);  // ���� 1 �������
        }

        // ���� ��������� �� ��������, ���� ����� �������
        if (Inventory.instance.IsFull())
        {
            Debug.Log("��������� �����, ���� �� ����� ���� �������.");
        }
        else
        {
            Debug.Log("���� ������� �������.");
            // �������� ������ ���������� ���� � ��������� ����� (��������, ����� ������ ��� ���������� ���� � ���������)
            // Inventory.instance.AddFishItem(fishData, gameObject);
            Destroy(gameObject); // ������� ���� �� �����
        }
    }

    public void InitializeFish(FishData fishData)
    {
        if (fishData == null)
        {
            Debug.LogError("fishData is null! Cannot apply fish data.");
            return;
        }

        this.fishData = fishData;

        // ������������� ��������� �� ������ ��� ������������ ������
        float size = fishData.RandomSize;
        transform.localScale = new Vector3(size, size, size);

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.mass = size * size * size;
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.drag = 3f;
            rb.angularDrag = 3f;
        }

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (fishData.RandomMaterial != null)
            {
                renderer.material = fishData.RandomMaterial;
            }
            else
            {
                Debug.LogWarning("RandomMaterial is null!");
            }
        }
    }

    public void TryPickUpFish()
    {
        // ���� ��������� ������, ���� �� ����� ���� �������
        if (Inventory.instance.IsFull())
        {
            Debug.Log("��������� ������, ���� �� ����� ���� �������.");
            return; // ����� �� ������, �� ������� ����
        }

        // ���� ��������� �� ������, �������� ������� ����
        if (destroyCoroutine != null)
        {
            StopCoroutine(destroyCoroutine);
            destroyCoroutine = null;
        }

        // ����� ����� �������� ������ ��� ���������� ���� � ���������
        Inventory.instance.AddFishItem(fishData, gameObject);
        Debug.Log("���� ������� �������.");
        Destroy(gameObject); // ������� ���� �� �����, ���� ��� �������
    }
}
