using System.Collections;
using TMPro;
using UnityEngine;

public class FishingButtonPhase : MonoBehaviour
{
    [Header("Fishing UI")]
    [SerializeField] private GameObject catchButton;
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Fishing Settings")]
    [SerializeField] private float biteDelayMin = 2f;
    [SerializeField] private float biteDelayMax = 5f;
    [SerializeField] private float catchButtonTimeLimit = 2f;
    [SerializeField] private int minCatchAttempts = 5;
    [SerializeField] private int maxCatchAttempts = 10;
    [SerializeField] private float newButtonDelay = 1;

    [SerializeField] private AudioSource buttonClickSound;

    private int currentClicks;
    private int targetClicks;
    private bool isFishing;
    private Coroutine catchButtonTimer;

    private SliderFishing sliderFishingScript;
    private FishingManager fishingManager;

    private void Start()
    {
        fishingManager = FindObjectOfType<FishingManager>();
        sliderFishingScript = FindObjectOfType<SliderFishing>();
    }

    public void StartFishing()
    {
        // Проверяем, в какой зоне рыбалки находится игрок
        if (isFishing || !IsPlayerInAnyFishingArea()) return;

        isFishing = true;
        feedbackText.gameObject.SetActive(true);
        feedbackText.text = "Заброс удочки...";

        targetClicks = Random.Range(minCatchAttempts, maxCatchAttempts + 1);
        StartCoroutine(FishingRoutine());
    }

    private bool IsPlayerInAnyFishingArea()
    {
        foreach (var fishingArea in FindObjectsOfType<FishingAreaTrigger>())
        {
            if (fishingArea.IsInFishingArea)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator FishingRoutine()
    {
        float delay = Random.Range(biteDelayMin, biteDelayMax);
        yield return new WaitForSeconds(delay);

        feedbackText.text = "рыба клюнула! Быстро нажмите на кнопку!";
        ShowCatchButton();
    }

    private void ShowCatchButton()
    {
        catchButton.SetActive(true);
        Vector2 randomPosition = GetRandomPosition();
        catchButton.GetComponent<RectTransform>().anchoredPosition = randomPosition;

        if (catchButtonTimer != null)
        {
            StopCoroutine(catchButtonTimer);
        }

        catchButtonTimer = StartCoroutine(CatchButtonCountdown());
    }

    private Vector2 GetRandomPosition()
    {
        RectTransform canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        float xPos = Random.Range(-canvasRect.rect.width / 2, canvasRect.rect.width / 2);
        float yPos = Random.Range(-canvasRect.rect.height / 2, canvasRect.rect.height / 2);
        return new Vector2(xPos, yPos);
    }

    private IEnumerator CatchButtonCountdown()
    {
        yield return new WaitForSeconds(catchButtonTimeLimit);

        feedbackText.text = "Вы не успели! рыбалка окончена.";
        catchButton.SetActive(false);
        EndPhase();
        fishingManager.EndFishingProcess(false);
        yield return new WaitForSeconds(2f);

        feedbackText.gameObject.SetActive(false);
    }

    public void OnCatchButtonClick()
    {
        buttonClickSound.Play();

        currentClicks++;

        if (catchButtonTimer != null)
        {
            StopCoroutine(catchButtonTimer);
            catchButtonTimer = null;
        }

        if (currentClicks >= targetClicks)
        {
            feedbackText.text = "Переходим к следующей фазе";
            EndPhase();
            sliderFishingScript.StartPhase();
        }
        else
        {
            feedbackText.text = $"Отлично! Ожидайте следующую кнопку ({currentClicks}/{targetClicks})";
            catchButton.SetActive(false);
            StartCoroutine(WaitAndShowNextCatchButton());
        }
    }

    private IEnumerator WaitAndShowNextCatchButton()
    {
        yield return new WaitForSeconds(newButtonDelay);
        ShowCatchButton();
    }

    public void EndPhase()
    {
        catchButton.SetActive(false);
        currentClicks = 0;
        targetClicks = 0;
        isFishing = false;
    }
}
