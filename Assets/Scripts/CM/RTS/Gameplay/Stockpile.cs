using System.Collections.Generic;
using CM;
using CM.Units;
using TMPro;
using UnityEngine;
using Entity = CM.Entity;

public class Stockpile : EntityBehavior, IInteractable
{
    public static Dictionary<ResourceSO, Stockpile> stockpiles = new();
    public ResourceSO resource;
    public TextMeshPro _textMeshPro;

    public int capacity = 10;
    private int _currentAmount;

    private void OnEnable()
    {
        stockpiles.TryAdd(resource, this);
    }

    private void OnDisable()
    {
        stockpiles.Remove(resource);
    }

    public void Deposit(int amount)
    {
        _currentAmount += amount;
        
        if (_currentAmount > capacity)
        {
            _currentAmount = capacity;
            Debug.Log("Stockpile full");
        }

        _textMeshPro.text = $"Stockpile {resource.name} ({_currentAmount}/{capacity})";
    }

    public void Withdraw(int amount)
    {
        var diff = _currentAmount - amount;
        
        if (diff > 0)
            _currentAmount -= amount;
        else
            _currentAmount -= diff;
        
        _textMeshPro.text = $"Stockpile {resource.name} ({_currentAmount}/{capacity})";
    }

    public void Interact(UnitController unitController)
    {
        unitController._inventory.DepositResources(this);
    }
}