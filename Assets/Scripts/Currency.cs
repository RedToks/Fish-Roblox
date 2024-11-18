using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Currency : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int CurrentCurrency { get; private set; }

    [SerializeField] private TextMeshProUGUI currencyText;

    private const string CurrencyKey = "PlayerCurrency"; // Ключ для валюты

    private void Start()
    {
        // Загружаем валюту при старте
        LoadCurrencyData();
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
            // Сохраняем данные о валюте после траты
            SaveCurrencyData();
            UpdateCurrencyDisplay();
        }
        else
        {
            Debug.Log("Недостаточно валюты для покупки.");
        }
    }

    public void AddCurrency(int amount)
    {
        CurrentCurrency += amount;
        Debug.Log($"Получено {amount} валюты. Текущий баланс: {CurrentCurrency}");

        // Сохраняем данные о валюте после добавления
        SaveCurrencyData();
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

    private void SaveCurrencyData()
    {
        // Сохраняем валюту в PlayerPrefs
        PlayerPrefs.SetInt(CurrencyKey, CurrentCurrency);
        PlayerPrefs.Save();
    }

    private void LoadCurrencyData()
    {
        // Загружаем валюту из PlayerPrefs, если она существует
        if (PlayerPrefs.HasKey(CurrencyKey))
        {
            CurrentCurrency = PlayerPrefs.GetInt(CurrencyKey);
        }
        else
        {
            CurrentCurrency = 0; // Если данных нет, начинаем с 0
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Используем конвертер при отображении тултипа
        string formattedCurrency = CurrencyFormatter.FormatCurrency(CurrentCurrency);
        Tooltip.Show(formattedCurrency); // Показываем отформатированную валюту
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.Hide(); // Скрыть тултип при выходе
    }
}
