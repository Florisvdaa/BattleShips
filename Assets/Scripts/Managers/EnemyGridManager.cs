using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyGridManager : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private GridSettings enemyGridSettings;
    [SerializeField] private GameObject hitMarkerPrefab;
    [SerializeField] private GameObject missMarkerPrefab;
    [SerializeField] private LayerMask enemyGridLayer;

    private GridData enemyGridData;
    private HashSet<Vector3> firedShots = new();

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
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPos = grid.WorldToCell(mouseWorld);

            TryShootAt(gridPos);
        }
    }
    private void TryShootAt(Vector3Int pos)
    {
        if (firedShots.Contains(pos)) return; // Already fired
        firedShots.Add(pos);

        bool isHit = enemyGridData.CanPlaceObjectAt(pos, Vector2Int.one) == false;

        Vector3 worldPos = grid.CellToWorld(pos);
        GameObject marker = Instantiate(isHit ? hitMarkerPrefab : missMarkerPrefab, worldPos, Quaternion.identity);

        Debug.Log(isHit ? "Hit!" : "Miss!");
    }

    private void PlaceEnemyShipsRandomly()
    {
        // You can re-use GridData.CanPlaceObjectAt
        // Loop through each ship type and randomly pick a position + rotation
        // Try until a valid spot is found
        // Use enemyGridData.AddObjectAt() to place
    }
}
