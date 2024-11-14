using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    public List<ItemData> items = new List<ItemData>();
    private int maxInventorySize = 6;
    [SerializeField] private FishingRodData firstFishingRod;
    private InventoryUI inventoryUI;

    // Переменная для отслеживания уровня удочки
    public int RodLvl { get; private set; } = 1;  // Начальный уровень удочки - 1

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        inventoryUI = FindObjectOfType<InventoryUI>();
        AddInitialItem(firstFishingRod);
    }

    private void AddInitialItem(FishingRodData item)
    {
        // Check if the item is already in the inventory
        if (item != null && !items.Contains(firstFishingRod) && items.Count < maxInventorySize)
        {
            items.Insert(0, item); // Insert at the first slot
            Debug.Log($"{item.Name} has been added to the first slot of the inventory.");
            inventoryUI.UpdateInventoryUI();

            // Устанавливаем уровень удочки, если она добавлена
            SetFishingRodLevel(item);
        }
        else if (items.Contains(item))
        {
            Debug.LogWarning($"{item.Name} is already in the inventory.");
        }
        else
        {
            Debug.LogWarning("Failed to add the initial item to the inventory.");
        }
    }

    public bool IsFull()
    {
        return items.Count >= maxInventorySize;
    }

    public void ReplaceItemInInventory(ItemData newItem)
    {
        int index = items.FindIndex(item => item is FishingRodData);
        if (index >= 0)
        {
            items[index] = newItem;
            inventoryUI.UpdateInventoryUI();

            // Устанавливаем уровень удочки, если она заменена
            SetFishingRodLevel(newItem as FishingRodData);
        }
    }

    public void AddFishItem(FishData item, GameObject fishModel)
    {
        if (item == null || fishModel == null)
        {
            Debug.LogWarning("Item or fish model is null, cannot add to inventory.");
            return;
        }

        if (!IsFull())
        {
            // Генерируем иконку для рыбы
            Sprite icon = GenerateIcon(fishModel, item.Material);
            item.SetIcon(icon);

            items.Add(item);
            Debug.Log("Item added to inventory.");
        }
        else
        {
            Debug.Log("Inventory is full.");
        }
    }

    public void RemoveItem(ItemData item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            Debug.Log("Item removed from inventory.");
        }
        else
        {
            Debug.LogWarning("Item not found in inventory.");
        }
    }

    private void SetFishingRodLevel(FishingRodData rodData)
    {
        if (rodData == null) return;

        // Устанавливаем уровень удочки в зависимости от ее типа
        if (rodData.Name.Contains("lvl1"))
        {
            RodLvl = 1;
        }
        else if (rodData.Name.Contains("lvl2"))
        {
            RodLvl = 2;
        }
        else if (rodData.Name.Contains("lvl3"))
        {
            RodLvl = 3;
        }

        Debug.Log($"Fishing rod level set to: {RodLvl}");
    }

    public FishingRodData GetCurrentFishingRodData()
    {
        // Ищем текущую удочку по уровню
        return items.Find(item => item is FishingRodData rod && rod.Level == RodLvl) as FishingRodData;
    }

    private Sprite GenerateIcon(GameObject fishModel, Material fishMaterial)
    {
        // Включаем модель рыбы для рендеринга иконки
        fishModel.SetActive(true);

        // Поворачиваем рыбу на -90 градусов (например, вокруг оси Y)
        fishModel.transform.rotation = Quaternion.Euler(0, -90, 0);

        // Получаем все MeshRenderer в дочерних объектах
        MeshRenderer[] renderers = fishModel.GetComponentsInChildren<MeshRenderer>();
        if (renderers.Length == 0)
        {
            Debug.LogWarning("No MeshRenderers found on fish model or its children.");
            return null;
        }

        // Применяем материал ко всем найденным MeshRenderer
        foreach (MeshRenderer renderer in renderers)
        {
            renderer.material = fishMaterial;
        }

        // Настроим камеру для рендеринга модели в иконку
        Camera iconCamera = new GameObject("IconCamera").AddComponent<Camera>();
        iconCamera.transform.position = fishModel.transform.position + new Vector3(0, 1, -2);
        iconCamera.transform.LookAt(fishModel.transform);
        iconCamera.orthographic = true;
        iconCamera.orthographicSize = 0.5f; // Уменьшаем размер камеры
        iconCamera.clearFlags = CameraClearFlags.SolidColor;
        iconCamera.backgroundColor = Color.clear;

        // Создаём RenderTexture для рендера иконки
        RenderTexture renderTexture = new RenderTexture(256, 256, 16);
        iconCamera.targetTexture = renderTexture;
        iconCamera.Render();

        // Создаём иконку из RenderTexture
        RenderTexture.active = renderTexture;
        Texture2D iconTexture = new Texture2D(256, 256, TextureFormat.ARGB32, false);
        iconTexture.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
        iconTexture.Apply();
        RenderTexture.active = null;

        // Преобразуем Texture2D в Sprite
        Sprite icon = Sprite.Create(iconTexture, new Rect(0, 0, iconTexture.width, iconTexture.height), Vector2.one * 0.5f);

        Destroy(iconCamera.gameObject);
        Destroy(fishModel);

        return icon;
    }
}
