using System;
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

    private void Start()
    {
        StopPlacement();
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
        GameObject newGO = Instantiate(objectsDatabase.objectsData[selectedObjectIndex].Prefab);
        newGO.transform.position = grid.CellToWorld(gridPos);
    }

    private void Update()
    {
        if (selectedObjectIndex < 0)
            return;
        Vector3 mousePos = inputManager.GetSelectedMapPosition();
        Vector3Int gridPos = grid.WorldToCell(mousePos);
        mouseIndicator.transform.position = mousePos;   
        cellIndicator.transform.position = grid.CellToWorld(gridPos);
    }
}
