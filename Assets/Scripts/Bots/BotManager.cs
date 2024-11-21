using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class BotManager : MonoBehaviour
{
    [Header("Bot Settings")]
    [Tooltip("Список префабов ботов.")]
    public GameObject[] botPrefabs;

    [Tooltip("Количество ботов, которые будут созданы.")]
    public int numberOfBots = 20;

    [Tooltip("Точки спавна для ботов.")]
    public Transform[] spawnPoints;

    [Tooltip("Список объектов, которые могут быть у бота в руках.")]
    public GameObject[] handItems;

    [Tooltip("Минимальный и максимальный размер предмета.")]
    public Vector2 itemSizeRange = new Vector2(0.5f, 1.5f);

    [Tooltip("Минимальное значение Y, ниже которого бот будет телепортирован на спавн.")]
    public float fallThreshold = -10f;

    [Tooltip("Префаб плашки с именем бота.")]
    public GameObject namePlatePrefab;

    [Tooltip("Список имен для ботов.")]
    public List<string> botNames;

    [Tooltip("Минимальное расстояние между ботами.")]
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
        // Проверка на падение ботов ниже заданного уровня и телепортация
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
            Debug.LogError("Не заданы префабы ботов или точки спавна!");
            return;
        }

        for (int i = 0; i < numberOfBots; i++)
        {
            // Выбираем случайный префаб и точку спавна
            GameObject botPrefab = botPrefabs[Random.Range(0, botPrefabs.Length)];
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Создаём бота
            GameObject bot = Instantiate(botPrefab, spawnPoint.position, Quaternion.identity);

            // Проверка расстояния до других ботов и перемещение если нужно
            CheckAndAdjustBotPosition(bot);

            // Присваиваем имя
            string botName = i < botNames.Count ? botNames[i] : $"Bot_{i}";
            bot.name = botName;

            // Дать боту случайный предмет
            if (handItems.Length > 0)
            {
                GiveBotItem(bot);
            }

            // Добавляем плашку с именем
            AddNamePlate(bot);

            bots.Add(bot);
        }
    }

    // Проверка расстояния и перемещение бота, если нужно
    private void CheckAndAdjustBotPosition(GameObject bot)
    {
        Vector3 spawnPosition = bot.transform.position;
        Collider[] colliders = Physics.OverlapSphere(spawnPosition, minDistanceBetweenBots);

        while (colliders.Length > 1) // Если есть другие боты в радиусе
        {
            // Сдвигаем бот в случайном направлении
            Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
            bot.transform.position += randomDirection * minDistanceBetweenBots;
            colliders = Physics.OverlapSphere(bot.transform.position, minDistanceBetweenBots); // Проверяем снова
        }
    }

    private void GiveBotItem(GameObject bot)
    {
        BotBehavior behavior = bot.GetComponent<BotBehavior>();
        if (behavior != null && behavior.handPoint != null)
        {
            GameObject randomItem = handItems[Random.Range(0, handItems.Length)];
            GameObject item = Instantiate(randomItem, behavior.handPoint.position, behavior.handPoint.rotation, behavior.handPoint);

            // Случайный масштаб
            float randomScale = Random.Range(itemSizeRange.x, itemSizeRange.y);
            item.transform.localScale = Vector3.one * randomScale;

            item.isStatic = true;
            Debug.Log($"{bot.name} получил {item.name} с размером {randomScale}.");
        }
    }

    private void AddNamePlate(GameObject bot)
    {
        if (namePlatePrefab == null)
        {
            Debug.LogError("NamePlatePrefab не задан!");
            return;
        }

        // Создаём плашку и прикрепляем к боту
        GameObject namePlate = Instantiate(namePlatePrefab, bot.transform);
        namePlate.transform.localPosition = new Vector3(0, 4f, 0); // Позиция над ботом        

        // Устанавливаем текст
        TextMeshPro textMesh = namePlate.GetComponentInChildren<TextMeshPro>();
        if (textMesh != null)
        {
            textMesh.text = bot.name;
        }
        else
        {
            Debug.LogError("TextMesh не найден в префабе плашки!");
        }

        // Добавляем компонент для поворота к камере
        NamePlate namePlateScript = namePlate.AddComponent<NamePlate>();
        namePlateScript.SetTargetCamera(cameraMain);
    }

    private void TeleportBotToSpawn(GameObject bot)
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        bot.transform.position = spawnPoint.position;
        Debug.Log($"{bot.name} телепортирован на точку спавна: {spawnPoint.position}");
    }
}
