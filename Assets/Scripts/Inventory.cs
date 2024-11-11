using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour 
{
    [SerializeField] private List<ItemData> _items;
    private const int _maxSlots = 6;
    public static Inventory instance;

    public event Action OnInventoryChanged; // Событие для обновления UI

    public Inventory()
    {
        if (instance == null)
        {
            instance = this;
        }

        _items = new List<ItemData>(_maxSlots);
    }

    public bool IsFull()
    {
        return _items.Count >= _maxSlots;
    }

    public bool AddItem(ItemData item)
    {
        if (_items.Count >= _maxSlots)
        {
            return false;
        }

        _items.Add(item);

        OnInventoryChanged?.Invoke(); // Вызываем событие
        return true;
    }

    public void RemoveItem(ItemData item)
    {
        if (_items.Contains(item))
        {
            _items.Remove(item);
            OnInventoryChanged?.Invoke(); // Вызываем событие
        }
    }

    public List<ItemData> Items => _items;    
}
