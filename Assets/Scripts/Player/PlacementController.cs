using System.Collections.Generic;
using UnityEngine;

public class PlacementController : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private GridManager grid;
    [SerializeField] private LayerMask cellMask;
    [SerializeField] private KeyCode rotateKey = KeyCode.R;

    [Header("Prefabs")]
    [SerializeField] private ShipGhost defaultGhostPrefab; // fallback als def.ghostPrefab leeg is

    [Header("Placement Visuals")]
    [SerializeField] private float ghostYOffset = 0.25f;       // hoogte voor ghost preview
    [SerializeField] private float shipVisualYOffset = 0.25f;   // hoogte voor definitieve visuals

    // state
    private Camera mainCam;
    private bool vertical = false;
    private int currentIndex = 0;
    private List<ShipDefinition> shipDefinitions;
    private readonly List<Ship> placedShips = new();

    private ShipGhost activeGhost;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    public void Begin(List<ShipDefinition> defs)
    {
        shipDefinitions = new List<ShipDefinition>(defs);
        currentIndex = 0;
        vertical = false;
        placedShips.Clear();
        SpawnGhostForCurrent();
    }

    public bool IsDone => shipDefinitions != null && currentIndex >= shipDefinitions.Count;

    public List<Ship> GetPlacedShips() => new List<Ship>(placedShips);

    private void Update()
    {
        if (shipDefinitions == null || IsDone) return;

        if (Input.GetKeyUp(rotateKey))
        {
            vertical = !vertical;
            if (activeGhost) activeGhost.Rotate();
        }

        if (Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out var hit, 100f, cellMask))
        {
            var view = hit.collider.GetComponent<CellVisualiser>();
            if (view)
            {
                var def = shipDefinitions[currentIndex];
                var cells = CollectCells(view.gridPos, def.length, vertical);
                bool valid = AreCellsFree(cells);

                // Preview tint op alle cells
                grid.ClearAllPreviews();
                grid.SetPreview(cells, valid ? PreviewKind.Valid : PreviewKind.Invalid, def);

                // Ghost op start-cell, hoger in Y, zonder scale aanpassen
                UpdateGhostTransform(view.gridPos);

                if (Input.GetMouseButtonDown(0) && valid)
                {
                    // Plaatsen in grid
                    var ship = new Ship(def);
                    foreach (var p in cells)
                    {
                        if (grid.TryGetCell(p, out var c)) { c.PlaceShip(ship); grid.UpdateView(c); }
                    }
                    ship.SetPositions(cells);
                    placedShips.Add(ship);

                    // Visuals: 1 prefab per segment (geen scaling, wel Y-offset)
                    if (def.shipVisualPrefab)
                        SpawnPlacedShipVisualSingleScaled(grid, ship, def, 0.25f);

                    currentIndex++;
                    grid.ClearAllPreviews();

                    // volgende ghost
                    DespawnGhost();
                    if (!IsDone) SpawnGhostForCurrent();
                }
            }
        }
    }

    private List<Vector2Int> CollectCells(Vector2Int start, int length, bool vert)
    {
        var list = new List<Vector2Int>(length);
        for (int i = 0; i < length; i++)
            list.Add(start + (vert ? new Vector2Int(0, i) : new Vector2Int(i, 0)));
        return list;
    }

    private bool AreCellsFree(List<Vector2Int> cellsList)
    {
        foreach (var p in cellsList)
        {
            if (!grid.TryGetCell(p, out var c)) return false;
            if (c.state != CellState.Empty) return false;
        }
        return true;
    }

    private void SpawnGhostForCurrent()
    {
        var def = shipDefinitions[currentIndex];
        ShipGhost prefab = null;

        if (def.ghostPrefab)
            prefab = def.ghostPrefab.GetComponent<ShipGhost>();

        if (!prefab) prefab = defaultGhostPrefab;
        if (!prefab) return;

        activeGhost = Instantiate(prefab, grid.transform);
        activeGhost.def = def;

        // Sync rotatie met huidige orientation
        if (activeGhost.vertical != vertical) activeGhost.Rotate();

        // Tint ghost (half transparant)
        TintGhost(activeGhost, def.color);
    }

    private void DespawnGhost()
    {
        if (activeGhost) Destroy(activeGhost.gameObject);
        activeGhost = null;
    }

    private void UpdateGhostTransform(Vector2Int startCell)
    {
        if (!activeGhost) return;
        var world = grid.GridToWorld(startCell);
        world.y += ghostYOffset;              // <<< hoger neerzetten
        activeGhost.SetPosition(world);
        // GEEN scale wijzigen => prefab scale blijft behouden
        // Rotatie gaat via ShipGhost.Rotate() op R
    }

    private void TintGhost(ShipGhost ghost, Color c)
    {
        var rends = ghost.GetComponentsInChildren<MeshRenderer>();
        var tint = new Color(c.r, c.g, c.b, 0.6f);
        foreach (var r in rends) if (r.material) r.material.color = tint;
    }

    private void SpawnPlacedShipVisualSingleScaled(GridManager grid, Ship ship, ShipDefinition def, float yOffset = 0.25f)
    {
        if (!def.shipVisualPrefab) return;

        // Bepaal oriëntatie op basis van de eerste twee segmenten
        bool vertical = ship.Occupied.Count <= 1 || ship.Occupied[0].x == ship.Occupied[1].x;

        // Bepaal middenpunt (centroid) tussen eerste en laatste cel - gebruik CELLCENTER
        var start = ship.Occupied[0];
        var end = ship.Occupied[ship.Occupied.Count - 1];

        Vector3 CellCenter(Vector2Int gp)
        {
            var w = grid.GridToWorld(gp);
            // verplaats naar midden van de tile
            return new Vector3(w.x + grid.CellSize * 0.5f, w.y, w.z + grid.CellSize * 0.5f);
        }

        var a = CellCenter(start);
        var b = CellCenter(end);
        var mid = (a + b) * 0.5f;
        mid.y += yOffset;

        var rot = vertical ? Quaternion.identity : Quaternion.Euler(0f, 90f, 0f);

        // Instantieer prefab op de middenpositie, met juiste oriëntatie
        var go = Instantiate(def.shipVisualPrefab, mid, rot, grid.transform);

        // Kleur optioneel toepassen
        var rends = go.GetComponentsInChildren<MeshRenderer>();
        foreach (var r in rends) if (r.material) r.material.color = def.color;

        // ---- Autoscale langs lokale Z zodat totale lengte == aantal cellen * cellSize ----
        // Huidige lengte bepalen via world-bounds na rotatie
        if (rends.Length > 0)
        {
            Bounds bounds = new Bounds(rends[0].bounds.center, Vector3.zero);
            for (int i = 0; i < rends.Length; i++)
                bounds.Encapsulate(rends[i].bounds);

            // Als prefab verticaal staat (rot==identity) is lengte op world Z,
            // na 90° rotatie wordt lokale Z wereld X. Meet dus as-afhankelijk.
            float currentLen = vertical ? bounds.size.z : bounds.size.x;
            float targetLen = ship.Occupied.Count * grid.CellSize;

            float mul = targetLen / Mathf.Max(currentLen, 0.0001f);

            var ls = go.transform.localScale;
            ls.z *= mul;          // alleen langs lokale Z schalen
            go.transform.localScale = ls;

            // Centraal laten staan: we positioneren op midden, dus extra correctie is niet nodig.
        }
    }

}
