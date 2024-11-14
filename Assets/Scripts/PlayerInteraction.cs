using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public InventoryUI inventoryUI;  // UI для инвентаря
    public float throwForce = 10f;  // Сила выброса рыбы
    public Transform throwPoint;  // Точка выброса рыбы (пустой объект перед игроком)

    private FishData fishData;  // Данные рыбы, которую игрок может забрать
    private GameObject fishObject;  // Объект рыбы, с которой взаимодействует игрок
    private bool isNearFish = false;  // Переменная для проверки, находится ли игрок рядом с рыбой

    private FishingAreaTrigger currentFishingArea;

    void Update()
    {
        // Если нажата клавиша F и игрок рядом с рыбой
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
            // Создаем новый объект рыбы в мире
            var fishObject = Instantiate(fish.Prefab, throwPoint.position, Quaternion.identity);
            var fishObjectScript = fishObject.GetComponent<FishObject>();

            // Инициализируем новый объект рыбы с его уникальными данными
            fishObjectScript.InitializeFish(fish);

            // Добавляем физику рыбе
            var rb = fishObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(throwPoint.forward * throwForce, ForceMode.VelocityChange);
            }

            // Убираем рыбу из инвентаря после выброса
            Inventory.instance.RemoveItem(fish);
            inventoryUI.UpdateInventoryUI();
        }
        else
        {
            Debug.LogError("Передан не FishData объект для выброса.");
        }
    }
}
