using UnityEngine;

public class TeleportToSpawn : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("��������� ����� (�����), ���� ����� ����� ��������������.")]
    public Transform spawnPoint; // ������ �� ����� ������
    private CharacterController cc;
    private GameObject player;

    [Header("Fall Settings")]
    [Tooltip("����������� �������� Y, ���� �������� ����� ��������������� �� �����.")]
    public float fallThreshold = -10f; // �����, ���� �������� ����� ���������������

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

        // ���������, ���� �� ����� ���� ������
        if (player.transform.position.y < fallThreshold)
        {
            TeleportPlayerToSpawn();
        }
    }

    public void TeleportPlayerToSpawn()
    {
        cc.enabled = false; // ��������� CharacterController, ����� �� ���� ������� � �������������
        player.transform.position = spawnPoint.position; // ���������� ������ �� �����
        Debug.Log("Player teleported to spawn at " + spawnPoint.position);
        cc.enabled = true; // �������� CharacterController �������
    }
}
