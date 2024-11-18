using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Tooltip tooltip;
    [SerializeField] private Button[] inventoryButtons;
    [SerializeField] private Shop _shop;
    [SerializeField] private PlayerInteraction _playerInteraction;


    private void Start()
    {
        UpdateInventoryUI();
    }

    public void UpdateInventoryUI()
    {
        var items = Inventory.instance.items;

        for (int i = 0; i < inventoryButtons.Length; i++)
        {
            if (i < items.Count)
            {
                SetUpInventoryButton(inventoryButtons[i], items[i]);
            }
            else
            {
                HideInventoryButton(inventoryButtons[i]);
            }
        }
    }

    private void SetUpInventoryButton(Button button, ItemData item)
    {
        // ����� �������� ������, ���������� ����� (������������, ��� ����� - ��� ������ �������� ������ � ����������� Image)
        Image frameImage = button.transform.GetChild(0).GetComponent<Image>();

        // ������������� ����������� �� ������
        var image = button.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = item.Sprite;
        }

        // �������� ���� ��������
        if (item is FishData fishData)
        {
            // ������������� ���� ����� �� ������ �������� ����
            if (frameImage != null)
            {
                frameImage.color = fishData.RarityColor;
            }

            // ��������� UI ��� ����������� ������ ����
            var inventoryItemUI = button.gameObject.GetComponent<InventoryItemUI>();
            if (inventoryItemUI == null)
            {
                inventoryItemUI = button.gameObject.AddComponent<InventoryItemUI>();
            }
            inventoryItemUI.Initialize(item);

            button.onClick.RemoveAllListeners();

            // ���� ������� ������, ������ ����
            if (_shop.shopIsOpen)
            {
                button.onClick.AddListener(() => _shop.AttemptToSellItem(item, button));
            }
            else
            {
                // ����� ����������� ����
                button.onClick.AddListener(() => _playerInteraction.ThrowFish(fishData));
            }
        }
        else if (item is FishingRodData rodData)
        {
            // ���� ��� ������, ��������� ������ ���������
            if (frameImage != null)
            {
                frameImage.color = Color.green; // ��� ������ ���� ��� ������
            }

            var inventoryItemUI = button.gameObject.GetComponent<InventoryItemUI>();
            if (inventoryItemUI == null)
            {
                inventoryItemUI = button.gameObject.AddComponent<InventoryItemUI>();
            }
            inventoryItemUI.Initialize(item);

            button.onClick.RemoveAllListeners();

            // ���� ������� ������, �������� �������� ������
            if (_shop.shopIsOpen)
            {
                button.onClick.AddListener(() => _shop.AttemptToUpgradeRod(rodData, button));
            }
            else
            {
                // ������ ��� ��������, ���� ������� ������ (��������, ������������� ������, ���� ��� ����������)
                // button.onClick.AddListener(() => _playerInteraction.UseFishingRod(rodData));
            }
        }
        else
        {
            // ��������� ��� ������ ����� ��������� (�� ���� � �� ������)
            if (frameImage != null)
            {
                frameImage.color = Color.white; // ��� ������ ���� �� ���������
            }

            // �������� �������� ��� ������ ���������
            button.onClick.RemoveAllListeners();
        }

        // ���������� ������ ����� ���������
        button.gameObject.SetActive(true);
    }


    private void HideInventoryButton(Button button)
    {
        Tooltip.Hide();
        button.gameObject.SetActive(false);
    }
}
