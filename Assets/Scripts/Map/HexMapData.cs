using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HexMapData", menuName = "HexMap/Map Data")]
public class HexMapData : ScriptableObject
{
    public List<HexCellData> Cells = new List<HexCellData>();
}

[System.Serializable]
public class HexCellData
{
    public int Q;
    public int R;
    public NodeState InitialState = NodeState.Closed;
    [Range(0, 5)]
    public int EnemyCount = 1;
}
