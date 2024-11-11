using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private RectTransform backgroundRect; // ��������� ������ �� RectTransform ����
    private static Tooltip instance;

    // ����������� ��������� ��� �������� ������ � ����������� �� ����������
    [SerializeField] private float fontSizeScaleFactor = 0.05f; // ����� ���������� ���� ��������� �� ������ ����������

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

            // ����������� ������ ������ � ����
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
        // ��������� ��������� ������� ��������� �� ������
        transform.position = new Vector2(Screen.width / 2, Screen.height / 2);
    }

    private void AdjustTooltipSize()
    {
        // ������������ ������ ������ � ����������� �� ������ ������
        float screenWidth = Screen.width;
        tooltipText.fontSize = screenWidth * fontSizeScaleFactor;

        // ������������ ������ ����, ����� �� �������������� ������� ������
        Vector2 textSize = tooltipText.GetPreferredValues();
        backgroundRect.sizeDelta = new Vector2(textSize.x + 20, textSize.y + 10); // ��������� �������
    }
}
