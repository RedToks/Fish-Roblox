using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class Currency : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private int _currentCurrency;

    [SerializeField] private TextMeshProUGUI currencyText;

    public int CurrentCurrency => _currentCurrency;

    private void Start()
    {
        UpdateCurrencyDisplay();
    }

    public void AddCurrency(int amount)
    {
        _currentCurrency += amount;
        Debug.Log($"Получено {amount} валюты. Текущий баланс: {_currentCurrency}");

        UpdateCurrencyDisplay();
    }

    private void UpdateCurrencyDisplay()
    {
        if (currencyText != null)
        {
            string formattedCurrency = CurrencyFormatter.FormatCurrency(_currentCurrency);
            currencyText.text = $"{formattedCurrency}";
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        Tooltip.Show(_currentCurrency.ToString());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.Hide();
    }
}
