using System;
using UnityEngine;

// Manages the visual feedback of structure placement previews
public class PreviewSystem : MonoBehaviour
{
    [SerializeField] private float previewYOffset = 0.06f;
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private Material previewMaterialPrefab;
    [SerializeField] private float rotatedXOffset = -0.5f;

    private GameObject previewObject;
    private Material previewMaterialInstance;
    private Renderer cellIndicatorRenderer;

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
    public void UpdatePosition(Vector3 pos, bool validity)
    {
        MovePreview(pos);
        MoveCursor(pos);
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

    private void MovePreview(Vector3 pos)
    {
        float offsetX = previewObject.transform.rotation.eulerAngles.y == 90f ? rotatedXOffset : 0f;

        previewObject.transform.position = new Vector3(
            pos.x + offsetX,
            pos.y + previewYOffset,
            pos.z
        );
    }

    public void SetRotation(float angle)
    {
        if (previewObject != null)
        {
            previewObject.transform.rotation = Quaternion.Euler(0, angle, 0);
        }
    }

}
