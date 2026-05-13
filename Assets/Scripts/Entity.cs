using UnityEngine;

public class Entity : MonoBehaviour
{
    protected Mover _mover;

    protected virtual void Awake()
    {
        _mover = GetComponent<Mover>();
    }
}
