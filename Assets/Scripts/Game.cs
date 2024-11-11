using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private FishData fishDataTemplate; // Общий шаблон рыбы для всех зон



    // Метод для создания рыбы с материалами определенного моря
    private FishData CreateUniqueFish()
    {

        FishData uniqueFish = new FishData(
            fishDataTemplate.Prefab,
            fishDataTemplate.Name,
            fishDataTemplate.Sprite,
            fishDataTemplate.Experience,
            fishDataTemplate.SeaType
        );

        return uniqueFish;
    }

    public void AddFishToInventory()
    {
        FishData uniqueFish = CreateUniqueFish();
        if (uniqueFish != null)
        {
            Inventory.instance.AddItem(uniqueFish);
            inventoryUI.UpdateInventoryUI(); // Обновляем интерфейс инвентаря
        }
    }
}
