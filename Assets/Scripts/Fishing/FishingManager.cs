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
    [SerializeField] private float fishingStartDelay = 1f; // Задержка перед началом рыбалки
    [SerializeField] private float fishingEndDelay = 1f; // Задержка после завершения рыбалки


    [Header("UI Elements")]
    [SerializeField] private GameObject fishingStartButton; // Ссылка на кнопку начала рыбалки

    private ThirdPersonController playerMovement;
    private InventoryUI inventoryUI;
    private Game game;
    private IUIManager uiManager;
    private SeaType currentSeaType;
    public bool isFishingActive { get; private set; } = false;

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

        // Проверяем все зоны рыбалки и показываем кнопку, если игрок в одной из них
        foreach (var fishingArea in FindObjectsOfType<FishingAreaTrigger>())
        {
            if (fishingArea.IsInFishingArea) // Если игрок в зоне рыбалки
            {
                isInAnyFishingArea = true;
                currentSeaType = fishingArea.seaType; // Обновляем тип моря для текущей зоны
                break; // Если хотя бы одна зона активна, не нужно проверять остальные
            }
        }

        if (!isFishingActive && isInAnyFishingArea)
        {
            fishingStartButton.SetActive(true); // Показать кнопку, если игрок в любой зоне рыбалки
        }
        else
        {
            fishingStartButton.SetActive(false); // Скрыть кнопку, если рыбалка началась или нет зоны
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
        feedbackText.text = "Подготовка к рыбалке...";        
        playerMovement.velocity = 0;

        StartCoroutine(FishingStartSequence());
    }

    // Последовательность начала рыбалки
    private IEnumerator FishingStartSequence()
    {
        yield return new WaitForSeconds(fishingStartDelay);

        feedbackText.text = "Рыбалка началась!";

        // Start fishing phase
        fishingButtonPhase.StartFishing();
    }

    // Метод для завершения рыбалки
    // Метод для завершения рыбалки
    public void EndFishingProcess(bool isFishCaught)
    {
        if (!isFishingActive) return;

        if (isFishCaught)
        {
            // Проверяем, можно ли добавить рыбу в инвентарь
            if (Inventory.instance.IsFull())
            {
                feedbackText.text = "Инвентарь полон, невозможно добавить рыбу!";
                Debug.Log("Инвентарь полон, невозможно добавить рыбу.");
            }
            else
            {
                // Создаем рыбу, если инвентарь не полон
                FishData caughtFish = game.CreateFishFromSea(currentSeaType);
                if (caughtFish != null)
                {
                    GameObject fishModel = Instantiate(caughtFish.Prefab, Vector3.zero, Quaternion.identity);
                    fishModel.SetActive(false);

                    Inventory.instance.AddFishItem(caughtFish, fishModel);
                    inventoryUI.UpdateInventoryUI();

                    feedbackText.text = $"<color=#00FF00>Поймана рыба из {currentSeaType}!</color>";
                }
            }
        }
        else
        {
            feedbackText.text = "Игрок не успел, рыба не поймана.";
            Debug.Log("Игрок не успел, рыба не поймана.");
        }

        playerMovement.velocity = playerMovement.StartVelocity;
        uiManager.ShowUI();
        isFishingActive = false;
        fishingButtonPhase.EndPhase();

        StartCoroutine(FishingEndSequence());
    }




    // Последовательность завершения рыбалки
    private IEnumerator FishingEndSequence()
    {
        yield return new WaitForSeconds(fishingEndDelay);

        fishingStartButton.SetActive(true); // Show button again for next fishing session       
    }
}
