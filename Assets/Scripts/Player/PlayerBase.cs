using UnityEngine;

public abstract class PlayerBase : MonoBehaviour
{
    public GridManager ownGrid;
    public GridManager targetGrid;

    public abstract void Init();
    public abstract void DoTurn(System.Action<bool> onFinished);
}
