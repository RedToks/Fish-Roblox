using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ItemData itemData { get; private set; }

    public void Initialize(ItemData data)
    {
        itemData = data;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemData is FishData fishData)
        {
            string materialName = fishData.RandomMaterial.name;
            float materialMultiplier = FishData.GetMaterialMultiplier(fishData.RandomMaterial);

            Tooltip.Show(
                $"Имя: {fishData.Name} (<color=#{ColorUtility.ToHtmlStringRGB(GetMaterialColor(fishData))}>{materialName}</color>)\n" +
                $"Размер: {fishData.RandomSize:F2}\n" +
                $"Базовая стоимость: {FishData.BasePrice} + {materialMultiplier:F2}x\n" +
                $"Стоимость: {CurrencyFormatter.FormatCurrency(fishData.Price)}\n" +
                $"Опыт: {fishData.Experience}\n" +
                $"Море: {fishData.SeaType}"
            );
        }
        else
        {
            Tooltip.Show($"Имя: {itemData.Name}");
        }
    }

    // Функция для получения цвета материала на основе его категории с использованием RGB
    public Color GetMaterialColor(FishData fishData)
    {
        if (fishData?.RandomMaterial == null)
        {
            Debug.LogError("Material is null in GetMaterialColor.");
            return Color.black;
        }

        // Получаем путь материала с учетом типа моря
        string seaPath = fishData.SeaType.ToString(); // Получаем название моря
        string path = AssetDatabase.GetAssetPath(fishData.RandomMaterial);

        // Проверяем, какой путь соответствует материалу в данном море
        if (path.Contains($"/Resources/Materials/{seaPath}/Legendary/"))
        {
            return new Color(1f, 1f, 0f);  // Легендарный (желтый)
        }
        else if (path.Contains($"/Resources/Materials/{seaPath}/Mythic/"))
        {
            return new Color(1f, 0.41f, 0.71f);  // Мифический (розовый)
        }
        else if (path.Contains($"/Resources/Materials/{seaPath}/Epic/"))
        {
            return new Color(0.6f, 0.4f, 0.8f);  // Эпический (синий)
        }
        else if (path.Contains($"/Resources/Materials/{seaPath}/Rare/"))
        {
            return new Color(0f, 0f, 1f); // Редкий (синий)
        }
        else if (path.Contains($"/Resources/Materials/{seaPath}/Default/"))
        {
            return new Color(1f, 1f, 1f);  // Обычный (белый)
        }
        else
        {
            Debug.LogWarning($"Unknown material category: {path}");
            return Color.black;  // По умолчанию черный цвет, если категория не найдена
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.Hide();
    }
}
