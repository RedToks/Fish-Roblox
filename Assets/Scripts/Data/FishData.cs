using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
public enum Rarity
{
    Common,
    Rare,
    Epic,
    Mythic,
    Legendary
}
public enum SeaType
{
    �����������,
    ���������,
    �������
}

public class FishData : ItemData
{
    public Rarity rarity;
    public Color RarityColor;
    public GameObject Prefab;
    public Material[] Materials;
    public float RandomSize;
    public Material RandomMaterial;
    public int Experience;
    public SeaType SeaType;
    public string SpritePath;

    public Material Material => RandomMaterial;

    public int BasePrice = 100;
    // ������� ��� ������������� ��������� � ����������� �� ���������
    private static readonly Dictionary<Material, float> MaterialPriceMultipliers = new Dictionary<Material, float>();
    public FishData(GameObject prefab, string name, Sprite sprite, SeaType seaType)
    {
        Prefab = prefab;
        Name = name;
        Sprite = sprite;
        SeaType = seaType;

        // Initialize materials, random size, and material multiplier
        Materials = LoadMaterialsByCategory();
        InitializeMaterialMultipliers();

        RandomMaterial = GetRandomMaterial();
        RandomSize = GetRandomSize();

        // Set Price and Experience, but don't re-set them unless necessary
        Price = CalculatePrice();
        Experience = Price / 2;  // Only calculate Experience once
        RarityColor = GetRarityColor();
    }


    public Color GetRarityColor()
    {
        switch (rarity)
        {
            case Rarity.Common:
                return Color.white;  // ����� ��� Common
            case Rarity.Rare:
                return new Color(0.14f, 0, 0.97f);  // ����� ��� Rare
            case Rarity.Epic:
                return new Color(0.75f, 0, 0.97f);  // ��������� ��� Epic
            case Rarity.Mythic:
                return new Color(0.97f, 0, 0);  // ��������� ��� Mythic
            case Rarity.Legendary:
                return new Color(1, 0.98f, 0);  // ������ ��� Legendary
            default:
                return Color.black;  // ����� �� ���������, ���� �������� �� �����������
        }
    }

    public Material[] LoadMaterialsByCategory()
    {
        string seaPath = SeaType.ToString();  // �������� ������ � ��������� ����
        Debug.Log($"Loading materials for sea: {seaPath}");

        // ��������� ��������� ��� �������� ����
        Material[] defaultMaterials = Resources.LoadAll<Material>($"Materials/{seaPath}/Default");
        Material[] rareMaterials = Resources.LoadAll<Material>($"Materials/{seaPath}/Rare");
        Material[] epicMaterials = Resources.LoadAll<Material>($"Materials/{seaPath}/Epic");
        Material[] mythicMaterials = Resources.LoadAll<Material>($"Materials/{seaPath}/Mythic");
        Material[] legendaryMaterials = Resources.LoadAll<Material>($"Materials/{seaPath}/Legendary");

        List<Material> allMaterials = new List<Material>();
        allMaterials.AddRange(defaultMaterials);
        allMaterials.AddRange(rareMaterials);
        allMaterials.AddRange(epicMaterials);
        allMaterials.AddRange(mythicMaterials);
        allMaterials.AddRange(legendaryMaterials);

        Debug.Log($"Total materials loaded for sea {seaPath}: {allMaterials.Count}");

        return allMaterials.ToArray();
    }

    public void InitializeMaterialMultipliers()
    {
        if (MaterialPriceMultipliers.Count > 0) return;

        float multiplier = 1.0f;
        foreach (var material in Materials)
        {
            MaterialPriceMultipliers[material] = multiplier;
            multiplier += 0.15f;
        }
    }

    // ����� ��� ������ ���������� ��������� � ������ �����������
    public Material GetRandomMaterial()
    {
        // �������� ������ ������� ������
        FishingRodData currentRod = Inventory.instance.GetCurrentFishingRodData();

        // ���� ������ ������ �� �������, ���������� �������� �� ���������
        if (currentRod == null)
        {
            Debug.LogWarning("Fishing rod data is missing.");
            return null;
        }

        // ������ ����������� �� ������ ������� ������
        List<(Material[] materials, float probability)> materialCategories = new List<(Material[], float)>
    {
        (Resources.LoadAll<Material>($"Materials/{SeaType.ToString()}/Default"), currentRod.rodlvlCommon),
        (Resources.LoadAll<Material>($"Materials/{SeaType.ToString()}/Rare"), currentRod.rodlvlRare),
        (Resources.LoadAll<Material>($"Materials/{SeaType.ToString()}/Epic"), currentRod.rodlvlEpic),
        (Resources.LoadAll<Material>($"Materials/{SeaType.ToString()}/Mythic"), currentRod.rodlvlMythic),
        (Resources.LoadAll<Material>($"Materials/{SeaType.ToString()}/Legendary"), currentRod.rodlvlLegendary)
    };

        // �������� ��������� �������� �� ������ ������������
        float randomValue = Random.value;
        float cumulativeProbability = 0;

        foreach (var category in materialCategories)
        {
            cumulativeProbability += category.probability;
            if (randomValue <= cumulativeProbability)
            {
                Material[] selectedMaterials = category.materials;
                if (selectedMaterials.Length > 0)
                {
                    Material selectedMaterial = selectedMaterials[Random.Range(0, selectedMaterials.Length)];
                    SetFishRarityBasedOnMaterial(selectedMaterial); // ����� ������ ��� ��������� ��������
                    return selectedMaterial;
                }
            }
        }

        return null;
    }





