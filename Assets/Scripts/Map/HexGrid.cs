using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manager utama hex map.
/// 
/// FLOW:
/// 1. Map generate → semua open node langsung punya enemy (spawn langsung)
/// 2. Player kalahkan semua enemy di open area
/// 3. Semua closed node yang BERBATASAN dengan open area → TriggerOpen()
/// 4. Node terbuka satu per satu (animasi rotate)
/// 5. Setelah terbuka → spawn enemy baru di node itu
/// 6. Ulangi
/// </summary>
public class HexGrid : MonoBehaviour
{
    public static HexGrid Instance { get; private set; }

    [Header("Map Data")]
    public HexMapData MapData;

    [Header("Prefabs")]
    public GameObject HexCellPrefab;
    public GameObject EnemyPrefab;

    [Header("Grid Settings")]
    public float HexSize = 1.0f;
    public float NodeRotationY = 0f;

    // Semua cells
    private Dictionary<Vector2Int, HexCell> _cells = new Dictionary<Vector2Int, HexCell>();

    // -------------------------------------------------------
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        GenerateMap();
    }

    // -------------------------------------------------------
    // Map Generation
    // -------------------------------------------------------

    private void GenerateMap()
    {
        if (MapData == null) { Debug.LogError("[HexGrid] MapData not assigned!"); return; }
        if (HexCellPrefab == null) { Debug.LogError("[HexGrid] HexCellPrefab not assigned!"); return; }

        foreach (var data in MapData.Cells)
            SpawnCell(data);

        // Spawn enemy HANYA di open node saat awal
        // (closed node akan spawn enemy setelah terbuka)
        foreach (var cell in _cells.Values)
        {
            if (cell.CurrentState == NodeState.Open && cell.EnemyCount > 0)
                cell.SpawnEnemiesOnOpen();
        }

        Debug.Log($"[HexGrid] Map selesai: {_cells.Count} nodes.");
        UpdateUI();
    }

    private void SpawnCell(HexCellData data)
    {
        Vector3 worldPos = AxialToWorld(data.Q, data.R);
        Quaternion spawnRot = Quaternion.Euler(0f, NodeRotationY, 0f);
        GameObject go = Instantiate(HexCellPrefab, worldPos, spawnRot, transform);
        go.name = $"HexNode({data.Q},{data.R})";

        HexCell cell = go.GetComponent<HexCell>();
        if (cell == null) { Debug.LogError("[HexGrid] HexCellPrefab missing HexCell!"); return; }

        cell.Q = data.Q;
        cell.R = data.R;
        cell.EnemyCount = data.EnemyCount;
        cell.EnemyPrefab = EnemyPrefab;
        cell.SetState(data.InitialState);

        _cells[new Vector2Int(data.Q, data.R)] = cell;
    }

    // -------------------------------------------------------
    // Public API — dipanggil oleh HexCell
    // -------------------------------------------------------

    /// <summary>
    /// Dipanggil setiap kali enemy mati di sebuah cell.
    /// Cek apakah SELURUH open area sudah clear → buka closed border.
    /// </summary>
    public void OnEnemyKilledInCell(HexCell cell)
    {
        if (!IsOpenAreaClear()) return;

        Debug.Log("[HexGrid] Open area CLEAR! Membuka closed border nodes...");
        OpenBorderClosedNodes();
        UpdateUI();
    }

    /// <summary>
    /// Dipanggil oleh HexCell setelah animasi selesai (node jadi Open).
    /// Spawn enemy di node baru, lalu cek lagi apakah ada closed neighbor tanpa enemy.
    /// </summary>
    public void OnCellOpened(HexCell openedCell)
    {
        // Spawn enemy di node yang baru terbuka
        openedCell.SpawnEnemiesOnOpen();

        // Chain: closed neighbor tanpa enemy config → auto open langsung
        foreach (var coord in GetNeighborCoords(openedCell.Q, openedCell.R))
        {
            HexCell neighbor = GetCell(coord.x, coord.y);
            if (neighbor == null) continue;
            if (neighbor.CurrentState != NodeState.Closed) continue;
            if (neighbor.EnemyCount == 0)
            {
                Debug.Log($"[HexGrid] Chain: auto-open ({neighbor.Q},{neighbor.R}) — no enemies");
                neighbor.TriggerOpen();
            }
        }

        UpdateUI();
    }

    // -------------------------------------------------------
    // Internal Logic
    // -------------------------------------------------------

    /// <summary>
    /// Cek apakah semua enemy di seluruh open node sudah mati.
    /// </summary>
    private bool IsOpenAreaClear()
    {
        foreach (var cell in _cells.Values)
        {
            if (cell.CurrentState != NodeState.Open) continue;
            if (cell.Enemies.Count > 0) return false;
        }
        return true;
    }

    /// <summary>
    /// Buka semua closed node yang berbatasan langsung dengan open area.
    /// </summary>
    private void OpenBorderClosedNodes()
    {
        var toOpen = new List<HexCell>();

        foreach (var cell in _cells.Values)
        {
            if (cell.CurrentState != NodeState.Closed) continue;
            if (cell.EnemyCount == 0) continue; // wall/border tanpa enemy, skip dulu

            // Cek apakah ada open neighbor
            bool adjToOpen = GetNeighborCoords(cell.Q, cell.R)
                .Any(coord => {
                    var n = GetCell(coord.x, coord.y);
                    return n != null && n.CurrentState == NodeState.Open;
                });

            if (adjToOpen)
                toOpen.Add(cell);
        }

        foreach (var cell in toOpen)
            cell.TriggerOpen();

        if (toOpen.Count == 0)
            Debug.Log("[HexGrid] Tidak ada closed border node — map mungkin sudah habis.");
    }

    // -------------------------------------------------------
    // Helpers
    // -------------------------------------------------------

    public HexCell GetCell(int q, int r)
    {
        _cells.TryGetValue(new Vector2Int(q, r), out HexCell cell);
        return cell;
    }

    public Vector3 AxialToWorld(int q, int r)
    {
        float x = HexSize * 1.5f * q;
        float z = HexSize * Mathf.Sqrt(3f) * (r + q * 0.5f);
        return new Vector3(x, 0f, z);
    }

    public Vector2Int WorldToAxial(Vector3 worldPos)
    {
        float q = (2f / 3f * worldPos.x) / HexSize;
        float r = (-1f / 3f * worldPos.x + Mathf.Sqrt(3f) / 3f * worldPos.z) / HexSize;
        return AxialRound(q, r);
    }

    public static List<Vector2Int> GetNeighborCoords(int q, int r)
    {
        return new List<Vector2Int>
        {
            new Vector2Int(q+1, r),   new Vector2Int(q-1, r),
            new Vector2Int(q,   r+1), new Vector2Int(q,   r-1),
            new Vector2Int(q+1, r-1), new Vector2Int(q-1, r+1),
        };
    }

    private Vector2Int AxialRound(float q, float r)
    {
        float s = -q - r;
        int rq = Mathf.RoundToInt(q), rr = Mathf.RoundToInt(r), rs = Mathf.RoundToInt(s);
        float dq = Mathf.Abs(rq - q), dr = Mathf.Abs(rr - r), ds = Mathf.Abs(rs - s);
        if (dq > dr && dq > ds) rq = -rr - rs;
        else if (dr > ds) rr = -rq - rs;
        return new Vector2Int(rq, rr);
    }

    private void UpdateUI()
    {
        if (GameUI.Instance == null) return;
        int open = 0, closed = 0, enemies = 0;
        foreach (var cell in _cells.Values)
        {
            if (cell.CurrentState == NodeState.Open) open++;
            else if (cell.CurrentState == NodeState.Closed) closed++;
            enemies += cell.Enemies.Count;
        }
        GameUI.Instance.UpdateHUD(open, closed, enemies);
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (MapData == null) return;
        foreach (var cell in MapData.Cells)
        {
            Vector3 pos = AxialToWorld(cell.Q, cell.R);
            Gizmos.color = cell.InitialState == NodeState.Open
                ? new Color(0, 1, 0, 0.4f)
                : new Color(1, 0, 0, 0.4f);
            Gizmos.DrawSphere(pos, 0.18f);
            UnityEditor.Handles.Label(pos + Vector3.up * 0.5f, $"({cell.Q},{cell.R})");
        }
    }
#endif
}