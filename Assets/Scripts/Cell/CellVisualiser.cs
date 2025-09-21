using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CellVisualiser : MonoBehaviour
{
    public Vector2Int gridPos;
    public Renderer rend;

    public void Init(Vector2Int pos)
    {
        gridPos = pos;
        if (!rend) rend = GetComponentInChildren<Renderer>();
        SetColor(Color.cyan * 0.3f); // default water
    }

    public void SetColor(Color c)
    {
        if (rend && rend.material) rend.material.color = c;
    }
}
