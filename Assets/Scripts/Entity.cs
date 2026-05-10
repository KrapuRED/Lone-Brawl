using UnityEngine;

public class Entity : MonoBehaviour
{
    public Mover _mover;

    public void Awake()
    {
        _mover = GetComponent<Mover>();
    }
}
