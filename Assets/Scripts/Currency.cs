using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Currency : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int CurrentCurrency { get; private set; }

    [SerializeField] private TextMeshProUGUI currencyText;

    private void Start()
    {
        UpdateCurrencyDisplay();
    }

    public bool CanAfford(int amount)
    {
        return CurrentCurrency >= amount;
    }

    public void SpendCurrency(int amount)
    {
        if (CanAfford(amount))
        {
            CurrentCurrency -= amount;
            UpdateCurrencyDisplay();
        }
        else
        {
            Debug.Log("������������ ������ ��� �������.");
        }
    }

    public void AddCurrency(int amount)
    {
        CurrentCurrency += amount;
        Debug.Log($"�������� {amount} ������. ������� ������: {CurrentCurrency}");

        UpdateCurrencyDisplay();
    }

    private void UpdateCurrencyDisplay()
    {
        if (currencyText != null)
        {
            string formattedCurrency = CurrencyFormatter.FormatCurrency(CurrentCurrency);
            currencyText.text = $"{formattedCurrency}";
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // ���������� ��������� ��� ����������� �������
        string formattedCurrency = CurrencyFormatter.FormatCurrency(CurrentCurrency);
        Tooltip.Show(formattedCurrency); // ���������� ����������������� ������
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.Hide(); // ������ ������ ��� ������
    }
}
