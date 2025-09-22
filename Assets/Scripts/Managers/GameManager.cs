using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] public GridManager playerGrid;
    [SerializeField] public GridManager aiGrid;
    [SerializeField] public Player human;
    [SerializeField] public AIPlayer ai;
    [SerializeField] public PlacementController placement;

    [Header("Ships")]
    [SerializeField] private List<ShipDefinition> shipDefs;

    [Header("Flow")]
    [SerializeField] private float turnDelay = 1f;

    private TurnSystem turns = new();
    private ScoreSystem score = new();
    private List<Ship> playerShips = new();
    private List<Ship> aiShips = new();

    private void Start()
    {
        Setup();
        StartCoroutine(PreGameThenLoop());
    }

    private void Setup()
    {
        playerGrid.Build();
        aiGrid.Build();
        playerShips = new List<Ship>();
        aiShips = new List<Ship>();
    }

    private System.Collections.IEnumerator PreGameThenLoop()
    {
        // Player placement
        placement.Begin(shipDefs);
        UIManager.Instance.SetStatus("Plaats je schepen (R = roteer) en klik om te plaatsen.");
        while (!placement.IsDone) yield return null;

        // Transfer geplaatste schepen uit placement naar ons lijstje
        playerShips = placement.GetPlacedShips();

        // AI auto placement
        aiShips = CreateAndPlaceShips(aiGrid);

        // Init spelers
        human.ownGrid = playerGrid; human.targetGrid = aiGrid; human.Init();
        ai.ownGrid = aiGrid; ai.targetGrid = playerGrid; ai.Init();

        UIManager.Instance.SetScore(score.PlayerHits, score.AIHits);
        UIManager.Instance.SetStatus("Jij begint. Klik op een vakje op het AI-bord.");

        yield return GameLoop();
    }

    private List<Ship> CreateAndPlaceShips(GridManager grid)
    {
        var list = new List<Ship>();
        foreach (var def in shipDefs)
        {
            var ship = new Ship(def);
            bool ok = ShipPlacement.TryPlaceShip(grid, ship);
            if (!ok) Debug.LogWarning($"Kon schip {def.displayName} niet plaatsen");
            list.Add(ship);

            // Optioneel: visueel prefab op AI grid (meestal uit), afhankelijk van revealShips
            if (grid.RevealShips && def.shipVisualPrefab)
            {
                SpawnPlacedShipVisualSingleScaled(grid, ship, def, 0.25f);
            }
        }
        return list;
    }

    private void SpawnPlacedShipVisualSingleScaled(GridManager grid, Ship ship, ShipDefinition def, float yOffset = 0.25f)
    {
        if (!def.shipVisualPrefab) return;

        bool vertical = ship.Occupied.Count <= 1 || ship.Occupied[0].x == ship.Occupied[1].x;

        Vector3 CellCenter(Vector2Int gp)
        {
            var w = grid.GridToWorld(gp);
            return new Vector3(w.x + grid.CellSize * 0.5f, w.y, w.z + grid.CellSize * 0.5f);
        }

        var a = CellCenter(ship.Occupied[0]);
        var b = CellCenter(ship.Occupied[^1]);
        var mid = (a + b) * 0.5f;
        mid.y += yOffset;

        var rot = vertical ? Quaternion.identity : Quaternion.Euler(0f, 90f, 0f);
        var go = Instantiate(def.shipVisualPrefab, mid, rot, grid.transform);

        var rends = go.GetComponentsInChildren<MeshRenderer>();
        foreach (var r in rends) if (r.material) r.material.color = def.color;

        if (rends.Length > 0)
        {
            Bounds bounds = new Bounds(rends[0].bounds.center, Vector3.zero);
            for (int i = 0; i < rends.Length; i++)
                bounds.Encapsulate(rends[i].bounds);

            float currentLen = vertical ? bounds.size.z : bounds.size.x;
            float targetLen = ship.Occupied.Count * grid.CellSize;

            float mul = targetLen / Mathf.Max(currentLen, 0.0001f);
            var ls = go.transform.localScale;
            ls.z *= mul;
            go.transform.localScale = ls;
        }
    }


    private System.Collections.IEnumerator GameLoop()
    {
        while (true)
        {
            if (turns.Current == Turn.Player)
            {
                UIManager.Instance.SetStatus("Jij bent aan de beurt");
                bool hit = false; bool turnDone = false;
                human.DoTurn(h => { hit = h; turnDone = true; });
                while (!turnDone) yield return null;

                if (hit) score.AddPlayerHit(); else turns.Next();
                UIManager.Instance.SetScore(score.PlayerHits, score.AIHits);
                if (AllSunk(aiShips)) { UIManager.Instance.SetStatus("Gewonnen!"); yield break; }

                yield return new WaitForSeconds(turnDelay);
            }
            else
            {
                UIManager.Instance.SetStatus("AI denkt...");
                yield return new WaitForSeconds(turnDelay);

                bool hit = false; bool turnDone = false;
                ai.DoTurn(h => { hit = h; turnDone = true; });
                while (!turnDone) yield return null;

                if (hit) score.AddAIHit(); else turns.Next();
                UIManager.Instance.SetScore(score.PlayerHits, score.AIHits);
                if (AllSunk(playerShips)) { UIManager.Instance.SetStatus("Verloren!"); yield break; }

                yield return new WaitForSeconds(turnDelay);
            }
        }
    }

    private bool AllSunk(List<Ship> ships)
    {
        foreach (var s in ships) if (!s.IsSunk) return false;
        return true;
    }
}
