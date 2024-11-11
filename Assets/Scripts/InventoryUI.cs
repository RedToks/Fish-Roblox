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
        var items = Inventory.instance.Items;

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
        var textComponent = button.GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = item.Name;
        }

        var image = button.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = item.Sprite;
        }

        var inventoryItemUI = button.gameObject.GetComponent<InventoryItemUI>();
        if (inventoryItemUI == null)
        {
            inventoryItemUI = button.gameObject.AddComponent<InventoryItemUI>();
        }
        inventoryItemUI.Initialize(item);

        button.onClick.RemoveAllListeners();

        // ���� ����� � ��������, �� ������ ������� �������
        if (_shop.shopIsOpen)
        {
            button.onClick.AddListener(() => _shop.AttemptToSellItem(item, button));
        }
        else
        {
            // ���� �� � ��������, �� ������ ����������� �������
            button.onClick.AddListener(() => _playerInteraction.ThrowFish(item as FishData));
        }

        button.gameObject.SetActive(true);
        Tooltip.Hide();
    }





    private void HideInventoryButton(Button button)
    {
        button.gameObject.SetActive(false);
    }
}
