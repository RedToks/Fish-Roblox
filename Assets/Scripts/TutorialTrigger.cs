using UnityEngine;
using UnityEngine.UI;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private Button closeButton;

    private bool hasBeenTriggered = false;

    private void Start()
    {
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
        }
    }

    private void ShowTutorial()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    private void CloseTutorial()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}
