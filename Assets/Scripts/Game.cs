using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [Header("Fish for Each Sea")]
    [SerializeField] private List<FishData> firstSeaFish;  // Fish for the First Sea
    [SerializeField] private List<FishData> secondSeaFish; // Fish for the Second Sea
    [SerializeField] private List<FishData> thirdSeaFish;  // Fish for the Third Sea

    private InventoryUI inventoryUI;

    public void Start()
    {
        inventoryUI = FindObjectOfType<InventoryUI>();
    }

    public void AddFishButton()
    {
        // Replace with actual sea type logic
        SeaType currentSeaType = SeaType.Стандартное;

        FishData caughtFish = CreateFishFromSea(currentSeaType);
        if (caughtFish != null)
        {
            GameObject fishModel = Instantiate(caughtFish.Prefab, Vector3.zero, Quaternion.identity);
            fishModel.SetActive(false);

            Inventory.instance.AddFishItem(caughtFish, fishModel);
            inventoryUI.UpdateInventoryUI();
        }
    }

    private FishData GetRandomFishFromSea(SeaType seaType)
    {
        List<FishData> selectedSeaFish = null;

        switch (seaType)
        {
            case SeaType.Стандартное:
                selectedSeaFish = firstSeaFish;
                break;
            case SeaType.Ледовитое:
                selectedSeaFish = secondSeaFish;
                break;
            case SeaType.Лавовое:
                selectedSeaFish = thirdSeaFish;
                break;
        }

        if (selectedSeaFish == null || selectedSeaFish.Count == 0)
        {
            Debug.LogWarning($"No fish available for {seaType}!");
            return null;
        }

        // Return a random fish from the selected list
        return selectedSeaFish[Random.Range(0, selectedSeaFish.Count)];
    }

    public FishData CreateFishFromSea(SeaType seaType)
    {
        FishData baseFish = GetRandomFishFromSea(seaType);
        if (baseFish == null) return null;

        // Create a new instance based on the selected fish
        FishData newFish = Instantiate(baseFish);

        InitializeFishData(newFish);

        return newFish;
    }

    private void InitializeFishData(FishData fishData)
    {
        fishData.Materials = fishData.LoadMaterialsByCategory();
        fishData.RandomMaterial = fishData.GetRandomMaterial();
        fishData.RandomSize = fishData.GetRandomSize();
        fishData.Price = fishData.CalculatePrice();
        fishData.Experience = fishData.Price / 2;
    }
}
