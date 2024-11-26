using UnityEngine;
using UnityEngine.UI;
using TMPro;
using YG;

public class PlayerExperience : MonoBehaviour
{
    [SerializeField] private int maxLevel = 100;
    [SerializeField] private Slider experienceSlider;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI experienceToNextLevelText;

    public int currentLevel { get; private set; } = 1;
    private int currentExperience = 0;
    private int experienceToNextLevel = 100;

    private const string LevelKey = "PlayerLevel";  // Ключ для уровня игрока
    private const string ExperienceKey = "PlayerExperience";  // Ключ для опыта игрока
    private const string ExperienceToNextLevelKey = "PlayerExperienceToNextLevel";  // Ключ для опыта до следующего уровня

    private void Start()
    {
        YandexGame.NewLeaderboardScores("LeaderBoardExperience", currentLevel);
        LoadPlayerData();
        UpdateUI();
    }

    public void AddExperience(int amount)
    {
        currentExperience += amount;

        // Проверка, если опыт больше, чем нужно для следующего уровня
        while (currentExperience >= experienceToNextLevel && currentLevel < maxLevel)
        {
            currentExperience -= experienceToNextLevel;
            currentLevel++;
            experienceToNextLevel = CalculateExperienceToNextLevel(currentLevel);
        }

        // Сохраняем данные о уровне, опыте и опыте до следующего уровня
        SavePlayerData();
        UpdateUI();
    }

    private int CalculateExperienceToNextLevel(int level)
    {
        return level * 100; // Пример: для 1 уровня требуется 100 опыта, для 2 уровня 200 и так далее.
    }

    private void UpdateUI()
    {
        if (currentLevel >= maxLevel)
        {
            levelText.text = $"уровень {maxLevel}";
            experienceToNextLevelText.text = "Вы достигли максимального уровня";
            experienceSlider.value = experienceSlider.maxValue;
            YandexGame.NewLeaderboardScores("LeaderBoardExperience", currentLevel);
        }
        else
        {
            experienceSlider.value = (float)currentExperience / experienceToNextLevel;
            levelText.text = $"уровень {currentLevel}";

            int experienceNeeded = experienceToNextLevel - currentExperience;
            experienceToNextLevelText.text = $"До след. уровня: {CurrencyFormatter.FormatCurrency(experienceNeeded)} опыта";
            YandexGame.NewLeaderboardScores("LeaderBoardExperience", currentLevel);
        }
    }

    private void SavePlayerData()
    {
        // Сохраняем уровень, опыт и опыт до следующего уровня в PlayerPrefs
        PlayerPrefs.SetInt(LevelKey, currentLevel);
        PlayerPrefs.SetInt(ExperienceKey, currentExperience);
        PlayerPrefs.SetInt(ExperienceToNextLevelKey, experienceToNextLevel);
        PlayerPrefs.Save();
    }

    private void LoadPlayerData()
    {
        // Загружаем уровень, опыт и опыт до следующего уровня из PlayerPrefs, если они существуют
        if (PlayerPrefs.HasKey(LevelKey) && PlayerPrefs.HasKey(ExperienceKey) && PlayerPrefs.HasKey(ExperienceToNextLevelKey))
        {
            currentLevel = PlayerPrefs.GetInt(LevelKey);
            currentExperience = PlayerPrefs.GetInt(ExperienceKey);
            experienceToNextLevel = PlayerPrefs.GetInt(ExperienceToNextLevelKey);

            // Проверка на возможное несоответствие опыта и уровня
            while (currentExperience >= experienceToNextLevel && currentLevel < maxLevel)
            {
                currentExperience -= experienceToNextLevel;
                currentLevel++;
                experienceToNextLevel = CalculateExperienceToNextLevel(currentLevel);
            }
        }
        else
        {
            // Если данных нет, начинаем с нулевого уровня и опыта
            currentLevel = 1;
            currentExperience = 0;
            experienceToNextLevel = CalculateExperienceToNextLevel(currentLevel);
        }
    }
}
