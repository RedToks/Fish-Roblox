using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class Shop : MonoBehaviour
{
    public enum ShopAction
    {
        None,
        PurchaseIsland,
        SellItem
    }

    private ShopAction currentAction = ShopAction.None;

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
    private YandexGame yandexGame;

    // Платформы с телепортами для островов
    [SerializeField] private GameObject iceIslandTeleportPlatform;
    [SerializeField] private GameObject lavaIslandTeleportPlatform;

    // Кнопки для покупки островов
    [SerializeField] private Button iceIslandButton;
    [SerializeField] private Button lavaIslandButton;
    [SerializeField] private TextMeshProUGUI iceIslandPriceText;
    [SerializeField] private TextMeshProUGUI lavaIslandPriceText;

    // Цена островов
    private int iceIslandPrice = 10000;
    private int lavaIslandPrice = 50000;

    // Флаги для отслеживания покупки островов
    private bool iceIslandPurchased = false;
    private bool lavaIslandPurchased = false;

    public event System.Action<ItemData, Button> OnItemSold;

    private void Start()
    {
        yandexGame = FindObjectOfType<YandexGame>();
        inventoryUI = FindObjectOfType<InventoryUI>();
        playerExperience = FindObjectOfType<PlayerExperience>();
        playerInteraction = FindObjectOfType<PlayerInteraction>();

        shopPanel.SetActive(false);
        confirmPanel.SetActive(false);

        yesButton.onClick.AddListener(ConfirmSell);
        noButton.onClick.AddListener(CancelSell);

        // Загружаем состояние покупок и улучшений
        LoadShopState();
        // Обновляем доступность кнопок при старте
        UpdateIslandButtons();

        // Подписываем кнопки островов на попытку покупки
        iceIslandButton.onClick.AddListener(() => AttemptToPurchaseIsland(iceIslandPrice, "Ледяной океан", iceIslandTeleportPlatform));
        lavaIslandButton.onClick.AddListener(() => AttemptToPurchaseIsland(lavaIslandPrice, "Лавовый океан", lavaIslandTeleportPlatform));
    }
    private void SaveShopState()
    {
        PlayerPrefs.SetInt("IceIslandPurchased", iceIslandPurchased ? 1 : 0);
        PlayerPrefs.SetInt("LavaIslandPurchased", lavaIslandPurchased ? 1 : 0);
        PlayerPrefs.Save();
    }

    // Метод для загрузки состояния покупок островов
    private void LoadShopState()
    {
        iceIslandPurchased = PlayerPrefs.GetInt("IceIslandPurchased", 0) == 1;
        lavaIslandPurchased = PlayerPrefs.GetInt("LavaIslandPurchased", 0) == 1;

        if (iceIslandPurchased)
        {
            iceIslandTeleportPlatform.SetActive(true);
        }

        if (lavaIslandPurchased)
        {
            lavaIslandTeleportPlatform.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out ThirdPersonController player))
        {
            shopPanel.SetActive(true);
            shopIsOpen = true;
            inventoryUI.UpdateInventoryUI();
            UpdateIslandButtons();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out ThirdPersonController player))
        {
            confirmPanel.SetActive(false);
            shopPanel.SetActive(false);
            shopIsOpen = false;
            inventoryUI.UpdateInventoryUI();
        }
    }

    // Метод для попытки покупки острова с запросом подтверждения
    public void AttemptToPurchaseIsland(int price, string islandName, GameObject teleportPlatform)
    {
        if (playerCurrency.CanAfford(price))
        {
            // Запрос подтверждения покупки
            confirmText.text = $"Вы уверены, что хотите купить {islandName} за {CurrencyFormatter.FormatCurrency(price)}?";

            // Устанавливаем флаг на покупку острова
            currentAction = ShopAction.PurchaseIsland;

            // Убираем слушатели и добавляем только для покупки острова
            yesButton.onClick.RemoveAllListeners();
            yesButton.onClick.AddListener(() => ConfirmPurchaseIsland(price, islandName, teleportPlatform));
            yesButton.interactable = true;
            UpdateIslandButtons();
            confirmPanel.SetActive(true);
        }
        else
        {
            Debug.Log("Недостаточно валюты для покупки острова.");
        }
    }

    // Метод для подтверждения покупки острова
    private void ConfirmPurchaseIsland(int price, string islandName, GameObject teleportPlatform)
    {
        playerCurrency.SpendCurrency(price);
        teleportPlatform.SetActive(true); // Активируем платформу
        confirmPanel.SetActive(false); // Закрываем панель подтверждения
        Debug.Log($"{islandName} куплен.");

        // Обновляем доступность кнопок сразу после покупки
        UpdateIslandButtons();

        // Устанавливаем флаг, что остров куплен
        if (islandName == "Ледяной океан")
        {
            iceIslandPurchased = true;
        }
        else if (islandName == "Лавовый океан")
        {
            lavaIslandPurchased = true;
        }
        SaveShopState();

        currentAction = ShopAction.None; // Сброс флага после завершения действия
    }

    // Метод для отмены покупки
    private void CancelSell()
    {
        confirmPanel.SetActive(false);
    }

    // Метод для обновления доступности кнопок покупки островов
    private void UpdateIslandButtons()
    {
        // Если остров уже куплен, кнопка будет заблокирована
        iceIslandButton.interactable = !iceIslandPurchased && playerCurrency.CanAfford(iceIslandPrice);
        lavaIslandButton.interactable = !lavaIslandPurchased && playerCurrency.CanAfford(lavaIslandPrice);

        if (iceIslandPurchased)
        {
            iceIslandPriceText.text = "Куплено";
        }
        else
        {
            iceIslandPriceText.text = $"{CurrencyFormatter.FormatCurrency(iceIslandPrice)}";
        }

        // Обновляем текст для лавового острова
        if (lavaIslandPurchased)
        {
            lavaIslandPriceText.text = "Куплено";
        }
        else
        {
            lavaIslandPriceText.text = $"{CurrencyFormatter.FormatCurrency(lavaIslandPrice)}";
        }
    }

    // Остальные методы, как и было
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

            // Запрос подтверждения продажи рыбы
            confirmText.text = $"Вы уверены, что хотите продать {fish.Name} за {CurrencyFormatter.FormatCurrency(fish.Price)}?";

            // Устанавливаем флаг на продажу рыбы
            currentAction = ShopAction.SellItem;

            // Активируем кнопку "Да"
            yesButton.interactable = true;
            yesButton.onClick.RemoveAllListeners();
            yesButton.onClick.AddListener(() => SellItem(fish, button));

            confirmPanel.SetActive(true);
        }
        else
        {
            Debug.Log($"Нельзя продать {item.Name}, так как это не рыба.");
        }
    }

    // Метод для улучшения удочки
    public void AttemptToUpgradeRod(ItemData item, Button button)
    {
        if (!shopIsOpen || !shopPanel.activeSelf)
        {
            Debug.Log("Магазин закрыт или игрок не в радиусе магазина.");
            return;
        }

        if (item is FishingRodData rod)
        {
            FishingRodData upgradedRod = GetUpgradedRod(rod);

            if (upgradedRod == null)
            {
                Debug.LogWarning("Удочка следующего уровня не найдена.");
                confirmText.text = "Удочка следующего уровня не найдена.";
                yesButton.interactable = false;
                confirmPanel.SetActive(true);
                return;
            }

            int requiredLevel = upgradedRod.Level == 2 ? 10 : (upgradedRod.Level == 3 ? 25 : 0);
            string errorMessage = "";

            if (playerExperience.currentLevel < requiredLevel)
            {
                errorMessage += $"Недостаточно уровня для улучшения удочки до {upgradedRod.Level} уровня.\nТребуется уровень: {requiredLevel}, ваш уровень: {playerExperience.currentLevel}.\n\n";  // Отступ после уровня
            }

            if (!playerCurrency.CanAfford(upgradedRod.Price))
            {
                errorMessage += $"Недостаточно средств для улучшения удочки до {upgradedRod.Level} уровня.\nТребуется: {CurrencyFormatter.FormatCurrency(upgradedRod.Price)}, у вас: {CurrencyFormatter.FormatCurrency(playerCurrency.CurrentCurrency)}.";
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                confirmText.text = errorMessage;
                yesButton.interactable = false;
                confirmPanel.SetActive(true);
                return;
            }

            // Если все проверки пройдены, отображаем подтверждение
            confirmText.text = $"Вы уверены, что хотите улучшить свою удочку до уровня {upgradedRod.Level} за {CurrencyFormatter.FormatCurrency(upgradedRod.Price)}?";
            yesButton.interactable = true;
            yesButton.onClick.RemoveAllListeners();
            yesButton.onClick.AddListener(() => UpgradeFishingRod(rod));
            confirmPanel.SetActive(true);
        }
        else
        {
            Debug.Log($"Нельзя улучшить {item.Name}, так как это не удочка.");
        }
    }




    private void UpgradeFishingRod(FishingRodData currentRod)
    {
        FishingRodData upgradedRod = GetUpgradedRod(currentRod);

        if (upgradedRod != null)
        {
            playerCurrency.SpendCurrency(upgradedRod.Price);
            Inventory.instance.ReplaceItemInInventory(upgradedRod);
            inventoryUI.UpdateInventoryUI();

            Debug.Log($"Удочка улучшена до уровня {upgradedRod.Level}");
            confirmPanel.SetActive(false);
            currentAction = ShopAction.None;
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
            if (currentAction == ShopAction.SellItem)
            {
                if (itemToSell is FishingRodData rod)
                {
                    UpgradeFishingRod(rod);
                }
                else
                {
                    SellItem(itemToSell, itemButton);
                }

                // Активируем платформы с телепортами в зависимости от покупки
                if (iceIslandPurchased)
                {
                    iceIslandTeleportPlatform.SetActive(true); // Активируем платформу для ледяного острова
                }
                if (lavaIslandPurchased)
                {
                    lavaIslandTeleportPlatform.SetActive(true); // Активируем платформу для лавового острова
                }

                confirmPanel.SetActive(false);
            }
        }
    }

    private void SellItem(ItemData item, Button button)
    {
        if (item is FishData fish)
        {
            // При продаже рыбы увеличиваем валюту игрока
            playerCurrency.AddCurrency(fish.Price);
            playerExperience.AddExperience(fish.Experience);
            Debug.Log($"Продано {fish.Name} за {fish.Price}");
            Inventory.instance.RemoveItem(fish);
            inventoryUI.UpdateInventoryUI();
            button.gameObject.SetActive(false);
            confirmPanel.SetActive(false);
            currentAction = ShopAction.None; // Сброс флага после завершения действия
            yandexGame._FullscreenShow();
        }
    }
}
