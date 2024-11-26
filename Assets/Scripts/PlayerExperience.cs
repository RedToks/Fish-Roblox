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

    private const string LevelKey = "PlayerLevel";  // ���� ��� ������ ������
    private const string ExperienceKey = "PlayerExperience";  // ���� ��� ����� ������
    private const string ExperienceToNextLevelKey = "PlayerExperienceToNextLevel";  // ���� ��� ����� �� ���������� ������

    private void Start()
    {
        YandexGame.NewLeaderboardScores("LeaderBoardExperience", currentLevel);
        LoadPlayerData();
        UpdateUI();
    }

    public void AddExperience(int amount)
    {
        currentExperience += amount;

        // ��������, ���� ���� ������, ��� ����� ��� ���������� ������
        while (currentExperience >= experienceToNextLevel && currentLevel < maxLevel)
        {
            currentExperience -= experienceToNextLevel;
            currentLevel++;
            experienceToNextLevel = CalculateExperienceToNextLevel(currentLevel);
        }

        // ��������� ������ � ������, ����� � ����� �� ���������� ������
        SavePlayerData();
        UpdateUI();
    }

    private int CalculateExperienceToNextLevel(int level)
    {
        return level * 100; // ������: ��� 1 ������ ��������� 100 �����, ��� 2 ������ 200 � ��� �����.
    }

    private void UpdateUI()
    {
        if (currentLevel >= maxLevel)
        {
            levelText.text = $"������� {maxLevel}";
            experienceToNextLevelText.text = "�� �������� ������������� ������";
            experienceSlider.value = experienceSlider.maxValue;
            YandexGame.NewLeaderboardScores("LeaderBoardExperience", currentLevel);
        }
        else
        {
            experienceSlider.value = (float)currentExperience / experienceToNextLevel;
            levelText.text = $"������� {currentLevel}";

            int experienceNeeded = experienceToNextLevel - currentExperience;
            experienceToNextLevelText.text = $"�� ����. ������: {CurrencyFormatter.FormatCurrency(experienceNeeded)} �����";
            YandexGame.NewLeaderboardScores("LeaderBoardExperience", currentLevel);
        }
    }

    private void SavePlayerData()
    {
        // ��������� �������, ���� � ���� �� ���������� ������ � PlayerPrefs
        PlayerPrefs.SetInt(LevelKey, currentLevel);
        PlayerPrefs.SetInt(ExperienceKey, currentExperience);
        PlayerPrefs.SetInt(ExperienceToNextLevelKey, experienceToNextLevel);
        PlayerPrefs.Save();
    }

    private void LoadPlayerData()
    {
        // ��������� �������, ���� � ���� �� ���������� ������ �� PlayerPrefs, ���� ��� ����������
        if (PlayerPrefs.HasKey(LevelKey) && PlayerPrefs.HasKey(ExperienceKey) && PlayerPrefs.HasKey(ExperienceToNextLevelKey))
        {
            currentLevel = PlayerPrefs.GetInt(LevelKey);
            currentExperience = PlayerPrefs.GetInt(ExperienceKey);
            experienceToNextLevel = PlayerPrefs.GetInt(ExperienceToNextLevelKey);

            // �������� �� ��������� �������������� ����� � ������
            while (currentExperience >= experienceToNextLevel && currentLevel < maxLevel)
            {
                currentExperience -= experienceToNextLevel;
                currentLevel++;
                experienceToNextLevel = CalculateExperienceToNextLevel(currentLevel);
            }
        }
        else
        {
            // ���� ������ ���, �������� � �������� ������ � �����
            currentLevel = 1;
            currentExperience = 0;
            experienceToNextLevel = CalculateExperienceToNextLevel(currentLevel);
        }
    }
}
