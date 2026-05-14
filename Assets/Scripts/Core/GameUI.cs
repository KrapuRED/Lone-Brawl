using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance { get; private set; }

    [Header("HUD")]
    public TextMeshProUGUI TxtOpenNodes;
    public TextMeshProUGUI TxtClosedNodes;
    public TextMeshProUGUI TxtEnemyCount;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void UpdateHUD(int open, int closed, int enemies)
    {
        if (TxtOpenNodes   != null) TxtOpenNodes.text   = $"Open: {open}";
        if (TxtClosedNodes != null) TxtClosedNodes.text = $"Closed: {closed}";
        if (TxtEnemyCount  != null) TxtEnemyCount.text  = $"Enemies: {enemies}";
    }
}
