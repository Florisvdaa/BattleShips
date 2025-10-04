using UnityEngine;

public class ButtonSelectionManager : MonoBehaviour
{
    public static ButtonSelectionManager Instance { get; private set; }

    private ButtonHoverFeedback currentHoverButton;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public void SelectButton(ButtonHoverFeedback button)
    {
        if(currentHoverButton == button)
            return;

        if (currentHoverButton != null)
            currentHoverButton.Deselect();

        currentHoverButton = button;
        currentHoverButton.Select();

    }
}
