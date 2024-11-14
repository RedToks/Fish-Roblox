using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Currency playerCurrency;
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private TextMeshProUGUI confirmText;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    public FishingRodData[] allFishingRods;

    public bool shopIsOpen { get; private set; } = false;
    
    private ItemData itemToSell;
    private Button itemButton;
    private PlayerExperience playerExperience;
    private PlayerInteraction playerInteraction;
    private InventoryUI inventoryUI;

    public event System.Action<ItemData, Button> OnItemSold;

    private void Start()
    {
        inventoryUI = FindObjectOfType<InventoryUI>();
        playerExperience = FindObjectOfType<PlayerExperience>();
        playerInteraction = FindObjectOfType<PlayerInteraction>();

        shopPanel.SetActive(false);
        confirmPanel.SetActive(false);

        yesButton.onClick.AddListener(ConfirmSell);
        noButton.onClick.AddListener(CancelSell);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shopPanel.SetActive(true);
            shopIsOpen = true;
            inventoryUI.UpdateInventoryUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shopPanel.SetActive(false);
            shopIsOpen = false;
            inventoryUI.UpdateInventoryUI();
        }
    }

    public void AttemptToSellItem(ItemData item, Button button)
    {
        if (!shopIsOpen || !shopPanel.activeSelf)
        {
            Debug.Log("Магазин закрыт или игрок не в радиусе магазина.");
            return;
        }

        if (item is FishData fish)
        {
            itemToSell = item;
            itemButton = button;
            confirmText.text = $"Вы уверены, что хотите продать {fish.Name} за {CurrencyFormatter.FormatCurrency(fish.Price)}?";
            confirmPanel.SetActive(true);
        }
        else
        {
            Debug.Log($"Нельзя продать {item.Name}, так как это не рыба.");
        }
    }

    public void AttemptToUpgradeRod(ItemData item, Button button)
    {
        if (!shopIsOpen || !shopPanel.activeSelf)
        {
            Debug.Log("Магазин закрыт или игрок не в радиусе магазина.");
            return;
        }

        // Проверка, является ли предмет удочкой
        if (item is FishingRodData rod)
        {
            itemToSell = item;
            itemButton = button;

            FishingRodData upgradedRod = GetUpgradedRod(rod);
            if (upgradedRod != null)
            {
                // Проверка на достаточность валюты для улучшения
                if (playerCurrency.CanAfford(upgradedRod.Price))
                {
                    confirmText.text = $"Вы уверены, что хотите улучшить свою удочку до уровня {upgradedRod.Level} за {CurrencyFormatter.FormatCurrency(upgradedRod.Price)}?";
                    yesButton.interactable = true;  // Активируем кнопку
                }
                else
                {
                    confirmText.text = $"Недостаточно средств для улучшения удочки до уровня {upgradedRod.Level}.";
                    yesButton.interactable = false;  // Деактивируем кнопку
                }

                confirmPanel.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Удочка следующего уровня не найдена.");
            }
        }
        else
        {
            Debug.Log($"Нельзя улучшить {item.Name}, так как это не удочка.");
        }
    }


    private void UpgradeFishingRod(FishingRodData currentRod)
    {
        // Получаем удочку следующего уровня
        FishingRodData upgradedRod = GetUpgradedRod(currentRod);

        // Если удочка следующего уровня существует
        if (upgradedRod != null)
        {
            // Проверка, есть ли у игрока достаточно средств для улучшения
            if (playerCurrency.CanAfford(upgradedRod.Price)) // Снимаем средства за удочку следующего уровня
            {
                // Спускаем валюту
                playerCurrency.SpendCurrency(upgradedRod.Price);

                // Заменяем старую удочку на новую
                Inventory.instance.ReplaceItemInInventory(upgradedRod);

                // Обновляем UI
                inventoryUI.UpdateInventoryUI();
                Debug.Log($"Удочка улучшена до уровня {upgradedRod.Level}");
            }
            else
            {
                Debug.LogWarning("Недостаточно валюты для улучшения удочки.");
            }
        }
        else
        {
            Debug.LogWarning("Удочка следующего уровня не найдена.");
        }
    }

    public FishingRodData GetUpgradedRod(FishingRodData currentRod)
    {
        // Поищем удочку следующего уровня
        int nextLevel = currentRod.Level + 1;

        // Предположим, что у вас есть массив или список удочек с уровнями
        foreach (var rod in allFishingRods)
        {
            if (rod.Level == nextLevel)
            {
                return rod; // Возвращаем удочку второго уровня
            }
        }

        return null; // Если удочка второго уровня не найдена
    }


    private void ConfirmSell()
    {
        if (itemToSell != null)
        {
            if (itemToSell is FishingRodData rod)
            {
                UpgradeFishingRod(rod);
            }
            else
            {
                SellItem(itemToSell, itemButton);
            }
            confirmPanel.SetActive(false);
        }
    }


    private void CancelSell()
    {
        confirmPanel.SetActive(false);
    }

    public void SellItem(ItemData item, Button button)
    {
        if (item.Price == 0)
        {
            Debug.Log($"Невозможно продать {item.Name}, так как его цена равна 0.");
            return;
        }

        if (item is FishData fish)
        {
            playerCurrency.AddCurrency(fish.Price);
            playerExperience.AddExperience(fish.Experience);
            Debug.Log($"Продано {fish.Name}, добавлено {fish.Price} валюты. Баланс: {playerCurrency.CurrentCurrency}");
            Inventory.instance.RemoveItem(fish);
            button.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log($"Нельзя продать {item.Name}");
        }

        OnItemSold?.Invoke(item, button);
    }
}


