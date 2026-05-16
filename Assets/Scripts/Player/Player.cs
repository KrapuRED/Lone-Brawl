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
    private Quaternion _targetTurretRotation;
    private Transform _turretRotation;

    private Camera _mainCam;

    protected override void Awake()
    {
        base.Awake();

        SetMaxHP(playerHP);

        Cursor.lockState = CursorLockMode.Confined;
        _turretRotation = transform.Find("Head").transform;

    }

    protected override void Start()
    {
        _mainCam = Camera.main;
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
        Plane groundPlane = new Plane(Vector3.up, _turretRotation.position);
        Ray ray = _mainCam.ScreenPointToRay(Input.mousePosition);

        if(groundPlane.Raycast(ray, out float hitDistance))
        {
            Vector3 hitPoint = ray.GetPoint(hitDistance);
            Vector3 aimDirection = _turretRotation.position - hitPoint;
            aimDirection.y = 0f;

            if(aimDirection != Vector3.zero)
            {
                _targetTurretRotation = Quaternion.LookRotation(aimDirection);
            }
        }

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
        _turretRotation.rotation = _targetTurretRotation;
    }

    protected override void die()
    {
        gameObject.SetActive(false);
        GameOverManager.Instance.ShowGameOver();
    }

}
