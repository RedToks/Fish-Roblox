using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportToLocations : MonoBehaviour
{
    public Transform targetZone; // Целевая зона для телепортации
    private bool playerInZone = false; // Флаг, находится ли игрок в зоне
    private CharacterController cc;
    private FishingManager fishingManager;

    private void Start()
    {
        cc = FindObjectOfType<CharacterController>();
        fishingManager = FindObjectOfType<FishingManager>();

        if (fishingManager == null)
        {
            Debug.LogError("FishingManager not found in the scene!");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out ThirdPersonController player))
        {
            playerInZone = true; // Игрок вошел в зону
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out ThirdPersonController player))
        {
            playerInZone = false; // Игрок вышел из зоны
        }
    }

    private void Update()
    {
        // Проверяем, что игрок находится в зоне и нажал G, а также не рыбачит
        if (playerInZone && (Input.GetKeyDown(KeyCode.G) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.F))  && CanTeleport())
        {
            TeleportPlayer();
        }
    }

    private bool CanTeleport()
    {
        // Проверяем, что рыбалка не активна
        if (fishingManager != null && fishingManager.isFishingActive)
        {
            Debug.Log("Cannot teleport while fishing is active!");
            return false;
        }
        return true;
    }

    private void TeleportPlayer()
    {
        cc.enabled = false;
        if (targetZone == null)
        {
            Debug.LogWarning("Target zone is not set!");
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = targetZone.position;
            Debug.Log("Player teleported to " + targetZone.position);
        }
        cc.enabled = true;
        playerInZone = false; // Сбрасываем флаг, чтобы нельзя было мгновенно телепортироваться снова
    }
}
