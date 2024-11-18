using UnityEngine;

public class TeleportToSpawn : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("Начальная точка (спавн), куда игрок будет телепортирован.")]
    public Transform spawnPoint; // Ссылка на точку спавна
    private CharacterController cc;
    private GameObject player;

    [Header("Fall Settings")]
    [Tooltip("Минимальное значение Y, ниже которого игрок телепортируется на спавн.")]
    public float fallThreshold = -10f; // Порог, ниже которого игрок телепортируется

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Player object with tag 'Player' not found in the scene!");
            return;
        }

        cc = player.GetComponent<CharacterController>();

        if (spawnPoint == null)
        {
            Debug.LogError("Spawn point not assigned! Please set it in the inspector.");
        }
    }

    private void Update()
    {
        if (player == null || spawnPoint == null) return;

        // Проверяем, упал ли игрок ниже порога
        if (player.transform.position.y < fallThreshold)
        {
            TeleportPlayerToSpawn();
        }
    }

    public void TeleportPlayerToSpawn()
    {
        cc.enabled = false; // Отключаем CharacterController, чтобы не было проблем с телепортацией
        player.transform.position = spawnPoint.position; // Перемещаем игрока на спавн
        Debug.Log("Player teleported to spawn at " + spawnPoint.position);
        cc.enabled = true; // Включаем CharacterController обратно
    }
}
