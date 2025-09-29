using System;
using UnityEngine;

// Manages the visual feedback of structure placement previews
public class PreviewSystem : MonoBehaviour
{
    [SerializeField] private float previewYOffset = 0.06f;
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private Material previewMaterialPrefab;
    //[SerializeField] private float rotatedXOffset = -0.5f;

    private GameObject previewObject;
    private Material previewMaterialInstance;
    private Renderer cellIndicatorRenderer;
    private Quaternion baseRotation = Quaternion.identity;

    private void Start()
    {
        // Create a separate material instance to tint preview objects
        previewMaterialInstance = new Material(previewMaterialPrefab);
        cellIndicator.SetActive(false);
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    // Initializes preview visuals for a selected object
    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)
    {
        previewObject = Instantiate(prefab);

        baseRotation = previewObject.transform.rotation;
        
        PreparePreview(previewObject);
        PrepareCursor(size);
        cellIndicator.SetActive(true);
    }

    // Adjusts the cursor indicator to match object size
    public void PrepareCursor(Vector2Int size)
    {
        if (size.x > 0 ||  size.y > 0)
        {
            cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
            cellIndicatorRenderer.material.mainTextureScale = size;
        }
    }

    // Applies the preview material to all renderers in the object
    public void PreparePreview(GameObject previewObject)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = previewMaterialInstance;
            }
            renderer.materials = materials;
        }
    }

    // Cleans up the preview object and indicator
    public void StopShowinPreview()
    {
        cellIndicator.SetActive(false);
        Destroy(previewObject);
    }

    // Updates the preview visuals at a new position and applies placement validity feedback
    public void UpdatePosition(Vector3 cornerPos, Vector3 centerPos, bool validity)
    {
        MoveCursor(cornerPos);
        MovePreview(centerPos);
        ApplyFeedback(validity);
    }

    // Tints the preview and indicator based on validity
    private void ApplyFeedback(bool validity)
    {
        Color c = validity ? Color.white : Color.red;
        c.a = 0.5f;
        cellIndicatorRenderer.material.color = c;
        previewMaterialInstance.color = c;
    }

    private void MoveCursor(Vector3 pos)
    {
        cellIndicator.transform.position = pos;
    }

    private void MovePreview(Vector3 centerPos)
    {
        previewObject.transform.position = new Vector3(
        centerPos.x,
        centerPos.y + previewYOffset,
        centerPos.z
        );
    }

    public void SetRotation(float angle)
    {
        if (previewObject != null)
        {
            previewObject.transform.rotation = baseRotation * Quaternion.Euler(0f, angle, 0f);
        }
    }

}
