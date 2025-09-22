using System;
using System.Collections;
using UnityEngine;

public class Player : PlayerBase
{
    [SerializeField] private LayerMask cellMask;

    public override void Init() { }

    public override void DoTurn(Action<bool> onFinished)
    {
        StartCoroutine(WaitClick(onFinished));
    }

    private IEnumerator WaitClick(System.Action<bool> done)
    {
        bool acted = false; bool hit = false;
        while (!acted)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hitInfo, 100f, cellMask))
                {
                    var view = hitInfo.collider.GetComponent<CellVisualiser>();
                    if (view && targetGrid.TryGetCell(view.gridPos, out var cell))
                    {
                        // blokkeer al-geschoten cellen
                        if (cell.state == CellState.Miss || cell.state == CellState.Hit)
                        {
                            // niks doen
                        }
                        else
                        {
                            bool h = cell.Shoot();
                            targetGrid.UpdateView(cell);
                            hit = h; acted = true;
                        }
                    }
                }
            }
            yield return null;
        }
        done?.Invoke(hit);
    }
}
