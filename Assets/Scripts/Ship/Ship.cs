using System.Collections.Generic;
using UnityEngine;

public class Ship
{
    public ShipDefinition Def { get; private set; }
    public List<Vector2Int> Occupied { get; private set; } = new();
    private HashSet<Vector2Int> hits = new();


    public Ship(ShipDefinition def)
    {
        Def = def;
    }

    public void SetPositions(List<Vector2Int> cells)
    {
        Occupied = cells;
    }


    public void RegisterHit(Vector2Int pos)
    {
        hits.Add(pos);
    }


    public bool IsSunk => hits.Count >= Occupied.Count;
}
