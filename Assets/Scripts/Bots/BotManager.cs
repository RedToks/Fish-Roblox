using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class BotManager : MonoBehaviour
{
    [Header("Bot Settings")]
    [Tooltip("������ �������� �����.")]
    public GameObject[] botPrefabs;

    [Tooltip("���������� �����, ������� ����� �������.")]
    public int numberOfBots = 20;

    [Tooltip("����� ������ ��� �����.")]
    public Transform[] spawnPoints;

    [Tooltip("������ ��������, ������� ����� ���� � ���� � �����.")]
    public GameObject[] handItems;

    [Tooltip("����������� � ������������ ������ ��������.")]
    public Vector2 itemSizeRange = new Vector2(0.5f, 1.5f);

    [Tooltip("����������� �������� Y, ���� �������� ��� ����� �������������� �� �����.")]
    public float fallThreshold = -10f;

    [Tooltip("������ ������ � ������ ����.")]
    public GameObject namePlatePrefab;

    [Tooltip("������ ���� ��� �����.")]
    public List<string> botNames;

    [Tooltip("����������� ���������� ����� ������.")]
    public float minDistanceBetweenBots = 1f;

    private List<GameObject> bots = new List<GameObject>();
    private Camera cameraMain;

    private void Start()
    {
        cameraMain = Camera.main;
        SpawnBots();
    }

    private void Update()
    {
        // �������� �� ������� ����� ���� ��������� ������ � ������������
        foreach (var bot in bots)
        {
            if (bot.transform.position.y < fallThreshold)
            {
                TeleportBotToSpawn(bot);
            }
        }
    }

    private void SpawnBots()
    {
        if (botPrefabs.Length == 0 || spawnPoints.Length == 0)
        {
            Debug.LogError("�� ������ ������� ����� ��� ����� ������!");
            return;
        }

        for (int i = 0; i < numberOfBots; i++)
        {
            // �������� ��������� ������ � ����� ������
            GameObject botPrefab = botPrefabs[Random.Range(0, botPrefabs.Length)];
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // ������ ����
            GameObject bot = Instantiate(botPrefab, spawnPoint.position, Quaternion.identity);

            // �������� ���������� �� ������ ����� � ����������� ���� �����
            CheckAndAdjustBotPosition(bot);

            // ����������� ���
            string botName = i < botNames.Count ? botNames[i] : $"Bot_{i}";
            bot.name = botName;

            // ���� ���� ��������� �������
            if (handItems.Length > 0)
            {
                GiveBotItem(bot);
            }

            // ��������� ������ � ������
            AddNamePlate(bot);

            bots.Add(bot);
        }
    }

    // �������� ���������� � ����������� ����, ���� �����
    private void CheckAndAdjustBotPosition(GameObject bot)
    {
        Vector3 spawnPosition = bot.transform.position;
        Collider[] colliders = Physics.OverlapSphere(spawnPosition, minDistanceBetweenBots);

        while (colliders.Length > 1) // ���� ���� ������ ���� � �������
        {
            // �������� ��� � ��������� �����������
            Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
            bot.transform.position += randomDirection * minDistanceBetweenBots;
            colliders = Physics.OverlapSphere(bot.transform.position, minDistanceBetweenBots); // ��������� �����
        }
    }

    private void GiveBotItem(GameObject bot)
    {
        BotBehavior behavior = bot.GetComponent<BotBehavior>();
        if (behavior != null && behavior.handPoint != null)
        {
            GameObject randomItem = handItems[Random.Range(0, handItems.Length)];
            GameObject item = Instantiate(randomItem, behavior.handPoint.position, behavior.handPoint.rotation, behavior.handPoint);

            // ��������� �������
            float randomScale = Random.Range(itemSizeRange.x, itemSizeRange.y);
            item.transform.localScale = Vector3.one * randomScale;

            item.isStatic = true;
            Debug.Log($"{bot.name} ������� {item.name} � �������� {randomScale}.");
        }
    }

    private void AddNamePlate(GameObject bot)
    {
        if (namePlatePrefab == null)
        {
            Debug.LogError("NamePlatePrefab �� �����!");
            return;
        }

        // ������ ������ � ����������� � ����
        GameObject namePlate = Instantiate(namePlatePrefab, bot.transform);
        namePlate.transform.localPosition = new Vector3(0, 4f, 0); // ������� ��� �����        

        // ������������� �����
        TextMeshPro textMesh = namePlate.GetComponentInChildren<TextMeshPro>();
        if (textMesh != null)
        {
            textMesh.text = bot.name;
        }
        else
        {
            Debug.LogError("TextMesh �� ������ � ������� ������!");
        }

        // ��������� ��������� ��� �������� � ������
        NamePlate namePlateScript = namePlate.AddComponent<NamePlate>();
        namePlateScript.SetTargetCamera(cameraMain);
    }

    private void TeleportBotToSpawn(GameObject bot)
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        bot.transform.position = spawnPoint.position;
        Debug.Log($"{bot.name} �������������� �� ����� ������: {spawnPoint.position}");
    }
}
