using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance {  get; private set; }

    // Button Binding
    [System.Serializable]
    public class ButtonBinding
    {
        public int ID;
        public Button button;
        public TextMeshProUGUI remainingText; // Optional, can be null
    }

    [SerializeField] private List<ButtonBinding> bindings;

    private Dictionary<int, ButtonBinding> map;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        map = new Dictionary<int, ButtonBinding>();
        foreach (var b in bindings)
        {
            if (b != null && b.button != null)
            {
                map[b.ID] = b;
            }
        }
    }
    // call this once at startup to set initial UI state
    public void InitializeButtons(ObjectsDatabase db)
    {
        foreach (var d in db.objectsData)
            UpdatePlacementState(d.ID, 0, d.maxPlacements);
    }

    public void UpdatePlacementState(int id, int placedCount, int max)
    {
        if (!map.TryGetValue(id, out var bind)) return;

        bool canPlace = placedCount < max;
        bind.button.interactable = canPlace;

        // Darken the button
        var img = bind.button.GetComponent<Image>();
        if (img) img.color = canPlace ? Color.white : new Color(0.65f, 0.65f, 0.65f, 1f);

        if (bind.remainingText)
            bind.remainingText.text = $"{Mathf.Max(0, max - placedCount)}";
    }
}