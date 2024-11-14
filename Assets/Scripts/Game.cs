using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [Header("Fish for Each Sea")]
    [SerializeField] private List<FishData> firstSeaFish;  // Fish for the First Sea
    [SerializeField] private List<FishData> secondSeaFish; // Fish for the Second Sea
    [SerializeField] private List<FishData> thirdSeaFish;  // Fish for the Third Sea

    [SerializeField] private FishData fishPrefabData; // Префаб рыбы для создания новых экземпляров
    private InventoryUI inventoryUI;

    // Метод для создания рыбы и добавления её в список
    public void Start()
    {
        inventoryUI = FindObjectOfType<InventoryUI>();
        AddFishToSeas();
    }
    private FishData CreateFishData(SeaType seaType)
    {
        // Создаем рыбу с базовыми параметрами
        return new FishData(
            fishPrefabData.Prefab,
            fishPrefabData.Name,
            fishPrefabData.Sprite,
            seaType
        );
    }
    public void AddFishButton()
    {
        FishData caughtFish = CreateFishFromSea(SeaType.First);
        if (caughtFish != null)
        {
            // Создаем 3D объект рыбы для иконки
            GameObject fishModel = Instantiate(caughtFish.Prefab, Vector3.zero, Quaternion.identity);
            fishModel.SetActive(false); // Делаем его невидимым

            // Добавляем рыбу и её модель в инвентарь
            Inventory.instance.AddFishItem(caughtFish, fishModel);
            inventoryUI.UpdateInventoryUI();
        }
    }
    // Метод для инициализации параметров рыбы
    private void InitializeFishData(FishData fishData)
    {
        fishData.Materials = fishData.LoadMaterialsByCategory();
        fishData.RandomMaterial = fishData.GetRandomMaterial();
        fishData.RandomSize = fishData.GetRandomSize();
        fishData.Price = fishData.CalculatePrice();
        fishData.Experience = fishData.Price / 2;
    }

    // Метод для добавления рыбы в соответствующий список моря
    private void AddFishToSeaList(FishData fishData, SeaType seaType)
    {
        switch (seaType)
        {
            case SeaType.First:
                firstSeaFish.Add(fishData);
                break;
            case SeaType.Second:
                secondSeaFish.Add(fishData);
                break;
            case SeaType.Third:
                thirdSeaFish.Add(fishData);
                break;
        }
    }

    // Метод для создания рыбы для определенного моря
    public FishData CreateFishFromSea(SeaType seaType)
    {
        // Создаем рыбу с базовыми данными для указанного моря
        FishData newFish = CreateFishData(seaType);

        // Инициализируем рыбу (материалы, размер, цена и т.д.)
        InitializeFishData(newFish);

        // Добавляем рыбу в список для соответствующего моря
        AddFishToSeaList(newFish, seaType);

        // Возвращаем созданную рыбу
        return newFish;
    }

    // Метод для добавления рыбы во все моря (пример использования)
    private void AddFishToSeas()
    {
        // Создаем и добавляем рыбу для каждого моря
        CreateFishFromSea(SeaType.First);
        CreateFishFromSea(SeaType.Second);
        CreateFishFromSea(SeaType.Third);
    }

}
