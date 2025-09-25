using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera sceneCamera;

    [SerializeField] private LayerMask placementLayermask;

    private Vector3 lastPos;

    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100 , placementLayermask))
        {
            lastPos = hit.point;
        }
        return lastPos;
    }
}
