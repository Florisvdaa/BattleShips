using System.Collections.Generic;
using UnityEngine;

public static class ShipPlacement
{
    public static bool TryPlaceShip(GridManager grid, Ship ship)
    {
        int tries = 200;
        while (tries-- > 0)
        {
            bool vertical = Random.value > 0.5f;
            int maxX = vertical ? grid.Width : grid.Width - ship.Def.length + 1;
            int maxY = vertical ? grid.Height - ship.Def.length + 1 : grid.Height;
            var start = new Vector2Int(Random.Range(0, maxX), Random.Range(0, maxY));

            var cells = new List<Vector2Int>();
            bool blocked = false;
            for (int i = 0; i < ship.Def.length; i++)
            {
                var p = start + (vertical ? new Vector2Int(0, i) : new Vector2Int(i, 0));
                if (!grid.TryGetCell(p, out var c) || c.state != CellState.Empty) { blocked = true; break; }
                cells.Add(p);
            }
            if (blocked) continue;

            foreach (var cp in cells)
            {
                grid.TryGetCell(cp, out var cell);
                cell.PlaceShip(ship);
                grid.UpdateView(cell);
            }
            ship.SetPositions(cells);
            return true;
        }
        return false;
    }
}
