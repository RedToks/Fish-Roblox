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
                $"���: {fishData.Name} (<color=#{ColorUtility.ToHtmlStringRGB(GetMaterialColor(fishData))}>{materialName}</color>)\n" +
                $"������: {fishData.RandomSize:F2}\n" +
                $"������� ���������: {FishData.BasePrice} + {materialMultiplier:F2}x\n" +
                $"���������: {CurrencyFormatter.FormatCurrency(fishData.Price)}\n" +
                $"����: {fishData.Experience}\n" +
                $"����: {fishData.SeaType}"
            );
        }
        else
        {
            Tooltip.Show($"���: {itemData.Name}");
        }
    }

    // ������� ��� ��������� ����� ��������� �� ������ ��� ��������� � �������������� RGB
    public Color GetMaterialColor(FishData fishData)
    {
        if (fishData?.RandomMaterial == null)
        {
            Debug.LogError("Material is null in GetMaterialColor.");
            return Color.black;
        }

        // �������� ���� ��������� � ������ ���� ����
        string seaPath = fishData.SeaType.ToString(); // �������� �������� ����
        string path = AssetDatabase.GetAssetPath(fishData.RandomMaterial);

        // ���������, ����� ���� ������������� ��������� � ������ ����
        if (path.Contains($"/Resources/Materials/{seaPath}/Legendary/"))
        {
            return new Color(1f, 1f, 0f);  // ����������� (������)
        }
        else if (path.Contains($"/Resources/Materials/{seaPath}/Mythic/"))
        {
            return new Color(1f, 0.41f, 0.71f);  // ���������� (�������)
        }
        else if (path.Contains($"/Resources/Materials/{seaPath}/Epic/"))
        {
            return new Color(0.6f, 0.4f, 0.8f);  // ��������� (�����)
        }
        else if (path.Contains($"/Resources/Materials/{seaPath}/Rare/"))
        {
            return new Color(0f, 0f, 1f); // ������ (�����)
        }
        else if (path.Contains($"/Resources/Materials/{seaPath}/Default/"))
        {
            return new Color(1f, 1f, 1f);  // ������� (�����)
        }
        else
        {
            Debug.LogWarning($"Unknown material category: {path}");
            return Color.black;  // �� ��������� ������ ����, ���� ��������� �� �������
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.Hide();
    }
}
