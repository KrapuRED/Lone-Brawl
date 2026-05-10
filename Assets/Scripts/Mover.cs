using UnityEngine;

public class Mover : MonoBehaviour
{
    private Rigidbody _rb;
    private Vector3 _currentVelocity;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void SetVelocity(Vector3 velocity)
    {
        _currentVelocity = velocity;
    }

    public void AddForce(Vector3 force)
    {
        _currentVelocity += force;
    }

    private void FixedUpdate()
    {
        //keep the original Y, otherwise causes bug on gravity
        _rb.linearVelocity = new Vector3(_currentVelocity.x, _rb.linearVelocity.y, _currentVelocity.z);
    }

    public Vector3 GetVelocity() => _rb.linearVelocity;

    public void Turn(Quaternion turnRotation)
    {
        _rb.MoveRotation(_rb.rotation * turnRotation);
    } 
}
