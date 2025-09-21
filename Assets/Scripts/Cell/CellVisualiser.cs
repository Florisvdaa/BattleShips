using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CellVisualiser : MonoBehaviour
{
    public Vector2Int gridPos;
    public Renderer rend;
    private Color baseColor;

    public void Init(Vector2Int pos)
    {
        gridPos = pos;
        if (!rend) rend = GetComponentInChildren<Renderer>();
        baseColor = Color.cyan * 0.3f;
        SetColor(baseColor);
    }

    public void SetColor(Color c)
    {
        if (rend && rend.material) rend.material.color = c;
    }
    void OnMouseEnter()
    {
        SetColor(Color.yellow); // hover highlight
    }

    void OnMouseExit()
    {
        SetColor(baseColor);
    }
}
