using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Test UI")]
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void SetStatus(string s)
    {
        if (statusText) statusText.text = s;
    }

    public void SetScore(int player, int ai)
    {
        if (scoreText) scoreText.text = $"Speler: {player} AI: {ai}";
    }
}
