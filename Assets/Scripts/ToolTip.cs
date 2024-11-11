using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private RectTransform backgroundRect; // Добавляем ссылку на RectTransform фона
    private static Tooltip instance;

    // Определение множителя для масштаба шрифта в зависимости от разрешения
    [SerializeField] private float fontSizeScaleFactor = 0.05f; // Можно подстроить этот множитель по своему усмотрению

    private void Start()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    public static void Show(string message)
    {
        if (instance != null)
        {
            instance.gameObject.SetActive(true);
            instance.tooltipText.text = message;

            // Пересчитать размер шрифта и фона
            instance.AdjustTooltipSize();
        }
    }

    public static void Hide()
    {
        if (instance != null)
        {
            instance.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // Постоянно обновляем позицию подсказки на экране
        transform.position = new Vector2(Screen.width / 2, Screen.height / 2);
    }

    private void AdjustTooltipSize()
    {
        // Подстраиваем размер шрифта в зависимости от ширины экрана
        float screenWidth = Screen.width;
        tooltipText.fontSize = screenWidth * fontSizeScaleFactor;

        // Подстраиваем размер фона, чтобы он соответствовал размеру текста
        Vector2 textSize = tooltipText.GetPreferredValues();
        backgroundRect.sizeDelta = new Vector2(textSize.x + 20, textSize.y + 10); // Добавляем отступы
    }
}
