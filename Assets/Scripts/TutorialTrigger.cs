using UnityEngine;
using UnityEngine.UI;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private Button closeButton;

    private bool hasBeenTriggered = false;
    private string tutorialKey; // ���������� ���� ��� ���������� ���������

    private void Start()
    {
        // ������ ���������� ���� ��� ������� �������� �� ������ ��� �����
        tutorialKey = $"{gameObject.name}_TutorialTriggered";

        // ���������, ��� �� �������� ��� �������
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
            // ��������� ���������, ����� �������� ������ �� �����������
            PlayerPrefs.SetInt(tutorialKey, 1);
            PlayerPrefs.Save();
        }
    }

    private void ShowTutorial()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
            Time.timeScale = 0f; // ������������� ����� ��� ������ �� ���������
        }
    }

    private void CloseTutorial()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
            Time.timeScale = 1f; // ������������ �����
        }
    }
}
