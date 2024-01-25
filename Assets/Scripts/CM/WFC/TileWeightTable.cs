using System;
using Random = UnityEngine.Random;

public class TileWeightTable
{
    public TileWeightTable(TileData[] tileData)
    {
        _table = new TileWeight[tileData.Length];
            
        for (int i = 0; i < tileData.Length; i++)
        {
            _table[i] = new TileWeight
            {
                TileData = tileData[i]
            };
        }
            
        BalanceTable();
    }
        
    public TileWeightTable()
    {
    }
        
    public void SetTileData(TileData[] tileData)
    {
        _table = new TileWeight[tileData.Length];
            
        for (int i = 0; i < tileData.Length; i++)
        {
            _table[i] = new TileWeight
            {
                TileData = tileData[i]
            };
        }
            
        BalanceTable();
    }
        
    [Serializable]
    public struct TileWeight
    {
        public TileData TileData;
        public float RawLootChance => TileData.weight;
        public float LootChance;
    }
        
    private TileWeight[] _table;

    private void BalanceTable()
    {
        var totalChance = 0f;
            
        foreach (var tileWeight in _table)
        {
            totalChance += tileWeight.RawLootChance;
        }

        for (var index = 0; index < _table.Length; index++)
        {
            var lootChance = _table[index].RawLootChance;
                
            if (lootChance == 0f)
                continue;
                
            lootChance /= totalChance;
                
            _table[index].LootChance = lootChance;
        }
    }
        
    public TileData Get(float value)
    {
        var choice = -1;
            
        for (int i = 0; i < _table.Length; i++)
        {
            var lootData = _table[i];

            if (value > lootData.LootChance)
                value -= lootData.LootChance;
            else
            {
                choice = i;
                break;
            }
        }

        var tileData = _table[choice].TileData;
        return tileData;
    }

    public TileData GetRandom()
    {
        var r = Random.value;
        return Get(r); 
    }
}