using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerExperience : MonoBehaviour
{
    [SerializeField] private int maxLevel = 100;
    [SerializeField] private Slider experienceSlider;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI experienceToNextLevelText;

    private int currentLevel = 1;
    private int currentExperience = 0;
    private int experienceToNextLevel = 100;

    private void Start()
    {
        UpdateUI();
    }

    public void AddExperience(int amount)
    {
        currentExperience += amount;

        while (currentExperience >= experienceToNextLevel && currentLevel < maxLevel)
        {
            currentExperience -= experienceToNextLevel;
            currentLevel++;
            experienceToNextLevel = CalculateExperienceToNextLevel(currentLevel);
        }

        UpdateUI();
    }

    private int CalculateExperienceToNextLevel(int level)
    {
        return level * 1000;
    }

    private void UpdateUI()
    {
        if (currentLevel >= maxLevel)
        {
            levelText.text = $"Уровень {maxLevel}";
            experienceToNextLevelText.text = "Вы достигли максимального уровня";
            experienceSlider.value = experienceSlider.maxValue;
        }
        else
        {
            experienceSlider.value = (float)currentExperience / experienceToNextLevel;
            levelText.text = $"Уровень {currentLevel}";

            int experienceNeeded = experienceToNextLevel - currentExperience;
            experienceToNextLevelText.text = $"До следующего уровня: {CurrencyFormatter.FormatCurrency(experienceNeeded)} опыта";
        }
    }
}
