using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    [Header("Refs")] 
    public GridManager playerGrid; 
    public GridManager aiGrid; 
    public Player human; public AIPlayer ai;

    [Header("Ships")] public List<ShipDefinition> shipDefs; // Maak 5 assets en sleep ze hier in (1x5,1x4,2x3,1x2)

    [SerializeField] private float turnDelay = 1f;

    private TurnSystem turns = new();
    private ScoreSystem score = new();
    private List<Ship> playerShips = new();
    private List<Ship> aiShips = new();


    void Start()
    {
        Setup();
        StartCoroutine(GameLoop());
    }


    void Setup()
    {
        playerGrid.Build();
        aiGrid.Build();

        // Plaats schepen op beide borden (schepen zichtbaar maken op eigen bord via GridManager.UpdateView)
        playerShips = CreateAndPlaceShips(playerGrid);
        aiShips = CreateAndPlaceShips(aiGrid);

        human.ownGrid = playerGrid; human.targetGrid = aiGrid; human.Init();
        ai.ownGrid = aiGrid; ai.targetGrid = playerGrid; ai.Init();

        UIManager.Instance.SetScore(score.PlayerHits, score.AIHits);
        UIManager.Instance.SetStatus("Jij begint. Klik op een vakje op het AI-bord.");
    }
    List<Ship> CreateAndPlaceShips(GridManager grid)
    {
        var list = new List<Ship>();
        foreach (var def in shipDefs)
        {
            var ship = new Ship(def);
            bool ok = ShipPlacement.TryPlaceShip(grid, ship);
            if (!ok) Debug.LogWarning($"Kon schip {def.displayName} niet plaatsen");
            list.Add(ship);
        }
        return list;
    }


    System.Collections.IEnumerator GameLoop()
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
            }
            else
            {
                UIManager.Instance.SetStatus("AI denkt...");
                yield return new WaitForSeconds(turnDelay); // <== Delay hier


                bool hit = false; bool turnDone = false;
                ai.DoTurn(h => { hit = h; turnDone = true; });
                while (!turnDone) yield return null;


                if (hit) score.AddAIHit(); else turns.Next();
                UIManager.Instance.SetScore(score.PlayerHits, score.AIHits);
                if (AllSunk(playerShips)) { UIManager.Instance.SetStatus("Verloren!"); yield break; }

                yield return new WaitForSeconds(turnDelay);
            }
            yield return null;
        }
    }


    bool AllSunk(List<Ship> ships)
    {
        foreach (var s in ships) if (!s.IsSunk) return false; return true;
    }
}