    private void SetFishRarityBasedOnMaterial(Material material)
    {
        if (material == null) return;

        // �������� ������� ������
        int rodLevel = Inventory.instance.RodLvl;

        // ������ ������� ���� � ����� Resources/Materials
        string basePath = $"Materials/{SeaType.ToString()}";

        // ���������� �������� �� ������ �����, � ������� ��������� ��������
        if (rodLevel >= 3 && IsMaterialInFolder(material, $"{basePath}/Legendary"))
        {
            rarity = Rarity.Legendary;
        }
        else if (rodLevel >= 2 && IsMaterialInFolder(material, $"{basePath}/Mythic"))
        {
            rarity = Rarity.Mythic;
        }
        else if (IsMaterialInFolder(material, $"{basePath}/Epic"))
        {
            rarity = Rarity.Epic;
        }
        else if (IsMaterialInFolder(material, $"{basePath}/Rare"))
        {
            rarity = Rarity.Rare;
        }
        else
        {
            rarity = Rarity.Common;
        }

        // ������������� ���� ��������
        RarityColor = GetRarityColor();
    }

    private bool IsMaterialInFolder(Material material, string folderPath)
    {
        // ��������� ��� ��������� � ��������� �����
        Material[] materialsInFolder = Resources.LoadAll<Material>(folderPath);

        // ���������, ���������� �� ������ �������� � ������ �����������
        return materialsInFolder != null && System.Array.Exists(materialsInFolder, m => m == material);
    }

    // ����� ��� ������ ���������� ������� � ������ �����������
    public float GetRandomSize()
    {
        // ���������� ��������� �������� �� 0 �� 1
        float randomValue = Random.value;

        // ��������� ��� �������� � �� �������������
        if (randomValue <= 0.85f)
        {
            // �������� �� 0.5 �� 2 (85% �����������)
            return Random.Range(0.5f, 2f);
        }
        else if (randomValue <= 0.98f)
        {
            // �������� �� 2 �� 6 (13% �����������)
            return Random.Range(2f, 6f);
        }
        else
        {
            // �������� �� 6 �� 10 (2% �����������)
            return Random.Range(6f, 10f);
        }
    }

    public FishData Clone()
    {
        FishData clone = new FishData(Prefab, Name, Sprite, SeaType);

        clone.RandomMaterial = RandomMaterial;
        clone.RandomSize = RandomSize;

        clone.rarity = rarity;
        clone.RarityColor = RarityColor;
        clone.Price = Price;
        clone.Experience = Experience;
        clone.SeaType = SeaType;
        return clone;
    }

    public int CalculatePrice()
    {
        // �������� ��������� ��� �������� (� ������ ���������� ��������� ��� ��������)


        // ������������ �������� ����
        float finalPrice = BasePrice * RandomSize * GetRarityMultiplier(rarity);

        // ��������� ���� � ���������� �
        return Mathf.RoundToInt(finalPrice);
    }

    public float GetRarityMultiplier(Rarity rarity)
    {
        // ������������� ��������� ��� ������� ���� ��������
        float multiplier = 1f;

        switch (rarity)
        {
            case Rarity.Common:
                multiplier = 1f;  // ��������� ��� Common
                break;
            case Rarity.Rare:
                multiplier = 1.6f;  // ��������� ��� Rare
                break;
            case Rarity.Epic:
                multiplier = 2.2f;  // ��������� ��� Epic
                break;
            case Rarity.Mythic:
                multiplier = 3.5f;  // ��������� ��� Mythic
                break;
            case Rarity.Legendary:
                multiplier = 6f;  // ��������� ��� Legendary
                break;
            default:
                Debug.LogWarning($"Unknown rarity: {rarity}");
                return 1f;
        }

        return multiplier;
    }

}
