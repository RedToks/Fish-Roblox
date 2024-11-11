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
            Debug.Log("������� ������ ��� ����� �� � ������� ��������.");
            return;
        }

        if (item is FishData fish)
        {
            itemToSell = item;
            itemButton = button;
            confirmText.text = $"�� �������, ��� ������ ������� {fish.Name} �� {CurrencyFormatter.FormatCurrency(fish.Price)}?";
            confirmPanel.SetActive(true);
        }
        else
        {
            Debug.Log($"������ ������� {item.Name}, ��� ��� ��� �� ����.");
        }
    }

    private void ConfirmSell()
    {
        if (itemToSell != null)
        {
            SellItem(itemToSell, itemButton);
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
            Debug.Log($"���������� ������� {item.Name}, ��� ��� ��� ���� ����� 0.");
            return;
        }

        if (item is FishData fish)
        {
            playerCurrency.AddCurrency(fish.Price);
            playerExperience.AddExperience(fish.Experience);
            Debug.Log($"������� {fish.Name}, ��������� {fish.Price} ������. ������: {playerCurrency.CurrentCurrency}");
            Inventory.instance.RemoveItem(fish);
            button.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log($"������ ������� {item.Name}");
        }

        OnItemSold?.Invoke(item, button);
    }
}


