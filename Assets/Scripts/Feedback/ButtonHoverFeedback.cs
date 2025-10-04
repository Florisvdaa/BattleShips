using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ButtonHoverFeedback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Hover Effect")]
    public float scaleMultiplier = 1.1f;
    public float moveOffsetX = 10f;
    public float animationSpeed = 10f;

    private Vector3 originalScale;
    private Vector3 originalPosition;
    private Vector3 targetScale;
    private Vector3 targetPosition;

    private bool isHovered = false;
    private bool isSelected = false;

    private void Start()
    {
        // Save starting values
        originalScale = transform.localScale;
        originalPosition = transform.localPosition;

        targetScale = originalScale;
        targetPosition = originalPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isSelected) return;

        isHovered = true;
        ApplyHoverState();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isSelected) return;

        isHovered = false;
        ResetState();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        ButtonSelectionManager.Instance.SelectButton(this);
    }
    public void Select()
    {
        isSelected = true;
        ApplyHoverState();
    }

    public void Deselect()
    {
        isSelected = false;
        if (isHovered)
            ApplyHoverState();
        else
            ResetState();
    }
    private void ApplyHoverState()
    {
        targetScale = originalScale * scaleMultiplier;
        targetPosition = originalPosition + new Vector3(moveOffsetX, 0f, 0f);
    }

    private void ResetState()
    {
        targetScale = originalScale;
        targetPosition = originalPosition;
    }
    private void Update()
    {
        // Smoothly animate scale and position
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * animationSpeed);
    }
}
