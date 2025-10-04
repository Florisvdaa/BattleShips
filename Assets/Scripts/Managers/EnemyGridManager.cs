using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyGridManager : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private GridSettings enemyGridSettings;
    [SerializeField] private GameObject hitMarkerPrefab;
    [SerializeField] private GameObject missMarkerPrefab;
    [SerializeField] private LayerMask enemyGridLayer;
    [SerializeField] private Camera sceneCamera;

    private GridData enemyGridData;
    private HashSet<Vector3> firedShots = new();
    private Vector3 lastPos;

    private void Start()
    {
        enemyGridData = new GridData();
        enemyGridData.SetGridBounds(enemyGridSettings.width, enemyGridSettings.height);
        enemyGridData.SetGridOrigin(enemyGridSettings.origin);

        PlaceEnemyShipsRandomly();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector3 hitPos = GetSelectedMapPosition();  // Use raycast hit from InputManager
            Vector3Int gridPos = grid.WorldToCell(hitPos);

            TryShootAt(gridPos);
        }
    }
    private void TryShootAt(Vector3Int pos)
    {
        if (firedShots.Contains(pos)) return;
        firedShots.Add(pos);

        // Your hit detection logic here

        // For example:
        //bool isHit = /* your condition here */;
        //Vector3 worldPos = grid.CellToWorld(pos);
        //Instantiate(isHit ? hitMarkerPrefab : missMarkerPrefab, worldPos, Quaternion.identity);
    }

    private void PlaceEnemyShipsRandomly()
    {
        // You can re-use GridData.CanPlaceObjectAt
        // Loop through each ship type and randomly pick a position + rotation
        // Try until a valid spot is found
        // Use enemyGridData.AddObjectAt() to place
    }
    private Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, enemyGridLayer))
        {
            lastPos = hit.point;
        }
        return lastPos;
    }
}
