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
        // Найдём дочерний объект, содержащий рамку (предполагаем, что рамка - это первый дочерний объект с компонентом Image)
        Image frameImage = button.transform.GetChild(0).GetComponent<Image>();

        // Устанавливаем изображение на кнопке
        var image = button.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = item.Sprite;
        }

        // Проверка типа предмета
        if (item is FishData fishData)
        {
            // Устанавливаем цвет рамки на основе редкости рыбы
            if (frameImage != null)
            {
                frameImage.color = fishData.RarityColor;
            }

            // Добавляем UI для отображения данных рыбы
            var inventoryItemUI = button.gameObject.GetComponent<InventoryItemUI>();
            if (inventoryItemUI == null)
            {
                inventoryItemUI = button.gameObject.AddComponent<InventoryItemUI>();
            }
            inventoryItemUI.Initialize(item);

            button.onClick.RemoveAllListeners();

            // Если магазин открыт, продаём рыбу
            if (_shop.shopIsOpen)
            {
                button.onClick.AddListener(() => _shop.AttemptToSellItem(item, button));
            }
            else
            {
                // Иначе выбрасываем рыбу
                button.onClick.AddListener(() => _playerInteraction.ThrowFish(fishData));
            }
        }
        else if (item is FishingRodData rodData)
        {
            // Если это удочка, добавляем логику улучшения
            if (frameImage != null)
            {
                frameImage.color = Color.green; // или другой цвет для удочки
            }

            var inventoryItemUI = button.gameObject.GetComponent<InventoryItemUI>();
            if (inventoryItemUI == null)
            {
                inventoryItemUI = button.gameObject.AddComponent<InventoryItemUI>();
            }
            inventoryItemUI.Initialize(item);

            button.onClick.RemoveAllListeners();

            // Если магазин открыт, пытаемся улучшить удочку
            if (_shop.shopIsOpen)
            {
                button.onClick.AddListener(() => _shop.AttemptToUpgradeRod(rodData, button));
            }
            else
            {
                // Логика для действия, если магазин закрыт (например, использование удочки, если это необходимо)
                // button.onClick.AddListener(() => _playerInteraction.UseFishingRod(rodData));
            }
        }
        else
        {
            // Настройки для других типов предметов (не рыба и не удочка)
            if (frameImage != null)
            {
                frameImage.color = Color.white; // или другой цвет по умолчанию
            }

            // Добавить действия для других предметов
            button.onClick.RemoveAllListeners();
        }

        // Активируем кнопку после настройки
        button.gameObject.SetActive(true);
    }


    private void HideInventoryButton(Button button)
    {
        Tooltip.Hide();
        button.gameObject.SetActive(false);
    }
}
