using UnityEngine;

[CreateAssetMenu(menuName = "Battleship/Ship Definition")]
public class ShipDefinition : ScriptableObject
{
    public string displayName;
    [Min(2)] public int length = 2;

    [Header("Visuals")]
    public Color color = Color.gray;

    [Tooltip("Optioneel: ghost prefab met ShipGhost component voor tijdens placement.")]
    public GameObject ghostPrefab;

    [Tooltip("Optioneel: uiteindelijke ship visual prefab na plaatsen.")]
    public GameObject shipVisualPrefab;
}
