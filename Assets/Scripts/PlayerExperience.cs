using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerExperience : MonoBehaviour
{
    [SerializeField] private int maxLevel = 100;
    [SerializeField] private Slider experienceSlider;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI experienceToNextLevelText;

    public int currentLevel { get; private set; } = 1;
    private int currentExperience = 0;
    private int experienceToNextLevel = 100;

    private const string LevelKey = "PlayerLevel";  //  люч дл€ уровн€ игрока
    private const string ExperienceKey = "PlayerExperience";  //  люч дл€ опыта игрока
    private const string ExperienceToNextLevelKey = "PlayerExperienceToNextLevel";  //  люч дл€ опыта до следующего уровн€

    private void Start()
    {
        // «агружаем данные о уровне и опыте при старте
        LoadPlayerData();
        UpdateUI();
    }

    public void AddExperience(int amount)
    {
        currentExperience += amount;

        // ѕроверка, если опыт больше, чем нужно дл€ следующего уровн€
        while (currentExperience >= experienceToNextLevel && currentLevel < maxLevel)
        {
            currentExperience -= experienceToNextLevel;
            currentLevel++;
            experienceToNextLevel = CalculateExperienceToNextLevel(currentLevel);
        }

        // —охран€ем данные о уровне, опыте и опыте до следующего уровн€
        SavePlayerData();
        UpdateUI();
    }

    private int CalculateExperienceToNextLevel(int level)
    {
        return level * 100; // ѕример: дл€ 1 уровн€ требуетс€ 100 опыта, дл€ 2 уровн€ 200 и так далее.
    }

    private void UpdateUI()
    {
        if (currentLevel >= maxLevel)
        {
            levelText.text = $"уровень {maxLevel}";
            experienceToNextLevelText.text = "¬ы достигли максимального уровн€";
            experienceSlider.value = experienceSlider.maxValue;
        }
        else
        {
            experienceSlider.value = (float)currentExperience / experienceToNextLevel;
            levelText.text = $"уровень {currentLevel}";

            int experienceNeeded = experienceToNextLevel - currentExperience;
            experienceToNextLevelText.text = $"ƒо след. уровн€: {CurrencyFormatter.FormatCurrency(experienceNeeded)} опыта";
        }
    }

    private void SavePlayerData()
    {
        // —охран€ем уровень, опыт и опыт до следующего уровн€ в PlayerPrefs
        PlayerPrefs.SetInt(LevelKey, currentLevel);
        PlayerPrefs.SetInt(ExperienceKey, currentExperience);
        PlayerPrefs.SetInt(ExperienceToNextLevelKey, experienceToNextLevel);
        PlayerPrefs.Save();
    }

    private void LoadPlayerData()
    {
        // «агружаем уровень, опыт и опыт до следующего уровн€ из PlayerPrefs, если они существуют
        if (PlayerPrefs.HasKey(LevelKey) && PlayerPrefs.HasKey(ExperienceKey) && PlayerPrefs.HasKey(ExperienceToNextLevelKey))
        {
            currentLevel = PlayerPrefs.GetInt(LevelKey);
            currentExperience = PlayerPrefs.GetInt(ExperienceKey);
            experienceToNextLevel = PlayerPrefs.GetInt(ExperienceToNextLevelKey);

            // ѕроверка на возможное несоответствие опыта и уровн€
            while (currentExperience >= experienceToNextLevel && currentLevel < maxLevel)
            {
                currentExperience -= experienceToNextLevel;
                currentLevel++;
                experienceToNextLevel = CalculateExperienceToNextLevel(currentLevel);
            }
        }
        else
        {
            // ≈сли данных нет, начинаем с нулевого уровн€ и опыта
            currentLevel = 1;
            currentExperience = 0;
            experienceToNextLevel = CalculateExperienceToNextLevel(currentLevel);
        }
    }
}
