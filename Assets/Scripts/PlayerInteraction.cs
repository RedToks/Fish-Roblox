using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public InventoryUI inventoryUI;  // UI ��� ���������
    public float throwForce = 10f;  // ���� ������� ����
    public Transform throwPoint;  // ����� ������� ���� (������ ������ ����� �������)

    private FishData fishData;  // ������ ����, ������� ����� ����� �������
    private GameObject fishObject;  // ������ ����, � ������� ��������������� �����
    private bool isNearFish = false;  // ���������� ��� ��������, ��������� �� ����� ����� � �����

    private FishingAreaTrigger currentFishingArea;

    void Update()
    {
        // ���� ������ ������� F � ����� ����� � �����
        if (Input.GetKeyDown(KeyCode.F) && isNearFish)
        {
            AddFishToInventory();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out FishObject fish))
        {
            isNearFish = true;
            fishObject = other.gameObject;
            fishData = fish?.fishData;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out FishObject fish))
        {
            if (other.gameObject == fishObject)
            {
                isNearFish = false;
                fishObject = null;
                fishData = null;
            }
        }
    }

    public void SetCurrentFishingArea(FishingAreaTrigger fishingArea)
    {
        currentFishingArea = fishingArea;
    }

    public void AddFishToInventory()
    {
            FishData uniqueFishData = fishData.Clone();
            Inventory.instance.AddFishItem(uniqueFishData, fishObject);
            Destroy(fishObject);
            fishObject = null;
            fishData = null;
            inventoryUI.UpdateInventoryUI();
    }

    public void ThrowFish(FishData fish)
    {
        if (fish != null)   
        {
            // ������� ����� ������ ���� � ����
            var fishObject = Instantiate(fish.Prefab, throwPoint.position, Quaternion.identity);
            var fishObjectScript = fishObject.GetComponent<FishObject>();

            // �������������� ����� ������ ���� � ��� ����������� �������
            fishObjectScript.InitializeFish(fish);

            // ��������� ������ ����
            var rb = fishObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(throwPoint.forward * throwForce, ForceMode.VelocityChange);
            }

            // ������� ���� �� ��������� ����� �������
            Inventory.instance.RemoveItem(fish);
            inventoryUI.UpdateInventoryUI();
        }
        else
        {
            Debug.LogError("������� �� FishData ������ ��� �������.");
        }
    }
}
