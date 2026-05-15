using System;
using System.Security;
using UnityEngine;
using UnityEngine.Animations;
public class Entity : MonoBehaviour, IDamageable
{
    private float _maxHP = 100f;
    private float _currentHP;

    protected Mover _mover;

    public event Action<float, float> OnHealthChanged;

    // protected = only child can call. virtual = child can override this function
    protected virtual void Awake()
    {
        _mover = GetComponent<Mover>();
    }

    private void Start()
    {
        _currentHP = _maxHP;
    }

    public void TakeDamage(float amount)
    {
        _currentHP -= amount;
        OnHealthChanged?.Invoke(_currentHP, _maxHP);

        if(_currentHP <= 0)
        {
            die();
        }

    }

    public void SetMaxHP(float amount)
    {
        _maxHP = amount;
    }

    protected virtual void die()
    {
        Destroy(gameObject);
    }

}
