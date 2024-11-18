using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ItemData itemData { get; private set; }

    // ����� ��� ��������� ������� ��������
    private Dictionary<string, Color> rarityColors = new Dictionary<string, Color>
    {
        { "�������", Color.white },
        { "������", new Color(0.14f, 0, 0.97f) },
        { "���������", new Color(0.75f, 0, 0.97f) }, // ����������
        { "����������", new Color(0.97f, 0, 0) }, // ���������
        { "�����������", new Color(1f, 0.98f, 0f) }    // �������
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
                $"���: {fishData.Name} (<color=#{ColorUtility.ToHtmlStringRGB(fishData.RarityColor)}>{materialName}</color>)\n" +
                $"������: {fishData.RandomSize:F2}\n" +
                $"������� ���������: {fishData.BasePrice} + {materialMultiplier:F2}x\n" +
                $"���������: {CurrencyFormatter.FormatCurrency(fishData.Price)}\n" +
                $"����: {fishData.Experience}\n" +
                $"����: {fishData.SeaType}"
            );
        }
        else if (itemData is FishingRodData fishingRod)
        {
            // ����������� �������� ������, � ��������� ����� ��������� �������
            string chances =
                $"���� �� <color=#{ColorUtility.ToHtmlStringRGB(rarityColors["�������"])}>�������</color> ����: {fishingRod.rodlvlCommon * 100:F1}%\n" +
                $"���� �� <color=#{ColorUtility.ToHtmlStringRGB(rarityColors["������"])}>������</color> ����: {fishingRod.rodlvlRare * 100:F1}%\n" +
                $"���� �� <color=#{ColorUtility.ToHtmlStringRGB(rarityColors["���������"])}>���������</color> ����: {fishingRod.rodlvlEpic * 100:F1}%\n" +
                $"���� �� <color=#{ColorUtility.ToHtmlStringRGB(rarityColors["����������"])}>����������</color> ����: {fishingRod.rodlvlMythic * 100:F1}%\n" +
                $"���� �� <color=#{ColorUtility.ToHtmlStringRGB(rarityColors["�����������"])}>�����������</color> ����: {fishingRod.rodlvlLegendary * 100:F1}%";

            Tooltip.Show(
                $"���: {fishingRod.Name}\n" +
                $"�������: {fishingRod.Level}\n\n" +
                chances
            );
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.Hide();
    }
}
