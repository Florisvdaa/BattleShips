using System;
using System.Collections.Generic;
using UnityEngine;

// Manages object placement on a grid-based map
public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject mouseIndicator;
    [SerializeField] private InputManager inputManager;

    [SerializeField] private Grid grid;
    [SerializeField] private ObjectsDatabase objectsDatabase;
    [SerializeField] private int selectedObjectIndex = -1;

    [SerializeField] private GameObject gridVisualization;
    [SerializeField] private PreviewSystem previewSystem;
    [SerializeField] private GridSettings gridSettings;

    
    private GridData allShipsData;                              // Holds global grid occupancy data (regardless of type)
    private Dictionary<int, GridData> gridDataMap;              // Individual data containers for each ship type (by ID)
    private List<GameObject> placedObjects = new();             // Keeps track of placed GameObjects
    private Vector3Int lastDetectedPosition = Vector3Int.zero;  // Used to detect mouse movement across grid cells
    private bool isRotated = false;                             // false = horizontal (0°), true = vertical (90°)

    private void Start()
    {
        StopPlacement();

        // Initialize global and per-ship grid data
        allShipsData = new GridData();
        gridDataMap = new Dictionary<int, GridData>
        {
            { 0, new GridData() },
            { 1, new GridData() },
            { 2, new GridData() },
            { 3, new GridData() }
        };

        // Apply grid dimensions and origin from settings
        allShipsData.SetGridBounds(gridSettings.width, gridSettings.height);
        allShipsData.SetGridOrigin(gridSettings.origin);
    }

    // Begins the placement mode for a selected object ID
    public void StartPlacement(int ID)
    {
        StopPlacement();
        selectedObjectIndex = objectsDatabase.objectsData.FindIndex(x => x.ID == ID);
        
        if (selectedObjectIndex < 0)
        {
            Debug.LogError($"No ID found {ID}");
            return;
        }

        gridVisualization.SetActive(true);
        previewSystem.StartShowingPlacementPreview(objectsDatabase.objectsData[selectedObjectIndex].Prefab, objectsDatabase.objectsData[selectedObjectIndex].Size);
        inputManager.OnRotation += RotateStructure;
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    // Rotates the current structure
    public void RotateStructure()
    {
        isRotated = !isRotated;

        var data = objectsDatabase.objectsData[selectedObjectIndex];
        Vector2Int rotatedSize = GetRotatedSize(data.Size);

        previewSystem.PrepareCursor(rotatedSize);
        previewSystem.SetRotation(isRotated ? 90f : 0f);
    }

    // Ends placement mode
    private void StopPlacement()
    {
        selectedObjectIndex = -1;
        gridVisualization.SetActive(false);
        previewSystem.StopShowinPreview();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
    }

    // Instantiates and places the selected structure on the grid
    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI())
            return;

        Vector3 mousePos = inputManager.GetSelectedMapPosition();
        Vector3Int gridPos = grid.WorldToCell(mousePos);


        if (!CheckPlacementValidity(gridPos, selectedObjectIndex))
            return;

        GameObject newGO = Instantiate(objectsDatabase.objectsData[selectedObjectIndex].Prefab);
        newGO.transform.position = grid.CellToWorld(gridPos);
        newGO.transform.rotation = Quaternion.Euler(0, isRotated ? 90f : 0f, 0); // Preserve rotation

        placedObjects.Add(newGO);

        var data = objectsDatabase.objectsData[selectedObjectIndex];
        Vector2Int rotatedSize = GetRotatedSize(data.Size);

        // Type-specific
        if (gridDataMap.TryGetValue(data.ID, out GridData selectedData))
        {
            selectedData.AddObjectAt(gridPos, rotatedSize, data.ID, placedObjects.Count - 1);
        }

        // Add to global GridData
        allShipsData.AddObjectAt(gridPos, rotatedSize, data.ID, placedObjects.Count - 1);

        // Hide preview after placing
        previewSystem.UpdatePosition(grid.CellToWorld(gridPos), false);
    }

    // Checks if the structure can be legally placed at the current grid position
    private bool CheckPlacementValidity(Vector3Int gridPos, int selectedObjectIndex)
    {
        var data = objectsDatabase.objectsData[selectedObjectIndex];
        Vector2Int rotatedSize = GetRotatedSize(data.Size);
        return allShipsData.CanPlaceObjectAt(gridPos, rotatedSize);
    }


    // Updates the preview based on the current mouse position
    private void Update()
    {
        if (selectedObjectIndex < 0)
            return;

        Vector3 mousePos = inputManager.GetSelectedMapPosition();
        Vector3Int gridPos = grid.WorldToCell(mousePos);

        // Only update when mouse has moved to a new grid cell
        if (lastDetectedPosition != gridPos)
        {
            bool placementValidity = CheckPlacementValidity(gridPos, selectedObjectIndex);

            mouseIndicator.transform.position = mousePos;
            previewSystem.UpdatePosition(grid.CellToWorld(gridPos), placementValidity);
            lastDetectedPosition = gridPos;
        }
    }

    private Vector2Int GetRotatedSize(Vector2Int originalSize)
    {
        return isRotated ? new Vector2Int(originalSize.y, originalSize.x) : originalSize;
    }
}
