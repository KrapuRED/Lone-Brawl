using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private GameObject hitParticle;
    [SerializeField]
    private float speed = 50f;
    [SerializeField]
    private float damage = 100f;
    private float _lockedYRotation;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _lockedYRotation = transform.eulerAngles.y;
        Vector3 Velocity = (transform.forward * speed);
        _rb.linearVelocity = Velocity;
    }

    private void FixedUpdate()
    {
        Quaternion targetRotation = Quaternion.LookRotation(_rb.linearVelocity);
        transform.rotation = Quaternion.Euler(targetRotation.eulerAngles.x, _lockedYRotation, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();

        if(damageable != null)
        {
            damageable.TakeDamage(damage);
        }

         // toggle particle
        if(hitParticle != null)
        {
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            Instantiate(hitParticle, hitPoint, Quaternion.identity);
        }

        Destroy(gameObject);
    }


}
