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

    // ���������� ��� ������������ ������ ������
    [Serialize] public int RodLvl { get; private set; } = 1;  // ��������� ������� ������ - 1


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

            // ������������� ������� ������, ���� ��� ���������
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

            // ������������� ������� ������, ���� ��� ��������
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
            // ���������� ������ ��� ����
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

        // ��������� �������, ���� �� ���������� �� ��������
        if (rodData.Level != RodLvl)
        {
            RodLvl = rodData.Level;
            PlayerPrefs.SetInt("RodLvl", RodLvl);
            Debug.Log($"Fishing rod level set to: {RodLvl}");
        }
    }


    public FishingRodData GetCurrentFishingRodData()
    {
        // ���� ������� ������ �� ������
        return items.Find(item => item is FishingRodData rod && rod.Level == RodLvl) as FishingRodData;
    }

    private Sprite GenerateIcon(GameObject fishModel, Material fishMaterial)
    {
        // ��������� �������� ��������� ����
        Vector3 originalPosition = fishModel.transform.position;
        Quaternion originalRotation = fishModel.transform.rotation;
        Vector3 originalScale = fishModel.transform.localScale;

        // ���������� ���� �� ����� ������� ��� �������
        fishModel.transform.position = Vector3.zero;
        fishModel.transform.rotation = Quaternion.identity;
        fishModel.transform.localScale = Vector3.one;

        // �������� ������ ����
        fishModel.SetActive(true);

        // �������� ��� Renderer ���������� (������� MeshRenderer) � �������� ��������
        Renderer[] renderers = fishModel.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.LogWarning("No Renderers found on fish model or its children.");
            return null;
        }

        // ��������� �������� �� ���� ��������� Renderer
        foreach (Renderer renderer in renderers)
        {
            renderer.material = fishMaterial;
        }

        // ������� ������ ��� �������
        Camera iconCamera = new GameObject("IconCamera").AddComponent<Camera>();
        iconCamera.orthographic = true;
        iconCamera.clearFlags = CameraClearFlags.SolidColor;
        iconCamera.backgroundColor = Color.clear;

        // �������� ��� ���������, ����� ��������� ������� �������
        Bounds bounds = renderers[0].bounds;
        foreach (var renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }

        // ������� ����� �������
        Vector3 center = bounds.center;

        // ������������� ������, ����� ��� �������� �� ����� �������
        iconCamera.transform.position = center + new Vector3(0, 1, -2);  // �������� ������ �� ��������� ����������
        iconCamera.transform.LookAt(center);

        // ��������� ���������� ������ ������, ����� ���� ������ ���������� � ����
        float objectHeight = bounds.size.y;  // ������ �������
        float objectWidth = bounds.size.x;   // ������ �������
        float maxObjectSize = Mathf.Max(objectHeight, objectWidth);

        // ������������� ������ ������ � ����������� �� �������
        iconCamera.orthographicSize = maxObjectSize / 2 + 0.1f; // ������� ��������� ��� �������

        // ������� RenderTexture ��� ������� ������
        RenderTexture renderTexture = new RenderTexture(256, 256, 16);
        iconCamera.targetTexture = renderTexture;
        iconCamera.Render();

        // ������� ������ �� RenderTexture
        RenderTexture.active = renderTexture;
        Texture2D iconTexture = new Texture2D(256, 256, TextureFormat.ARGB32, false);
        iconTexture.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
        iconTexture.Apply();
        RenderTexture.active = null;

        // ������� ��������� ������ � ������ ����� �������
        Destroy(iconCamera.gameObject);
        Destroy(fishModel);

        // ��������������� �������� ��������� ����
        fishModel.transform.position = originalPosition;
        fishModel.transform.rotation = originalRotation;
        fishModel.transform.localScale = originalScale;

        // ����������� Texture2D � Sprite
        Sprite icon = Sprite.Create(iconTexture, new Rect(0, 0, iconTexture.width, iconTexture.height), Vector2.one * 0.5f);

        return icon;
    }

}

