using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SliderFishing : MonoBehaviour
{
    [Header("Slider Settings")]
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private Image greenZoneImage;
    [SerializeField] private CanvasGroup feedbackCanvasGroup; // Добавляем CanvasGroup для анимации

    [Header("Slider Range")]
    [SerializeField] private float minSliderPos = 0f;
    [SerializeField] private float maxSliderPos = 1f;

    [Header("Slider Control Settings")]
    [SerializeField] private float sliderMoveSpeed = 0.5f;

    [Header("Fishing Progress")]
    [SerializeField] private Slider fishingProgressSlider;
    [SerializeField] private Image fishingProgressFill; // Ссылка на изображение заливки прогресс-бара
    [SerializeField] private float progressDecreaseRate = 0.1f;
    [SerializeField] private float progressIncreaseRate = 0.2f;

    [SerializeField] private float greenZoneMoveSpeed = 0.1f;
    private float greenZonePositionX = 0f;
    private bool isMovingRight = true;

    [SerializeField] private AudioSource phaseMusic;

    private bool isSliderActive = false;
    private int direction = 0; // -1: влево, 1: вправо, 0: остановка
    private RectTransform greenZoneRect;
    private FishingManager fishingManager;

    private void Start()
    {
        fishingManager = FindObjectOfType<FishingManager>();
        slider.gameObject.SetActive(false);
        fishingProgressSlider.gameObject.SetActive(false);
        feedbackCanvasGroup.alpha = 1f; // Начальная прозрачность текста
        fishingProgressSlider.value = 0.5f;
        greenZoneRect = greenZoneImage.GetComponent<RectTransform>();
    }

    public void StartPhase()
    {

        isSliderActive = true;
        slider.gameObject.SetActive(true);
        fishingProgressSlider.gameObject.SetActive(true);
        feedbackText.text = "Удерживайте слайдер в зоне!";
        feedbackCanvasGroup.alpha = 1f;
        fishingProgressSlider.value = 0.5f;

        phaseMusic.Play();
    }

    private void Update()
    {
        if (isSliderActive)
        {
            HandleSliderInput();
            MoveSlider();
            MoveGreenZone();

            UpdateProgressColor();

            if (IsSliderInGreenZone())
            {
                fishingProgressSlider.value += progressIncreaseRate * Time.deltaTime;
                feedbackText.text = "Хорошо! Удерживайте слайдер в зоне!";
            }
            else
            {
                fishingProgressSlider.value -= progressDecreaseRate * Time.deltaTime;
                feedbackText.text = "рыба ускользает";
            }

            if (fishingProgressSlider.value >= 1f)
            {
                feedbackText.text = "Вы поймали рыбу!";
                StartCoroutine(FadeOutText());
                EndFishing();
                fishingManager.EndFishingProcess(true);
                phaseMusic.Stop();
            }
            else if (fishingProgressSlider.value <= 0f)
            {
                feedbackText.text = "рыба ускользнула!";
                StartCoroutine(FadeOutText());
                EndFishing();
                fishingManager.EndFishingProcess(false);
                phaseMusic.Stop();
            }
        }
    }

    private void HandleSliderInput()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            direction = 1;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            direction = -1;
        }
    }

    private void MoveSlider()
    {
        if (direction != 0)
        {
            slider.value += direction * sliderMoveSpeed * Time.deltaTime;

            if (slider.value >= maxSliderPos || slider.value <= minSliderPos)
            {
                direction = 0;
            }
        }
    }

    private void MoveGreenZone()
    {
        float sliderWidth = slider.GetComponent<RectTransform>().rect.width;
        float greenZoneWidth = greenZoneRect.rect.width;
        float maxPosition = (sliderWidth / 2) - (greenZoneWidth / 2);
        float minPosition = -maxPosition;

        greenZonePositionX += (isMovingRight ? 1 : -1) * greenZoneMoveSpeed * Time.deltaTime;

        if (greenZonePositionX >= maxPosition)
        {
            isMovingRight = false;
            greenZonePositionX = maxPosition;
        }
        else if (greenZonePositionX <= minPosition)
        {
            isMovingRight = true;
            greenZonePositionX = minPosition;
        }

        greenZoneRect.localPosition = new Vector3(greenZonePositionX, greenZoneRect.localPosition.y, greenZoneRect.localPosition.z);
    }

    private bool IsSliderInGreenZone()
    {
        Vector2 handlePosition = slider.handleRect.localPosition;
        float greenZoneLeft = greenZoneRect.localPosition.x - greenZoneRect.rect.width / 2f;
        float greenZoneRight = greenZoneRect.localPosition.x + greenZoneRect.rect.width / 2f;
        return handlePosition.x >= greenZoneLeft && handlePosition.x <= greenZoneRight;
    }

    private void UpdateProgressColor()
    {
        Color color = Color.Lerp(Color.red, Color.green, fishingProgressSlider.value);
        fishingProgressFill.color = color;
    }

    private void EndFishing()
    {
        slider.value = 0;
        fishingProgressSlider.value = 0.5f;
        isSliderActive = false;
        slider.gameObject.SetActive(false);
        fishingProgressSlider.gameObject.SetActive(false);
    }

    private IEnumerator FadeOutText()
    {
        yield return new WaitForSeconds(2f);

        float fadeDuration = 1f;
        float startAlpha = feedbackCanvasGroup.alpha;
        float time = 0;

        while (time < fadeDuration)
        {
            feedbackCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }

        feedbackCanvasGroup.alpha = 0;
        feedbackText.gameObject.SetActive(false);
        feedbackCanvasGroup.alpha = 1;
    }
}
