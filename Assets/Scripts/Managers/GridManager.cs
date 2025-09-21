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

    private Dictionary<Vector2Int, Cell> cells = new();
    private Dictionary<Vector2Int, CellVisualiser> cellsVisuals = new();

    private void Awake()
    {
        Build();
    }

    public void Build()
    {
        Clear();
        for (int y =0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var pos = new Vector2Int(x, y);
                var cell = new Cell(pos);
                cells[pos] = cell;

                var go = Instantiate(cellPrefab, GridToWorld(pos), Quaternion.identity, transform);
                var view = go.GetComponent<CellVisualiser>();
                view.Init(pos);
                cellsVisuals[pos] = view;
            }
        }
    }

    public Vector3 GridToWorld(Vector2Int pos) => new Vector3(pos.x * cellSize, 0f, pos.y * cellSize);
    
    public bool TryGetCell(Vector2Int p, out Cell c) => cells.TryGetValue(p, out c);
    public IEnumerable<Cell> AllCells() => cells.Values;

    public void UpdateView(Cell cell)
    {
        if (!cellsVisuals.TryGetValue(cell.gridPos, out var v)) return;
        switch(cell.state)
        {
            case CellState.Empty: v.SetColor(Color.cyan * 0.3f); break;
            case CellState.Ship: v.SetColor(Color.gray * 0.6f); break; // alleen tonen op eigen bord
            case CellState.Miss: v.SetColor(Color.blue * 0.7f); break;
            case CellState.Hit: v.SetColor(Color.red); break;
        }
    }

    public void Clear()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        cells.Clear();
        cellsVisuals.Clear();
    }

    // References
    public int Width => width;
    public int Height => height;
}
