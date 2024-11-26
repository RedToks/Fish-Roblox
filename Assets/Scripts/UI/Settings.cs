using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class Settings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider sensitivitySlider; // Слайдер для чувствительности
    [SerializeField] private Button resetButton; // Кнопка для сброса
    [SerializeField] private GameObject resetConfirmationPanel; // Панель подтверждения
    [SerializeField] private Button confirmResetButton; // Кнопка подтверждения сброса
    [SerializeField] private Button cancelResetButton; // Кнопка отмены сброса
    [SerializeField] private TMP_Dropdown qualityDropdown;

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
            Debug.LogWarning("No saved settings found. Using default settings.");
            currentSettings = new GameSettings();
        }

        // Логируем загруженные настройки
        Debug.Log($"Loaded Settings: Volume={currentSettings.volume}, Sensitivity={currentSettings.sensitivity}, QualityLevel={currentSettings.qualityLevel}");

        // Применяем настройки громкости
        if (audioMixer != null)
        {
            audioMixer.SetFloat("volume", currentSettings.volume);
            Debug.Log($"Audio volume set to {currentSettings.volume}");
        }

        if (volumeSlider != null)
        {
            volumeSlider.value = currentSettings.volume;
        }

        // Применяем чувствительность
        if (sensitivitySlider != null)
        {
            sensitivitySlider.value = currentSettings.sensitivity;
            sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
        }

        // Применяем уровень качества
        if (qualityDropdown != null)
        {
            Debug.Log($"Applying quality level: {currentSettings.qualityLevel}");
            QualitySettings.SetQualityLevel(currentSettings.qualityLevel); // Применяем уровень качества
            qualityDropdown.value = currentSettings.qualityLevel;         // Обновляем Dropdown
            qualityDropdown.RefreshShownValue();                          // Принудительное обновление отображения
            qualityDropdown.onValueChanged.AddListener(SetQuality);
            Debug.Log($"Dropdown updated to value: {qualityDropdown.value}");
        }
        else
        {
            Debug.LogError("Quality dropdown is not assigned.");
        }

        // Настройка кнопок сброса
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
        if (audioMixer != null)
        {
            audioMixer.SetFloat("volume", volume);
            Debug.Log($"Volume changed to {volume}");
        }

        currentSettings.volume = volume;
        saveLoad.SaveSettings(currentSettings);
    }

    public void SetSensitivity(float sensitivity)
    {
        Debug.Log($"Sensitivity changed to {sensitivity}");
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
        Debug.Log($"Quality level changed to {qualityIndex}");
        QualitySettings.SetQualityLevel(qualityIndex);
        currentSettings.qualityLevel = qualityIndex;
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
        Debug.Log("Resetting settings to default.");
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        resetConfirmationPanel.SetActive(false);
    }
}
