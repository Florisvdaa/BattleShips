using UnityEngine;

public enum PreviewKind { None, Valid, Invalid }

[RequireComponent(typeof(Collider))]
public class CellVisualiser : MonoBehaviour
{
    public Vector2Int gridPos;
    [SerializeField] private Renderer rend;
    private Color baseColor;
    private PreviewKind preview = PreviewKind.None;

    public void Init(Vector2Int pos)
    {
        gridPos = pos;
        if (!rend) rend = GetComponentInChildren<Renderer>();
        baseColor = Color.cyan * 0.3f;
        SetColor(baseColor);
    }

    public void Refresh(CellState state, Color? shipColor = null)
    {
        switch (state)
        {
            case CellState.Empty: baseColor = Color.cyan * 0.3f; break;
            case CellState.Miss: baseColor = Color.blue * 0.7f; break;
            case CellState.Hit: baseColor = Color.red; break;
            case CellState.Ship: baseColor = (shipColor ?? (Color.gray * 0.6f)); break;
        }
        SetColor(baseColor);
    }

    public void SetPreview(PreviewKind kind, Color shipColor)
    {
        preview = kind;
        var col = baseColor;
        if (kind == PreviewKind.Valid) col = Color.Lerp(shipColor, baseColor, 0.5f);
        if (kind == PreviewKind.Invalid) col = Color.Lerp(Color.red, baseColor, 0.5f);
        SetColor(col);
    }

    public void ClearPreview()
    {
        preview = PreviewKind.None;
        SetColor(baseColor);
    }

    public void SetColor(Color c)
    {
        if (rend && rend.material) rend.material.color = c;
    }

    void OnMouseEnter()
    {
        // lichte hover only als geen preview actief
        if (preview == PreviewKind.None)
            SetColor(Color.Lerp(baseColor, Color.white, 0.2f));
    }

    void OnMouseExit()
    {
        if (preview == PreviewKind.None)
            SetColor(baseColor);
    }
}
