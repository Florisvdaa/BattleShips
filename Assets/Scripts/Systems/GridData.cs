using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// Stores and manages data about placed objects on a logical grid
public class GridData
{
    // Stores the occupied grid positions and associated placement data
    Dictionary<Vector3Int, PlacementData> placedObjects = new();

    // Grid boundaries (default: unlimited)
    private int gridWidth = int.MaxValue;   
    private int gridHeight = int.MaxValue;

    // Starting point (origin) of the grid
    private Vector2Int gridOrigin = Vector2Int.zero;

    // Sets the starting grid cell (useful if grid doesn't begin at (0, 0))
    public void SetGridOrigin(Vector2Int origin)
    {
        gridOrigin = origin;
    }

    // Defines the max width and height of the grid
    public void SetGridBounds(int width, int height)
    {
        gridWidth = width;
        gridHeight = height;
    }

    // Stores a new placed object on the grid at a given position and size
    public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int ID, int placedObjectIndex)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex);

        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
                throw new Exception($"Dictionary already contains object on this pos {pos}");
            placedObjects[pos] = data;
        }
    }

    // Calculates all the grid positions a multi-cell object will occupy
    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }
        return returnVal;
    }

    // Checks if the object can be placed at the target position without overlapping or exceeding bounds
    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition,objectSize);
        foreach (var pos in positionToOccupy)
        {
            // Reject if position already occupied
            if (placedObjects.ContainsKey(pos))
                return false;

            // Reject if position is outside the allowed grid bounds
            if (pos.x < gridOrigin.x || pos.z < gridOrigin.y || pos.x >= gridOrigin.x + gridWidth || pos.z >= gridOrigin.y + gridHeight)
                return false;
        }
        return true;
    }
}

// Stores information about a single placed object (used internally in GridData)
public class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int ID {  get; private set; }    
    public int placedObjectIndex { get; private set; }
    public PlacementData(List<Vector3Int> occupiedPositions, int iD, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        this.placedObjectIndex = placedObjectIndex;
    }
}
