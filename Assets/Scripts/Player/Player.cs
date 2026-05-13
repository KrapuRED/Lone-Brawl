using System;
using UnityEngine;

public class Player : Entity
{
    public float movementSpeed = 85f;
    public float turnSpeed = 35f;
    private Vector3 _movementInput;
    private float _turnInputValue;
    private float _mouseInputX;
    private Transform _turretRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        _turretRotation = transform.Find("Turret").transform;
    }

    private void Update()
    {
        // move input
        float movementInputValue = Input.GetAxisRaw("Vertical");
        _movementInput = transform.forward * movementInputValue;

        // rotate input
        _turnInputValue = Input.GetAxisRaw("Horizontal");

        // mouse input
        _mouseInputX = Input.GetAxisRaw("Mouse X");
        

    }

    private void FixedUpdate()
    {
        // player move
        Vector3 newVelocity = _movementInput * movementSpeed * Time.fixedDeltaTime;
        _mover.SetVelocity(newVelocity);

        // player rotate
        float turn = _turnInputValue * turnSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        _mover.Turn(turnRotation);

        // turret rotate
        _turretRotation.Rotate(Vector3.up * _mouseInputX * 100 * Time.fixedDeltaTime);
    }



}
