using UnityEngine;

public enum CellState { Empty, Ship, Miss, Hit }

public class Cell
{
    public Vector2Int gridPos {  get; private set; }
    public CellState state { get; private set; } = CellState.Empty;
    public Ship occupyingShip { get; private set; }
    
    public Cell(Vector2Int pos) { gridPos = pos; }

    public void PlaceShip(Ship ship) 
    {
        occupyingShip = ship;
        state = CellState.Ship;
    }

    public bool Shoot()
    {
        if(state == CellState.Empty)
        {
            state = CellState.Miss;
            return false;
        }
        if (state == CellState.Ship)
        {
            state = CellState.Hit;
            occupyingShip?.RegisterHit(gridPos);
            return true;
        }

        // Allready shot leave it as it is
        return state == CellState.Hit;
    }
}
