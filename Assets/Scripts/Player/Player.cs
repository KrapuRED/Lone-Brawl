using UnityEngine;

public class Player : Entity
{
    public float movementSpeed = 5f;
    private Vector3 _movementInput;

    public void Update()
    {
        float movementX = Input.GetAxisRaw("Horizontal");
        float movementZ = Input.GetAxisRaw("Vertical");

        _movementInput = new Vector3(movementX, 0, movementZ).normalized;
    }

    public void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        Vector3 newVelocity = _movementInput * movementSpeed;
        _mover.SetVelocity(newVelocity);
    }

}
