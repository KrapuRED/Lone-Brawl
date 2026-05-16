using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeState { Open, Closed, Opening }

public class HexCell : MonoBehaviour
{
    [Header("Coordinates (Axial)")]
    public int Q;
    public int R;

    [Header("State")]
    public NodeState CurrentState = NodeState.Closed;

    [Header("Visual Meshes")]
    public GameObject MeshOpen;
    public GameObject MeshClosed;
    public GameObject MeshOpening;

    [Header("Blocking")]
    [Tooltip("Kosongkan — otomatis ambil Collider dari MeshClosed.")]
    public Collider BlockerCollider;

    [Header("Enemy Config")]
    [Range(0, 5)]
    public int EnemyCount = 1;
    [HideInInspector] public GameObject EnemyPrefab;

    // Runtime
    [HideInInspector] public List<Enemy> Enemies = new List<Enemy>();

    private bool _animating = false;
    private bool _enemiesSpawned = false;

    // -------------------------------------------------------
    void Awake()
    {
        if (BlockerCollider == null && MeshClosed != null)
            BlockerCollider = MeshClosed.GetComponent<Collider>();
    }

    void Start()
    {
        ApplyVisualState();
    }

    public void SetState(NodeState newState)
    {
        CurrentState = newState;
        ApplyVisualState();
    }

    /// <summary>
    /// Spawn enemy setelah node ini terbuka.
    /// Dipanggil oleh HexGrid.OnCellOpened() — bukan saat Start.
    /// </summary>
    public void SpawnEnemiesOnOpen()
    {
        if (_enemiesSpawned) return;
        if (EnemyCount == 0) return;
        if (EnemyPrefab == null)
        {
            Debug.LogWarning($"[HexCell ({Q},{R})] EnemyPrefab not assigned!");
            return;
        }

        _enemiesSpawned = true;

        for (int i = 0; i < EnemyCount; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * 0.5f;
            Vector3 spawnPos = transform.position + new Vector3(randomOffset.x, 0.6f, randomOffset.y);

            GameObject go = Instantiate(EnemyPrefab, spawnPos, Quaternion.identity, transform);
            go.name = $"Enemy_{Q}_{R}_{i}";

            Enemy enemy = go.GetComponent<Enemy>();
            if (enemy == null)
            {
                Debug.LogError($"[HexCell] EnemyPrefab tidak punya EnemyStateMachine!");
                continue;
            }

            //enemy.OwnerCell = this;
            Enemies.Add(enemy);
        }

        Debug.Log($"[HexCell ({Q},{R})] Spawned {EnemyCount} enemies after opening.");
    }

    /// Dipanggil oleh EnemyStateMachine saat enemy mati.
    public void OnEnemyDied(Enemy enemy)
    {
        Enemies.Remove(enemy);
        Debug.Log($"[HexCell ({Q},{R})] Enemy died. Remaining: {Enemies.Count}");

        // Beritahu HexGrid bahwa ada enemy mati di cell ini
        // HexGrid yang memutuskan apakah seluruh open area sudah clear
        HexGrid.Instance?.OnEnemyKilledInCell(this);
    }

    /// Mulai animasi terbuka. Dipanggil oleh HexGrid.
    public void TriggerOpen()
    {
        if (_animating) return;
        if (CurrentState != NodeState.Closed) return;
        _animating = true;
        SetState(NodeState.Opening);
        StartCoroutine(OpenAnimationCoroutine());
    }

    private IEnumerator OpenAnimationCoroutine()
    {
        float duration = 0.85f;
        float elapsed = 0f;

        if (MeshOpening != null) MeshOpening.SetActive(true);
        if (MeshClosed != null) MeshClosed.SetActive(false);
        if (MeshOpen != null) MeshOpen.SetActive(false);

        Quaternion startRot = transform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0f, 180f, 0f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            transform.rotation = Quaternion.Lerp(startRot, endRot, t);
            yield return null;
        }

        transform.rotation = endRot;
        _animating = false;

        // Selesai animasi → open
        SetState(NodeState.Open);

        // Beritahu HexGrid → spawn enemy di node ini + cek chain
        HexGrid.Instance?.OnCellOpened(this);

        Debug.Log($"[HexCell ({Q},{R})] Now OPEN.");
    }

    private void ApplyVisualState()
    {
        bool isOpen = CurrentState == NodeState.Open;
        bool isClosed = CurrentState == NodeState.Closed;
        bool isOpening = CurrentState == NodeState.Opening;

        if (MeshOpen != null) MeshOpen.SetActive(isOpen);
        if (MeshClosed != null) MeshClosed.SetActive(isClosed);
        if (MeshOpening != null) MeshOpening.SetActive(isOpening);

        // Blocker: aktif hanya saat Closed
        if (BlockerCollider != null)
            BlockerCollider.enabled = isClosed;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = CurrentState == NodeState.Open ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.1f, 0.3f);
        UnityEditor.Handles.Label(transform.position + Vector3.up * 1.2f, $"({Q},{R})\n{CurrentState}");
    }
#endif
}