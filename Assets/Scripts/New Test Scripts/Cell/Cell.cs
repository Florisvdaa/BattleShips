using System;
using UnityEngine;

public enum CellState { Empty, Ship, Miss, Hit }
public class Cell : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private Vector2Int gridPos;
    [SerializeField] private GridOwner owner;
    [SerializeField] private CellState state = CellState.Empty;
    [SerializeField] private Material defaultMat;
    [SerializeField] private Material hoverMat;
    [SerializeField] private Material shipMat;
    [SerializeField] private Material missMat;
    [SerializeField] private Material hitMat;

    private Renderer rend;

    public event Action<Cell> OnClicked;
    public event Action<Cell> OnHovered;
    public event Action<Cell> OnHoverExit;

    public void Init(int newId, Vector2Int pos, GridOwner gridOwner)
    {
        id = newId;
        gridPos = pos;
        owner = gridOwner;
        rend = GetComponentInChildren<Renderer>();
        if (rend == null) rend = gameObject.AddComponent<MeshRenderer>();
        if (defaultMat != null) rend.material = defaultMat;

    }
    private void OnMouseEnter()
    {
        if (hoverMat != null) rend.material = hoverMat;
        OnHovered?.Invoke(this);
    }

    private void OnMouseExit()
    {
        ApplyMaterialForState();
        OnHoverExit?.Invoke(this);
    }

    private void OnMouseDown()
    {
        OnClicked?.Invoke(this);
    }

    public void ApplyMaterialForState()
    {
        if (rend == null) rend = GetComponent<Renderer>();
        switch (state)
        {
            case CellState.Empty:
                if (defaultMat != null) rend.material = defaultMat;
                break;
            case CellState.Ship:
                if (shipMat != null) rend.material = shipMat;
                break;
            case CellState.Miss:
                if (missMat != null) rend.material = missMat;
                break;
            case CellState.Hit:
                if (hitMat != null) rend.material = hitMat;
                break;
        }
    }

    public void MarkAsShip()
    {
        state = CellState.Ship;
        ApplyMaterialForState();
    }

    public void MarkAsMiss()
    {
        state = CellState.Miss;
        ApplyMaterialForState();
    }

    public void MarkAsHit()
    {
        state = CellState.Hit;
        ApplyMaterialForState();
    }

    // References
    public Vector2Int GridPos => gridPos;

    public GridOwner CellOwner => owner;

    public CellState CellState => state;

    public Material HoverMat => hoverMat;

}
