using UnityEngine;

public class ShipGhost : MonoBehaviour
{
    public ShipDefinition def;
    public bool vertical = true;

    public void SetPosition(Vector3 worldPos)
    {
        transform.position = worldPos;
    }

    public void Rotate()
    {
        vertical = !vertical;
        transform.rotation = Quaternion.Euler(0,vertical ? 0 : 90, 0);
    }
}
