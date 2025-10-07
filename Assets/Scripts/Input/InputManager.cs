using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Manages user input, raycasting, and UI interaction checks
public class InputManager : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private LayerMask placementLayermask;
    [SerializeField] private LayerMask enemyBoardLayermask;

    private Vector3 lastPos;

    // Input events
    public event Action OnClicked, OnExit, OnRotation;

    private void Update()
    {
        // Left mouse click
        if (Input.GetMouseButtonDown(0))
            OnClicked?.Invoke();

        // ESC key to cancel placement
        if (Input.GetKeyDown(KeyCode.Escape)) 
             OnExit?.Invoke(); 

        if(Input.GetKeyDown(KeyCode.Space))
            OnRotation?.Invoke();
    }

    // Checks if the mouse is over a UI element (e.g., button, panel)
    public bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();

    // Casts a ray from the mouse position into the scene to find a valid placement point
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
        else if(Physics.Raycast(ray, out hit, 100, placementLayermask))
        {

        }
        
        return lastPos;
    }
}
