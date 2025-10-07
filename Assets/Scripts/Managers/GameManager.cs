using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GridSpawner playerGridSpawner;
    [SerializeField] private GridSpawner enemyGridSpawner;
    public List<Ship> shipsToPlace = new List<Ship>()
    {
        new Ship() { name = "Carrier" , length = 5},
        new Ship() { name = "Cruiser" , length = 3},
    };

    private Dictionary<GridOwner, Cell[,]> gridCells = new Dictionary<GridOwner, Cell[,]>();
    private int playerWidth = 10;
    private int playerHeight = 10;
    private bool placementMode = true;
    private int currentShipIndex = 0;
    private bool placingHorizontal = true;
    void Start()
    {
        if (playerGridSpawner == null || enemyGridSpawner == null) Debug.LogError("Assign spawners");
        BuildCellLookup(playerGridSpawner);
        BuildCellLookup(enemyGridSpawner);
        HookCellEvents(playerGridSpawner.transform, GridOwner.Player);
        HookCellEvents(enemyGridSpawner.transform, GridOwner.Enemy);
    }

    void BuildCellLookup(GridSpawner spawner)
    {
        int w = spawner.Width;
        int h = spawner.Height;
        Cell[,] arr = new Cell[w, h];
        foreach (Transform child in spawner.transform)
        {
            Cell c = child.GetComponent<Cell>();
            if (c == null) continue;
            arr[c.GridPos.x, c.GridPos.y] = c;
        }
        gridCells[spawner.GridOwner] = arr;
    }

    void HookCellEvents(Transform parent, GridOwner owner)
    {
        foreach (Transform t in parent)
        {
            Cell c = t.GetComponent<Cell>();
            if (c == null) continue;
            c.OnClicked += cell => OnCellClicked(cell);
            c.OnHovered += cell => OnCellHovered(cell);
            c.OnHoverExit += cell => OnCellHoverExit(cell);
            // Hide enemy ships visually when not placed by player
            if (owner == GridOwner.Enemy)
            {
                // ensure enemy cells use default material to hide ships
            }
        }
    }

    void OnCellHovered(Cell c)
    {
        if (!placementMode) return;
        if (c.CellOwner != GridOwner.Player) return;
        HighlightPlacement(c.GridPos);
    }

    void OnCellHoverExit(Cell c)
    {
        if (!placementMode) return;
        if (c.CellOwner != GridOwner.Player) return;
        ClearTemporaryHighlights();
    }

    void OnCellClicked(Cell c)
    {
        if (placementMode && c.CellOwner == GridOwner.Player)
        {
            TryPlaceCurrentShipAt(c.GridPos);
            return;
        }

        if (!placementMode && c.CellOwner == GridOwner.Enemy)
        {
            ShootAtCell(c);
            return;
        }
    }

    void ShootAtCell(Cell c)
    {
        if (c.CellState == CellState.Empty)
        {
            c.MarkAsMiss();
            // enemy turn or other logic here
        }
        else if (c.CellState == CellState.Ship)
        {
            c.MarkAsHit();
            // check for sunk ship
        }
    }

    void HighlightPlacement(Vector2Int basePos)
    {
        Ship ship = shipsToPlace[currentShipIndex];
        var cells = GetCellsForPlacement(GridOwner.Player, basePos, ship.length, placingHorizontal);
        if (cells == null) return;
        foreach (var cell in cells)
        {
            if (cell.CellState == CellState.Empty)
            {
                if (cell.GetComponent<Renderer>() != null)
                    cell.GetComponent<Renderer>().material = cell.HoverMat;
            }
        }
    }

    void ClearTemporaryHighlights()
    {
        var arr = gridCells[GridOwner.Player];
        for (int x = 0; x < arr.GetLength(0); x++)
            for (int y = 0; y < arr.GetLength(1); y++)
                arr[x, y]?.ApplyMaterialForState();
    }

    Cell[] GetCellsForPlacement(GridOwner owner, Vector2Int basePos, int length, bool horizontal)
    {
        var arr = gridCells[owner];
        int w = arr.GetLength(0);
        int h = arr.GetLength(1);
        var result = new List<Cell>();
        for (int i = 0; i < length; i++)
        {
            int x = basePos.x + (horizontal ? i : 0);
            int y = basePos.y + (horizontal ? 0 : i);
            if (x < 0 || x >= w || y < 0 || y >= h) return null;
            result.Add(arr[x, y]);
        }
        return result.ToArray();
    }

    void TryPlaceCurrentShipAt(Vector2Int basePos)
    {
        Ship ship = shipsToPlace[currentShipIndex];
        var cells = GetCellsForPlacement(GridOwner.Player, basePos, ship.length, placingHorizontal);
        if (cells == null) { Debug.Log("Invalid placement (out of bounds)"); return; }

        foreach (var c in cells)
        {
            if (c.CellState == CellState.Ship) { Debug.Log("Overlaps another ship"); return; }
        }

        foreach (var c in cells)
        {
            c.MarkAsShip();
            ship.positions.Add(c.GridPos);
        }

        ship.placed = true;
        ship.horizontal = placingHorizontal;

        currentShipIndex++;
        if (currentShipIndex >= shipsToPlace.Count)
        {
            placementMode = false;
            Debug.Log("All ships placed, switch to battle mode");
            // Optionally auto-place enemy ships here
        }
    }

    // Public helper to toggle orientation from UI or key
    public void TogglePlacementOrientation()
    {
        placingHorizontal = !placingHorizontal;
        Debug.Log($"Placing horizontally: {placingHorizontal}");
    }

}
