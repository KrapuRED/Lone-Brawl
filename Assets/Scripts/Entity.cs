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

    protected virtual void Start()
    {
        _currentHP = _maxHP;
    }

    public void TakeDamage(float amount)
    {
        Debug.Log(amount);
        _currentHP -= amount;
        
        // let it out loud
        OnHealthChanged?.Invoke(_currentHP, _maxHP);

        if(_currentHP <= 0)
        {
            die();
        }

    }

    public void SetMaxHP(float amount)
    {
        _maxHP = amount;
        _currentHP = _maxHP;
    }

    protected virtual void die()
    {
        Destroy(gameObject);
    }

    public float GetCurrentHP()
    {
        return _currentHP;
    }

}
