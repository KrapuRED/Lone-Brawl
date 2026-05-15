using System;
using Unity.VisualScripting;
using UnityEngine;

public class Player : Entity
{
    [SerializeField]
    private Transform hole; 
    public GameObject projectile;

    public float playerHP = 100f;
    public float movementSpeed = 85f;
    public float turnSpeed = 35f;
    private Vector3 _movementInput;
    private float _turnInputValue;
    private float _mouseInputX;
    private Transform _turretRotation;

    private void Start()
    {
        SetMaxHP(playerHP);

        Cursor.lockState = CursorLockMode.Locked;

        _turretRotation = transform.Find("Head").transform;
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
        // on left click
        if(Input.GetMouseButtonDown(0))
        {
            // Debug.Log("Shoot!");
            Instantiate(projectile, hole.position, hole.rotation);
        }
        


    }

    private void FixedUpdate()
    {
        // player move
        Vector3 newVelocity = _movementInput * -movementSpeed * Time.fixedDeltaTime;
        _mover.SetVelocity(newVelocity);

        // player rotate
        float turn = _turnInputValue * turnSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        _mover.Turn(turnRotation);

        // turret rotate
        _turretRotation.Rotate(Vector3.up * _mouseInputX * 100 * Time.fixedDeltaTime);
    }



}
