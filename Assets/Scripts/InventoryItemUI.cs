using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ItemData itemData { get; private set; }

    // Цвета для различных уровней редкости
    private Dictionary<string, Color> rarityColors = new Dictionary<string, Color>
    {
        { "обычная", Color.white },
        { "редкая", new Color(0.14f, 0, 0.97f) },
        { "эпическая", new Color(0.75f, 0, 0.97f) }, // Фиолетовый
        { "мифическая", new Color(0.97f, 0, 0) }, // Оранжевый
        { "легендарная", new Color(1f, 0.98f, 0f) }    // Золотой
    };

    public void Initialize(ItemData data)
    {
        itemData = data;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemData is FishData fishData)
        {
            string materialName = fishData.RandomMaterial.name;
            float materialMultiplier = fishData.GetRarityMultiplier(fishData.rarity);

            Tooltip.Show(
                $"Имя: {fishData.Name} (<color=#{ColorUtility.ToHtmlStringRGB(fishData.RarityColor)}>{materialName}</color>)\n" +
                $"размер: {fishData.RandomSize:F2}\n" +
                $"Базовая стоимость: {fishData.BasePrice} + {materialMultiplier:F2}x\n" +
                $"Стоимость: {CurrencyFormatter.FormatCurrency(fishData.Price)}\n" +
                $"Опыт: {fishData.Experience}\n" +
                $"Море: {fishData.SeaType}"
            );
        }
        else if (itemData is FishingRodData fishingRod)
        {
            // Форматируем редкость цветом, а остальной текст оставляем обычным
            string chances =
                $"Шанс на <color=#{ColorUtility.ToHtmlStringRGB(rarityColors["обычная"])}>обычную</color> рыбу: {fishingRod.rodlvlCommon * 100:F1}%\n" +
                $"Шанс на <color=#{ColorUtility.ToHtmlStringRGB(rarityColors["редкая"])}>редкую</color> рыбу: {fishingRod.rodlvlRare * 100:F1}%\n" +
                $"Шанс на <color=#{ColorUtility.ToHtmlStringRGB(rarityColors["эпическая"])}>эпическую</color> рыбу: {fishingRod.rodlvlEpic * 100:F1}%\n" +
                $"Шанс на <color=#{ColorUtility.ToHtmlStringRGB(rarityColors["мифическая"])}>мифическую</color> рыбу: {fishingRod.rodlvlMythic * 100:F1}%\n" +
                $"Шанс на <color=#{ColorUtility.ToHtmlStringRGB(rarityColors["легендарная"])}>легендарную</color> рыбу: {fishingRod.rodlvlLegendary * 100:F1}%";

            Tooltip.Show(
                $"Имя: {fishingRod.Name}\n" +
                $"Уровень: {fishingRod.Level}\n\n" +
                chances
            );
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.Hide();
    }
}
