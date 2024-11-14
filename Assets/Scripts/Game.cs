using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [Header("Fish for Each Sea")]
    [SerializeField] private List<FishData> firstSeaFish;  // Fish for the First Sea
    [SerializeField] private List<FishData> secondSeaFish; // Fish for the Second Sea
    [SerializeField] private List<FishData> thirdSeaFish;  // Fish for the Third Sea

    [SerializeField] private FishData fishPrefabData; // ������ ���� ��� �������� ����� �����������
    private InventoryUI inventoryUI;

    // ����� ��� �������� ���� � ���������� � � ������
    public void Start()
    {
        inventoryUI = FindObjectOfType<InventoryUI>();
        AddFishToSeas();
    }
    private FishData CreateFishData(SeaType seaType)
    {
        // ������� ���� � �������� �����������
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
            // ������� 3D ������ ���� ��� ������
            GameObject fishModel = Instantiate(caughtFish.Prefab, Vector3.zero, Quaternion.identity);
            fishModel.SetActive(false); // ������ ��� ���������

            // ��������� ���� � � ������ � ���������
            Inventory.instance.AddFishItem(caughtFish, fishModel);
            inventoryUI.UpdateInventoryUI();
        }
    }
    // ����� ��� ������������� ���������� ����
    private void InitializeFishData(FishData fishData)
    {
        fishData.Materials = fishData.LoadMaterialsByCategory();
        fishData.RandomMaterial = fishData.GetRandomMaterial();
        fishData.RandomSize = fishData.GetRandomSize();
        fishData.Price = fishData.CalculatePrice();
        fishData.Experience = fishData.Price / 2;
    }

    // ����� ��� ���������� ���� � ��������������� ������ ����
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

    // ����� ��� �������� ���� ��� ������������� ����
    public FishData CreateFishFromSea(SeaType seaType)
    {
        // ������� ���� � �������� ������� ��� ���������� ����
        FishData newFish = CreateFishData(seaType);

        // �������������� ���� (���������, ������, ���� � �.�.)
        InitializeFishData(newFish);

        // ��������� ���� � ������ ��� ���������������� ����
        AddFishToSeaList(newFish, seaType);

        // ���������� ��������� ����
        return newFish;
    }

    // ����� ��� ���������� ���� �� ��� ���� (������ �������������)
    private void AddFishToSeas()
    {
        // ������� � ��������� ���� ��� ������� ����
        CreateFishFromSea(SeaType.First);
        CreateFishFromSea(SeaType.Second);
        CreateFishFromSea(SeaType.Third);
    }

}
