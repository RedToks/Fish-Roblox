using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider sensitivitySlider; // Слайдер для чувствительности
    [SerializeField] private Button resetButton; // Кнопка для сброса
    [SerializeField] private GameObject resetConfirmationPanel; // Панель подтверждения
    [SerializeField] private Button confirmResetButton; // Кнопка подтверждения сброса
    [SerializeField] private Button cancelResetButton; // Кнопка отмены сброса

    private GameSettings currentSettings;
    private SaveLoad saveLoad;

    private void Start()
    {
        saveLoad = FindObjectOfType<SaveLoad>();
        if (saveLoad == null)
        {
            Debug.LogError("SaveLoad component not found in the scene.");
            return;
        }

        currentSettings = saveLoad.LoadSettings();
        if (currentSettings == null)
        {
            currentSettings = new GameSettings();
        }

        audioMixer.SetFloat("volume", currentSettings.volume);
        volumeSlider.value = currentSettings.volume;

        sensitivitySlider.value = currentSettings.sensitivity;
        sensitivitySlider.onValueChanged.AddListener(SetSensitivity);

        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ShowResetConfirmation);
        }

        if (confirmResetButton != null)
        {
            confirmResetButton.onClick.AddListener(ResetSettings);
        }

        if (cancelResetButton != null)
        {
            cancelResetButton.onClick.AddListener(HideResetConfirmation);
        }
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
        currentSettings.volume = volume;
        saveLoad.SaveSettings(currentSettings);
    }

    public void SetSensitivity(float sensitivity)
    {
        currentSettings.sensitivity = sensitivity;
        saveLoad.SaveSettings(currentSettings);

        CameraController cameraController = FindObjectOfType<CameraController>();
        if (cameraController != null)
        {
            cameraController.sensitivity = sensitivity;
        }
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        currentSettings.qualityLevel = qualityIndex;
        saveLoad.SaveSettings(currentSettings);
    }

    public void SetResolution(int width, int height)
    {
        Screen.SetResolution(width, height, currentSettings.isFullScreen);
        currentSettings.resolutionWidth = width;
        currentSettings.resolutionHeight = height;
        saveLoad.SaveSettings(currentSettings);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
        currentSettings.isFullScreen = isFullScreen;
        saveLoad.SaveSettings(currentSettings);
    }

    private void ShowResetConfirmation()
    {
        resetConfirmationPanel.SetActive(true);
    }

    private void HideResetConfirmation()
    {
        resetConfirmationPanel.SetActive(false);
    }

    public void ResetSettings()
    {
        // Удаляем все PlayerPrefs
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        resetConfirmationPanel.SetActive(false);
    }
}
