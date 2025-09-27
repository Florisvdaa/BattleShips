using System;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject mouseIndicator;
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private InputManager inputManager;

    [SerializeField] private Grid grid;
    [SerializeField] private ObjectsDatabase objectsDatabase;
    [SerializeField] private int selectedObjectIndex = -1;

    [SerializeField] private GameObject gridVisualization;

    private GridData gridData, boatData;
    private Renderer previewRenderer;
    private List<GameObject> placedObjects = new();

    private void Start()
    {
        Debug.Log("Start");

        StopPlacement();
        gridData = new();
        boatData = new();
        previewRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

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
        cellIndicator.SetActive(true);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void StopPlacement()
    {
        selectedObjectIndex = -1;
        gridVisualization.SetActive(false);
        cellIndicator.SetActive(false);
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
    }

    private void PlaceStructure()
    {
        if(inputManager.IsPointerOverUI())
        {
            return;
        }
        Vector3 mousePos = inputManager.GetSelectedMapPosition();
        Vector3Int gridPos = grid.WorldToCell(mousePos);

        bool placementValidity = CheckPlacementValidity(gridPos, selectedObjectIndex);
        if (!placementValidity)
            return;

        GameObject newGO = Instantiate(objectsDatabase.objectsData[selectedObjectIndex].Prefab);
        newGO.transform.position = grid.CellToWorld(gridPos);

        placedObjects.Add(newGO);
        GridData selectedData = objectsDatabase.objectsData[selectedObjectIndex].ID == 0 ? gridData : boatData;
        selectedData.AddObjectAt(gridPos, objectsDatabase.objectsData[selectedObjectIndex].Size, objectsDatabase.objectsData[selectedObjectIndex].ID, placedObjects.Count - 1);
    }

    private bool CheckPlacementValidity(Vector3Int gridPos, int selectedObjectIndex)
    {
        GridData selectedData = objectsDatabase.objectsData[selectedObjectIndex].ID == 0 ? gridData : boatData;

        return selectedData.CanPlaceObjectAt(gridPos, objectsDatabase.objectsData[selectedObjectIndex].Size);
    }

    private void Update()
    {
        if (selectedObjectIndex < 0)
            return;
        Vector3 mousePos = inputManager.GetSelectedMapPosition();
        Vector3Int gridPos = grid.WorldToCell(mousePos);

        bool placementValidity = CheckPlacementValidity(gridPos, selectedObjectIndex);
        previewRenderer.material.color = placementValidity ? Color.green : Color.red;

        mouseIndicator.transform.position = mousePos;   
        cellIndicator.transform.position = grid.CellToWorld(gridPos);
    }
}
