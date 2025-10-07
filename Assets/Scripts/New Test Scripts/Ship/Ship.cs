using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Ship
{
    public string name;
    public int length;
    public List<Vector2Int> positions = new();
    public bool placed = false;
    public bool horizontal = false;
}
