using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    public List<ItemData> items = new List<ItemData>();
    private int maxInventorySize = 6;
    [SerializeField] private FishingRodData firstFishingRod;
    private InventoryUI inventoryUI;

    // Переменная для отслеживания уровня удочки
    [Serialize] public int RodLvl { get; private set; } = 1;  // Начальный уровень удочки - 1


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
        inventoryUI.UpdateInventoryUI();
        if (items.Count == 0)
        {
            AddInitialItem(firstFishingRod);
        }
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
        int index = items.FindIndex(item => item is FishingRodData rod && rod.Level == RodLvl);
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

        // Обновляем уровень, если он отличается от текущего
        if (rodData.Level != RodLvl)
        {
            RodLvl = rodData.Level;
            PlayerPrefs.SetInt("RodLvl", RodLvl);
            Debug.Log($"Fishing rod level set to: {RodLvl}");
        }
    }


    public FishingRodData GetCurrentFishingRodData()
    {
        // Ищем текущую удочку по уровню
        return items.Find(item => item is FishingRodData rod && rod.Level == RodLvl) as FishingRodData;
    }

    private Sprite GenerateIcon(GameObject fishModel, Material fishMaterial)
    {
        // Сохраняем исходное состояние рыбы
        Vector3 originalPosition = fishModel.transform.position;
        Quaternion originalRotation = fishModel.transform.rotation;
        Vector3 originalScale = fishModel.transform.localScale;

        // Перемещаем рыбу на новую позицию для рендера
        fishModel.transform.position = Vector3.zero;
        fishModel.transform.rotation = Quaternion.identity;
        fishModel.transform.localScale = Vector3.one;

        // Включаем модель рыбы
        fishModel.SetActive(true);

        // Получаем все Renderer компоненты (включая MeshRenderer) в дочерних объектах
        Renderer[] renderers = fishModel.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.LogWarning("No Renderers found on fish model or its children.");
            return null;
        }

        // Применяем материал ко всем найденным Renderer
        foreach (Renderer renderer in renderers)
        {
            renderer.material = fishMaterial;
        }

        // Создаем камеру для рендера
        Camera iconCamera = new GameObject("IconCamera").AddComponent<Camera>();
        iconCamera.orthographic = true;
        iconCamera.clearFlags = CameraClearFlags.SolidColor;
        iconCamera.backgroundColor = Color.clear;

        // Получаем все рендереры, чтобы вычислить размеры объекта
        Bounds bounds = renderers[0].bounds;
        foreach (var renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }

        // Находим центр объекта
        Vector3 center = bounds.center;

        // Устанавливаем камеру, чтобы она смотрела на центр объекта
        iconCamera.transform.position = center + new Vector3(0, 1, -2);  // Сдвигаем камеру на небольшое расстояние
        iconCamera.transform.LookAt(center);

        // Вычисляем подходящий размер камеры, чтобы весь объект поместился в кадр
        float objectHeight = bounds.size.y;  // Высота объекта
        float objectWidth = bounds.size.x;   // Ширина объекта
        float maxObjectSize = Mathf.Max(objectHeight, objectWidth);

        // Устанавливаем размер камеры в зависимости от объекта
        iconCamera.orthographicSize = maxObjectSize / 2 + 0.1f; // Немного добавляем для отступа

        // Создаем RenderTexture для рендера иконки
        RenderTexture renderTexture = new RenderTexture(256, 256, 16);
        iconCamera.targetTexture = renderTexture;
        iconCamera.Render();

        // Создаем иконку из RenderTexture
        RenderTexture.active = renderTexture;
        Texture2D iconTexture = new Texture2D(256, 256, TextureFormat.ARGB32, false);
        iconTexture.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
        iconTexture.Apply();
        RenderTexture.active = null;

        // Удаляем временную камеру и объект после рендера
        Destroy(iconCamera.gameObject);
        Destroy(fishModel);

        // Восстанавливаем исходное состояние рыбы
        fishModel.transform.position = originalPosition;
        fishModel.transform.rotation = originalRotation;
        fishModel.transform.localScale = originalScale;

        // Преобразуем Texture2D в Sprite
        Sprite icon = Sprite.Create(iconTexture, new Rect(0, 0, iconTexture.width, iconTexture.height), Vector2.one * 0.5f);

        return icon;
    }

}

