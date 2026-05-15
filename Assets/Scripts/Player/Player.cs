using System;
using Unity.VisualScripting;
using UnityEngine;

public class Player : Entity
{
    [SerializeField]
    private Transform hole; 
    public GameObject projectile;

    public float playerHP;
    public float movementSpeed = 85f;
    public float turnSpeed = 35f;
    private Vector3 _movementInput;
    private float _turnInputValue;
    private float _targetTurretAngle;
    private Transform _turretRotation;


    protected override void Start()
    {
        SetMaxHP(playerHP);

        Cursor.lockState = CursorLockMode.Confined;

        _turretRotation = transform.Find("Head").transform;
    }

    private void Update()
    {
        // float x = GetCurrentHP();
        // Debug.Log(x);


        // move input
        float movementInputValue = Input.GetAxisRaw("Vertical");
        _movementInput = transform.forward * movementInputValue;

        // rotate input
        _turnInputValue = Input.GetAxisRaw("Horizontal");

        // mouse input
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector3 mousePosition = Input.mousePosition;

        float deltaX = mousePosition.x - screenCenter.x;
        float deltaY = mousePosition.y - screenCenter.y;

        _targetTurretAngle = Mathf.Atan2(deltaX, deltaY) * Mathf.Rad2Deg;

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
        _turretRotation.localRotation = Quaternion.Euler(0f, _targetTurretAngle, 0f);
    }



}
