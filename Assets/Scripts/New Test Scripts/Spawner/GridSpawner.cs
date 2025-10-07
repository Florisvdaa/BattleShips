using UnityEngine;

public enum GridOwner { Player, Enemy}
public class GridSpawner : MonoBehaviour
{
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private float spacing = 1.05f;
    [SerializeField] private GridOwner owner = GridOwner.Player;
    [SerializeField] private Vector2 origin = Vector2.zero;

    private void Start()
    {
        SpawnGrid();
    }

    private void SpawnGrid()
    {
        if (cellPrefab == null)
        {
            Debug.LogError("Cell prefab not set ");
            return;
        }

        int idCounter = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 worldPos = new Vector3(origin.x + x * spacing, 0f, origin.y + y * spacing);
                GameObject go = Instantiate(cellPrefab, worldPos, Quaternion.identity, transform);
                go.name = $"Cell_{owner}_{x}_{y}";
                Cell cell = go.GetComponent<Cell>();
                if (cell == null) cell = go.AddComponent<Cell>();
                cell.Init(idCounter, new Vector2Int(x, y), owner);
                idCounter++;

            }
        }
    }

    // References
    public int Width => width;
    public int Height => height;
    public GridOwner GridOwner => owner;
}
