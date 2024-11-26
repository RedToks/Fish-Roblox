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

    // ��������� � ����������� ��� ��������
    [SerializeField] private GameObject iceIslandTeleportPlatform;
    [SerializeField] private GameObject lavaIslandTeleportPlatform;

    // ������ ��� ������� ��������
    [SerializeField] private Button iceIslandButton;
    [SerializeField] private Button lavaIslandButton;
    [SerializeField] private TextMeshProUGUI iceIslandPriceText;
    [SerializeField] private TextMeshProUGUI lavaIslandPriceText;

    // ���� ��������
    private int iceIslandPrice = 10000;
    private int lavaIslandPrice = 50000;

    // ����� ��� ������������ ������� ��������
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

        // ��������� ��������� ������� � ���������
        LoadShopState();
        // ��������� ����������� ������ ��� ������
        UpdateIslandButtons();

        // ����������� ������ �������� �� ������� �������
        iceIslandButton.onClick.AddListener(() => AttemptToPurchaseIsland(iceIslandPrice, "������� �����", iceIslandTeleportPlatform));
        lavaIslandButton.onClick.AddListener(() => AttemptToPurchaseIsland(lavaIslandPrice, "������� �����", lavaIslandTeleportPlatform));
    }
    private void SaveShopState()
    {
        PlayerPrefs.SetInt("IceIslandPurchased", iceIslandPurchased ? 1 : 0);
        PlayerPrefs.SetInt("LavaIslandPurchased", lavaIslandPurchased ? 1 : 0);
        PlayerPrefs.Save();
    }

    // ����� ��� �������� ��������� ������� ��������
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

    // ����� ��� ������� ������� ������� � �������� �������������
    public void AttemptToPurchaseIsland(int price, string islandName, GameObject teleportPlatform)
    {
        if (playerCurrency.CanAfford(price))
        {
            // ������ ������������� �������
            confirmText.text = $"�� �������, ��� ������ ������ {islandName} �� {CurrencyFormatter.FormatCurrency(price)}?";

            // ������������� ���� �� ������� �������
            currentAction = ShopAction.PurchaseIsland;

            // ������� ��������� � ��������� ������ ��� ������� �������
            yesButton.onClick.RemoveAllListeners();
            yesButton.onClick.AddListener(() => ConfirmPurchaseIsland(price, islandName, teleportPlatform));
            yesButton.interactable = true;
            UpdateIslandButtons();
            confirmPanel.SetActive(true);
        }
        else
        {
            Debug.Log("������������ ������ ��� ������� �������.");
        }
    }

    // ����� ��� ������������� ������� �������
    private void ConfirmPurchaseIsland(int price, string islandName, GameObject teleportPlatform)
    {
        playerCurrency.SpendCurrency(price);
        teleportPlatform.SetActive(true); // ���������� ���������
        confirmPanel.SetActive(false); // ��������� ������ �������������
        Debug.Log($"{islandName} ������.");

        // ��������� ����������� ������ ����� ����� �������
        UpdateIslandButtons();

        // ������������� ����, ��� ������ ������
        if (islandName == "������� �����")
        {
            iceIslandPurchased = true;
        }
        else if (islandName == "������� �����")
        {
            lavaIslandPurchased = true;
        }
        SaveShopState();

        currentAction = ShopAction.None; // ����� ����� ����� ���������� ��������
    }

    // ����� ��� ������ �������
    private void CancelSell()
    {
        confirmPanel.SetActive(false);
    }

    // ����� ��� ���������� ����������� ������ ������� ��������
    private void UpdateIslandButtons()
    {
        // ���� ������ ��� ������, ������ ����� �������������
        iceIslandButton.interactable = !iceIslandPurchased && playerCurrency.CanAfford(iceIslandPrice);
        lavaIslandButton.interactable = !lavaIslandPurchased && playerCurrency.CanAfford(lavaIslandPrice);

        if (iceIslandPurchased)
        {
            iceIslandPriceText.text = "�������";
        }
        else
        {
            iceIslandPriceText.text = $"{CurrencyFormatter.FormatCurrency(iceIslandPrice)}";
        }

        // ��������� ����� ��� �������� �������
        if (lavaIslandPurchased)
        {
            lavaIslandPriceText.text = "�������";
        }
        else
        {
            lavaIslandPriceText.text = $"{CurrencyFormatter.FormatCurrency(lavaIslandPrice)}";
        }
    }

    // ��������� ������, ��� � ����
    public void AttemptToSellItem(ItemData item, Button button)
    {
        if (!shopIsOpen || !shopPanel.activeSelf)
        {
            Debug.Log("������� ������ ��� ����� �� � ������� ��������.");
            return;
        }

        if (item is FishData fish)
        {
            itemToSell = item;
            itemButton = button;

            // ������ ������������� ������� ����
            confirmText.text = $"�� �������, ��� ������ ������� {fish.Name} �� {CurrencyFormatter.FormatCurrency(fish.Price)}?";

            // ������������� ���� �� ������� ����
            currentAction = ShopAction.SellItem;

            // ���������� ������ "��"
            yesButton.interactable = true;
            yesButton.onClick.RemoveAllListeners();
            yesButton.onClick.AddListener(() => SellItem(fish, button));

            confirmPanel.SetActive(true);
        }
        else
        {
            Debug.Log($"������ ������� {item.Name}, ��� ��� ��� �� ����.");
        }
    }

    // ����� ��� ��������� ������
    public void AttemptToUpgradeRod(ItemData item, Button button)
    {
        if (!shopIsOpen || !shopPanel.activeSelf)
        {
            Debug.Log("������� ������ ��� ����� �� � ������� ��������.");
            return;
        }

        if (item is FishingRodData rod)
        {
            FishingRodData upgradedRod = GetUpgradedRod(rod);

            if (upgradedRod == null)
            {
                Debug.LogWarning("������ ���������� ������ �� �������.");
                confirmText.text = "������ ���������� ������ �� �������.";
                yesButton.interactable = false;
                confirmPanel.SetActive(true);
                return;
            }

            int requiredLevel = upgradedRod.Level == 2 ? 10 : (upgradedRod.Level == 3 ? 25 : 0);
            string errorMessage = "";

            if (playerExperience.currentLevel < requiredLevel)
            {
                errorMessage += $"������������ ������ ��� ��������� ������ �� {upgradedRod.Level} ������.\n��������� �������: {requiredLevel}, ��� �������: {playerExperience.currentLevel}.\n\n";  // ������ ����� ������
            }

            if (!playerCurrency.CanAfford(upgradedRod.Price))
            {
                errorMessage += $"������������ ������� ��� ��������� ������ �� {upgradedRod.Level} ������.\n���������: {CurrencyFormatter.FormatCurrency(upgradedRod.Price)}, � ���: {CurrencyFormatter.FormatCurrency(playerCurrency.CurrentCurrency)}.";
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                confirmText.text = errorMessage;
                yesButton.interactable = false;
                confirmPanel.SetActive(true);
                return;
            }

            // ���� ��� �������� ��������, ���������� �������������
            confirmText.text = $"�� �������, ��� ������ �������� ���� ������ �� ������ {upgradedRod.Level} �� {CurrencyFormatter.FormatCurrency(upgradedRod.Price)}?";
            yesButton.interactable = true;
            yesButton.onClick.RemoveAllListeners();
            yesButton.onClick.AddListener(() => UpgradeFishingRod(rod));
            confirmPanel.SetActive(true);
        }
        else
        {
            Debug.Log($"������ �������� {item.Name}, ��� ��� ��� �� ������.");
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

            Debug.Log($"������ �������� �� ������ {upgradedRod.Level}");
            confirmPanel.SetActive(false);
            currentAction = ShopAction.None;
        }
    }


    public FishingRodData GetUpgradedRod(FishingRodData currentRod)
    {
        // ������ ������ ���������� ������
        int nextLevel = currentRod.Level + 1;

        // �����������, ��� � ��� ���� ������ ��� ������ ������ � ��������
        foreach (var rod in allFishingRods)
        {
            if (rod.Level == nextLevel)
            {
                return rod; // ���������� ������ ������� ������
            }
        }

        return null; // ���� ������ ������� ������ �� �������
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

                // ���������� ��������� � ����������� � ����������� �� �������
                if (iceIslandPurchased)
                {
                    iceIslandTeleportPlatform.SetActive(true); // ���������� ��������� ��� �������� �������
                }
                if (lavaIslandPurchased)
                {
                    lavaIslandTeleportPlatform.SetActive(true); // ���������� ��������� ��� �������� �������
                }

                confirmPanel.SetActive(false);
            }
        }
    }

    private void SellItem(ItemData item, Button button)
    {
        if (item is FishData fish)
        {
            // ��� ������� ���� ����������� ������ ������
            playerCurrency.AddCurrency(fish.Price);
            playerExperience.AddExperience(fish.Experience);
            Debug.Log($"������� {fish.Name} �� {fish.Price}");
            Inventory.instance.RemoveItem(fish);
            inventoryUI.UpdateInventoryUI();
            button.gameObject.SetActive(false);
            confirmPanel.SetActive(false);
            currentAction = ShopAction.None; // ����� ����� ����� ���������� ��������
            yandexGame._FullscreenShow();
        }
    }
}
