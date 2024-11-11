using System.Collections;
using TMPro;
using UnityEngine;

public class FishingManager : MonoBehaviour
{
    [Header("Fishing Systems")]
    [SerializeField] private SliderFishing sliderFishing;
    [SerializeField] private FishingButtonPhase fishingButtonPhase;
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Fishing Settings")]
    [SerializeField] private float fishingStartDelay = 1f; // �������� ����� ������� �������
    [SerializeField] private float fishingEndDelay = 1f; // �������� ����� ���������� �������

    [Header("Fishing Area")]
    [SerializeField] private FishingAreaTrigger fishingAreaTrigger; // ������ �� FishingAreaTrigger

    [Header("UI Elements")]
    [SerializeField] private GameObject fishingStartButton; // ������ �� ������ ������ �������

    private ThirdPersonController playerMovement;
    private IUIManager uiManager;
    private bool isFishingActive = false;

    private void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        playerMovement = FindObjectOfType<ThirdPersonController>();
        fishingStartButton.SetActive(false);
        feedbackText.gameObject.SetActive(false);
    }
    private void Update()
    {
        // Only show the fishing start button when the player is in the fishing area and is not fishing
        if (!isFishingActive && fishingAreaTrigger.IsInFishingArea)
        {
            fishingStartButton.SetActive(true); // Show button when in fishing area
        }
        else
        {
            fishingStartButton.SetActive(false); // Hide button when the player starts fishing
        }
    }
    public void StartFishingProcess()
    {
        if (isFishingActive) return;

        if (!fishingAreaTrigger.IsInFishingArea)
        {
            feedbackText.text = "����� ������� � ������� �������!";
            return;
        }

        isFishingActive = true;
        uiManager.HideUI();
        fishingStartButton.SetActive(false); // Hide the start button when fishing begins

        // Hide UI before starting fishing
        feedbackText.gameObject.SetActive(true);
        feedbackText.text = "���������� � �������...";        
        playerMovement.velocity = 0;

        StartCoroutine(FishingStartSequence());
    }

    // ������������������ ������ �������
    private IEnumerator FishingStartSequence()
    {
        yield return new WaitForSeconds(fishingStartDelay);

        feedbackText.text = "������� ��������!";

        // Start fishing phase
        fishingButtonPhase.StartFishing();
    }

    // ����� ��� ���������� �������
    public void EndFishingProcess()
    {
        if (!isFishingActive) return;

        playerMovement.velocity = playerMovement.StartVelocity;
        uiManager.ShowUI();
        isFishingActive = false;
        fishingButtonPhase.EndPhase();

        // �������� ��������� UI ����� �������
        fishingStartButton.SetActive(true); // �������� ������ ������

        StartCoroutine(FishingEndSequence());
    }

    // ������������������ ���������� �������
    private IEnumerator FishingEndSequence()
    {
        yield return new WaitForSeconds(fishingEndDelay);

        fishingStartButton.SetActive(true); // Show button again for next fishing session       
    }
}
