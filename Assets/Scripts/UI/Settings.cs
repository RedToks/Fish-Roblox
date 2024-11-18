using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider sensitivitySlider; // Слайдер для чувствительности
    [SerializeField] private Button resetButton; // Кнопка для сброса

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
            resetButton.onClick.AddListener(ResetSettings);
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

    public void ResetSettings()
    {
        // Удаляем все PlayerPrefs
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        Debug.Log("Настройки сброшены.");
    }
}
