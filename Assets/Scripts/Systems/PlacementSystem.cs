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

    private Dictionary<int, int> placedCounts = new();          // ID -> count
    private GridData allShipsData;                              // Holds global grid occupancy data (regardless of type)
    private Dictionary<int, GridData> gridDataMap;              // Individual data containers for each ship type (by ID)
    private List<GameObject> placedObjects = new();             // Keeps track of placed GameObjects
    private Vector3Int lastDetectedPosition = Vector3Int.zero;  // Used to detect mouse movement across grid cells
    private bool isRotated = false;                             // false = horizontal (0°), true = vertical (90°)
    private bool previewDirty = false;


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

        placedCounts.Clear();
        foreach (var d in objectsDatabase.objectsData)
            placedCounts[d.ID] = 0;

        UIManager.Instance.InitializeButtons(objectsDatabase);
    }

    // Begins the placement mode for a selected object ID
    public void StartPlacement(int ID)
    {
        StopPlacement();

        selectedObjectIndex = objectsDatabase.objectsData.FindIndex(x => x.ID == ID);
        if (selectedObjectIndex < 0) { Debug.LogError($"No ID found {ID}"); return; }

        var data = objectsDatabase.objectsData[selectedObjectIndex];

        // cap check: block if already at limit
        if (placedCounts.TryGetValue(data.ID, out var count) && count >= data.maxPlacements)
        {
            // already capped, just ensure UI is up to date and bail
            UIManager.Instance.UpdatePlacementState(data.ID, count, data.maxPlacements);
            selectedObjectIndex = -1;
            return;
        }

        gridVisualization.SetActive(true);
        //previewSystem.StartShowingPlacementPreview(
        //    objectsDatabase.objectsData[selectedObjectIndex].Prefab,
        //    objectsDatabase.objectsData[selectedObjectIndex].Size);
        previewSystem.StartShowingPlacementPreview(data.Prefab, data.Size);

        inputManager.OnRotation += RotateStructure;
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;

        previewDirty = true; // force first draw
    }

    // Rotates the current structure
    public void RotateStructure()
    {
        isRotated = !isRotated;

        var data = objectsDatabase.objectsData[selectedObjectIndex];
        Vector2Int rotatedSize = GetRotatedSize(data.Size);

        previewSystem.PrepareCursor(rotatedSize);
        previewSystem.SetRotation(isRotated ? 90f : 0f);

        previewDirty = true;
    }

    // Ends placement mode
    private void StopPlacement()
    {
        selectedObjectIndex = -1;
        gridVisualization.SetActive(false);
        previewSystem.StopShowinPreview();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        inputManager.OnRotation -= RotateStructure;
        lastDetectedPosition = Vector3Int.zero;
        previewDirty = false;
    }

    // Instantiates and places the selected structure on the grid
    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI()) return;

        Vector3 mousePos = inputManager.GetSelectedMapPosition();
        Vector3Int gridPos = grid.WorldToCell(mousePos);

        if (!CheckPlacementValidity(gridPos, selectedObjectIndex)) return;

        var data = objectsDatabase.objectsData[selectedObjectIndex];
        Vector2Int rotatedSize = GetRotatedSize(data.Size);

        GameObject newGO = Instantiate(data.Prefab);

        Quaternion baseRot = data.Prefab.transform.rotation;

        // place at the center of the occupied footprint
        newGO.transform.position = GetFootprintCenter(gridPos, data.Size, isRotated);
        newGO.transform.rotation = baseRot * Quaternion.Euler(0f, isRotated ? 90f : 0f, 0f);

        placedObjects.Add(newGO);

        if (gridDataMap.TryGetValue(data.ID, out GridData selectedData))
            selectedData.AddObjectAt(gridPos, rotatedSize, data.ID, placedObjects.Count - 1);

        allShipsData.AddObjectAt(gridPos, rotatedSize, data.ID, placedObjects.Count - 1);

        // update counts + UI
        placedCounts[data.ID] = placedCounts.GetValueOrDefault(data.ID) + 1;
        UIManager.Instance.UpdatePlacementState(data.ID, placedCounts[data.ID], data.maxPlacements);

        // reached the cap? auto-exit placement so they can't place more of this one
        if (placedCounts[data.ID] >= data.maxPlacements)
            StopPlacement();
        else
            previewSystem.UpdatePosition(grid.CellToWorld(gridPos), newGO.transform.position, false);
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
        if (selectedObjectIndex < 0) return;

        Vector3 mousePos = inputManager.GetSelectedMapPosition();
        Vector3Int gridPos = grid.WorldToCell(mousePos);

        if (lastDetectedPosition != gridPos || previewDirty)
        {
            bool placementValidity = CheckPlacementValidity(gridPos, selectedObjectIndex);

            var data = objectsDatabase.objectsData[selectedObjectIndex];
            Vector3 corner = grid.CellToWorld(gridPos);                         // for cursor
            Vector3 center = GetFootprintCenter(gridPos, data.Size, isRotated); // for ship

            mouseIndicator.transform.position = mousePos;
            previewSystem.UpdatePosition(corner, center, placementValidity);

            lastDetectedPosition = gridPos;
            previewDirty = false;
        }
    }

    private Vector2Int GetRotatedSize(Vector2Int originalSize)
    {
        return isRotated ? new Vector2Int(originalSize.y, originalSize.x) : originalSize;
    }

    // helper: from bottom-left corner to center of the footprint
    private Vector3 GetFootprintCenter(Vector3Int gridPos, Vector2Int size, bool rotated)
    {
        int w = rotated ? size.y : size.x; // cells in X
        int h = rotated ? size.x : size.y; // cells in Z

        Vector3 corner = grid.CellToWorld(gridPos);
        Vector3 cell = grid.cellSize;              // supports non-1 cell sizes

        // offset from corner to center of occupied rectangle
        Vector3 offset = new Vector3(w * cell.x * 0.5f, 0f, h * cell.z * 0.5f);
        return corner + offset;
    }
}
