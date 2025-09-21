using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : PlayerBase
{
    private readonly List<Vector2Int> candidates = new();

    public override void Init()
    {
        candidates.Clear();
        for (int y = 0; y < targetGrid.Height; y++)
            for (int x = 0; x < targetGrid.Width; x++)
                candidates.Add(new Vector2Int(x, y));
    }


    public override void DoTurn(System.Action<bool> onFinished)
    {
        // Heel simpel: kies random uit nog niet-geschoten cellen;
        // uitbreiding: gewicht geven aan buren van een hit
        Vector2Int pick = PickCell();
        if (targetGrid.TryGetCell(pick, out var cell))
        {
            bool hit = cell.Shoot();
            targetGrid.UpdateView(cell);


            // Hunt: als hit, push buren naar voorkant
            if (hit)
            {
                PushIfValid(pick + Vector2Int.up);
                PushIfValid(pick + Vector2Int.down);
                PushIfValid(pick + Vector2Int.left);
                PushIfValid(pick + Vector2Int.right);
            }
            onFinished?.Invoke(hit);
        }
        else onFinished?.Invoke(false);
    }


    private Vector2Int PickCell()
    {
        // Verwijder al geschoten cellen uit kandidaten
        for (int i = candidates.Count - 1; i >= 0; i--)
        {
            if (!targetGrid.TryGetCell(candidates[i], out var c)) { candidates.RemoveAt(i); continue; }
            if (c.state == CellState.Miss || c.state == CellState.Hit) candidates.RemoveAt(i);
        }
        if (candidates.Count == 0) return Vector2Int.zero;
        return candidates[Random.Range(0, candidates.Count)];
    }


    private void PushIfValid(Vector2Int p)
    {
        if (!targetGrid.TryGetCell(p, out var c)) return;
        if (c.state == CellState.Empty || c.state == CellState.Ship) candidates.Insert(0, p);
    }
}
