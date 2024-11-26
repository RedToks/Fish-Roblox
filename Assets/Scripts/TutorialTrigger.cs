using UnityEngine;
using UnityEngine.UI;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private Button closeButton;

    private bool hasBeenTriggered = false;
    private string tutorialKey; // Уникальный ключ для сохранения состояния

    private void Start()
    {
        // Создаём уникальный ключ для каждого триггера на основе его имени
        tutorialKey = $"{gameObject.name}_TutorialTriggered";

        // Проверяем, был ли туториал уже показан
        hasBeenTriggered = PlayerPrefs.GetInt(tutorialKey, 0) == 1;

        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseTutorial);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasBeenTriggered) return;

        if (other.TryGetComponent(out PlayerInteraction player))
        {
            ShowTutorial();
            hasBeenTriggered = true;
            // Сохраняем состояние, чтобы туториал больше не показывался
            PlayerPrefs.SetInt(tutorialKey, 1);
            PlayerPrefs.Save();
        }
    }

    private void ShowTutorial()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
            Time.timeScale = 0f; // Останавливаем время для фокуса на туториале
        }
    }

    private void CloseTutorial()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
            Time.timeScale = 1f; // Возобновляем время
        }
    }
}
