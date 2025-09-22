using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private float cellSize = 1f;

    [Header("Prefabs")]
    [SerializeField] private GameObject cellPrefab;

    [Header("Visuals")]
    [SerializeField] private bool revealShips = true;   // playerGrid: true, aiGrid: false

    private Dictionary<Vector2Int, Cell> cells = new();
    private Dictionary<Vector2Int, CellVisualiser> views = new();

    public void Build()
    {
        Clear();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var pos = new Vector2Int(x, y);
                var cell = new Cell(pos);
                cells[pos] = cell;

                var go = Instantiate(cellPrefab, GridToWorld(pos), Quaternion.identity, transform);
                var view = go.GetComponent<CellVisualiser>();
                view.Init(pos);
                views[pos] = view;
            }
        }
    }

    public void Clear()
    {
        foreach (Transform child in transform) Destroy(child.gameObject);
        cells.Clear();
        views.Clear();
    }

    public Vector3 GridToWorld(Vector2Int pos) => new Vector3(pos.x * cellSize, 0f, pos.y * cellSize);
    public bool TryGetCell(Vector2Int p, out Cell c) => cells.TryGetValue(p, out c);
    public bool TryGetView(Vector2Int p, out CellVisualiser v) => views.TryGetValue(p, out v);
    public IEnumerable<Cell> AllCells() => cells.Values;

    public void UpdateView(Cell cell)
    {
        if (!views.TryGetValue(cell.gridPos, out var v)) return;

        // Kleur afgeleiden
        if (cell.state == CellState.Ship)
        {
            if (revealShips)
            {
                var col = cell.occupyingShip?.Def?.color ?? (Color.gray * 0.6f);
                v.Refresh(cell.state, col);
            }
            else
            {
                v.Refresh(CellState.Empty); // verberg schepen op dit bord
            }
        }
        else
        {
            v.Refresh(cell.state);
        }
    }

    // Preview helpers voor placement
    public void SetPreview(IEnumerable<Vector2Int> cellsToPreview, PreviewKind kind, ShipDefinition def)
    {
        foreach (var p in cellsToPreview)
        {
            if (views.TryGetValue(p, out var v))
            {
                v.SetPreview(kind, def ? def.color : Color.green);
            }
        }
    }

    public void ClearAllPreviews()
    {
        foreach (var v in views.Values) v.ClearPreview();
    }

    // Properties
    public int Width => width;
    public int Height => height;
    public float CellSize => cellSize;
    public bool RevealShips { get => revealShips; set => revealShips = value; }
}
