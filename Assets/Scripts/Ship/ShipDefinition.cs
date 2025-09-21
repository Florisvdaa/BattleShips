using UnityEngine;


[CreateAssetMenu(menuName = "Battleship/Ship Definition")]
public class ShipDefinition : ScriptableObject
{
    public string displayName;
    [Min(2)] public int length = 2;
}