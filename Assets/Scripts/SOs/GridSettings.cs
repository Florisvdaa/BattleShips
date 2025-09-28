using UnityEngine;

[CreateAssetMenu(fileName = "GridSettings", menuName = "Game/Grid Settings")]
public class GridSettings : ScriptableObject
{
    public int width = 10;
    public int height = 10;
    public Vector2Int origin = Vector2Int.zero;
}
