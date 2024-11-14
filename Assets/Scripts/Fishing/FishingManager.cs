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


    [Header("UI Elements")]
    [SerializeField] private GameObject fishingStartButton; // ������ �� ������ ������ �������

    private ThirdPersonController playerMovement;
    private InventoryUI inventoryUI;
    private Game game;
    private IUIManager uiManager;
    private SeaType currentSeaType;
    private bool isFishingActive = false;

    private void Start()
    {
        game = FindObjectOfType<Game>();
        inventoryUI = FindObjectOfType<InventoryUI>();
        uiManager = FindObjectOfType<UIManager>();
        playerMovement = FindObjectOfType<ThirdPersonController>();
        fishingStartButton.SetActive(false);
        feedbackText.gameObject.SetActive(false);
    }
    private void Update()
    {
        bool isInAnyFishingArea = false;

        // ��������� ��� ���� ������� � ���������� ������, ���� ����� � ����� �� ���
        foreach (var fishingArea in FindObjectsOfType<FishingAreaTrigger>())
        {
            if (fishingArea.IsInFishingArea) // ���� ����� � ���� �������
            {
                isInAnyFishingArea = true;
                currentSeaType = fishingArea.seaType; // ��������� ��� ���� ��� ������� ����
                break; // ���� ���� �� ���� ���� �������, �� ����� ��������� ���������
            }
        }

        if (!isFishingActive && isInAnyFishingArea)
        {
            fishingStartButton.SetActive(true); // �������� ������, ���� ����� � ����� ���� �������
        }
        else
        {
            fishingStartButton.SetActive(false); // ������ ������, ���� ������� �������� ��� ��� ����
        }
    }

    public void StartFishingProcess()
    {
        if (isFishingActive) return;

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

        FishData caughtFish = game.CreateFishFromSea(currentSeaType);
        if (caughtFish != null)
        {
            // ������� 3D ������ ���� ��� ������
            GameObject fishModel = Instantiate(caughtFish.Prefab, Vector3.zero, Quaternion.identity);
            fishModel.SetActive(false); // ������ ��� ���������

            // ��������� ���� � � ������ � ���������
            Inventory.instance.AddFishItem(caughtFish, fishModel);
            inventoryUI.UpdateInventoryUI();
            Debug.Log($"������� ���� �� {currentSeaType}!");
        }

        playerMovement.velocity = playerMovement.StartVelocity;
        uiManager.ShowUI();
        isFishingActive = false;
        fishingButtonPhase.EndPhase();
        fishingStartButton.SetActive(true);

        StartCoroutine(FishingEndSequence());
    }


    // ������������������ ���������� �������
    private IEnumerator FishingEndSequence()
    {
        yield return new WaitForSeconds(fishingEndDelay);

        fishingStartButton.SetActive(true); // Show button again for next fishing session       
    }
}
