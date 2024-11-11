using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public enum SeaType
{
    First,
    Second,
    Third
    // добавьте другие моря, если нужно
}

public class FishData : ItemData
{
    public GameObject Prefab;
    public Material[] Materials;  // Массив автоматически загружаемых материалов
    public float RandomSize;
    public Material RandomMaterial;
    public int Experience;
    public SeaType SeaType;

    public static readonly int BasePrice = 100;

    // Словарь для коэффициентов стоимости в зависимости от материала
    private static readonly Dictionary<Material, float> MaterialPriceMultipliers = new Dictionary<Material, float>();

    public FishData(GameObject prefab, string name, Sprite sprite, int experience, SeaType seaType)
    {
        Prefab = prefab;
        Name = name;
        Sprite = sprite;
        SeaType = seaType;

        // Загрузить материалы на основе текущего моря
        Materials = LoadMaterialsByCategory();

        InitializeMaterialMultipliers();
        RandomMaterial = GetRandomMaterial();
        RandomSize = GetRandomSize();
        Price = CalculatePrice();
        Experience = Price / 2;
    }

    private Material[] LoadMaterialsByCategory()
    {
        string seaPath = SeaType.ToString();  // Получаем строку с названием моря
        Debug.Log($"Loading materials for sea: {seaPath}");

        // Загружаем материалы для текущего моря
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

    private void InitializeMaterialMultipliers()
    {
        if (MaterialPriceMultipliers.Count > 0) return;

        float multiplier = 1.0f;
        foreach (var material in Materials)
        {
            MaterialPriceMultipliers[material] = multiplier;
            multiplier += 0.15f;
        }
    }

    // Метод для выбора случайного материала с учетом вероятности
    private Material GetRandomMaterial()
    {
        // Создаём список категорий материалов с вероятностями для текущего моря
        List<(Material[] materials, float probability)> materialCategories = new List<(Material[], float)>
        {
            (Resources.LoadAll<Material>($"Materials/{SeaType.ToString()}/Default"), 0.50f),
            (Resources.LoadAll<Material>($"Materials/{SeaType.ToString()}/Rare"), 0.28f),
            (Resources.LoadAll<Material>($"Materials/{SeaType.ToString()}/Epic"), 0.15f),
            (Resources.LoadAll<Material>($"Materials/{SeaType.ToString()}/Mythic"), 0.05f),
            (Resources.LoadAll<Material>($"Materials/{SeaType.ToString()}/Legendary"), 0.02f)
        };

        float totalWeight = materialCategories.Sum(category => category.probability);

        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0;

        foreach (var category in materialCategories)
        {
            currentWeight += category.probability;
            if (randomValue <= currentWeight)
            {
                if (category.materials.Length == 0)
                {
                    Debug.LogError("Category has no materials.");
                    continue;
                }
                int randomIndex = Random.Range(0, category.materials.Length);
                return category.materials[randomIndex];
            }
        }

        // Если ничего не найдено, возвращаем null
        return null;
    }

    // Метод для выбора случайного размера с учетом вероятности
    private float GetRandomSize()
    {
        // Генерируем случайное значение от 0 до 1
        float randomValue = Random.value;

        // Диапазоны для размеров с их вероятностями
        if (randomValue <= 0.85f)
        {
            // Диапазон от 0.5 до 2 (85% вероятности)
            return Random.Range(0.5f, 2f);
        }
        else if (randomValue <= 0.98f)
        {
            // Диапазон от 2 до 6 (13% вероятности)
            return Random.Range(2f, 6f);
        }
        else
        {
            // Диапазон от 6 до 10 (2% вероятности)
            return Random.Range(6f, 10f);
        }
    }

    public FishData Clone()
    {
        FishData clone = new FishData(Prefab, Name, Sprite, Experience, SeaType);
        clone.RandomMaterial = RandomMaterial;
        clone.RandomSize = RandomSize;
        clone.Price = Price;
        clone.SeaType = SeaType;
        return clone;
    }

    public int CalculatePrice()
    {
        float materialMultiplier = MaterialPriceMultipliers.ContainsKey(RandomMaterial)
            ? MaterialPriceMultipliers[RandomMaterial]
            : 1f;

        return Mathf.RoundToInt(BasePrice * RandomSize * materialMultiplier);
    }

    public static float GetMaterialMultiplier(Material material)
    {
        return MaterialPriceMultipliers.ContainsKey(material) ? MaterialPriceMultipliers[material] : 1.0f;
    }
}
