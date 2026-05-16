using System;
using System.Security;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class Enemy : Entity
{
    [SerializeField]
    private Transform hole; 
    public GameObject projectile;
    public float enemyHP = 100f;

    public NavMeshAgent agent;
    private Transform _player;
    public LayerMask whatIsGround, whatIsPlayer;

    // attacking
    public float recoilTime;
    public float timeBetweenAttacks;
    private bool _alreadyAttack;
    private bool _onRecoil;

    // property for multiple conditions
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    private AudioSource audioSource;

    // broadcast on die
    public static event Action<int> OnEnemyDeath;

    protected override void Awake()
    {
        base.Awake();

        _player = GameObject.Find("Player").transform;

        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
    }

    protected override void Start()
    {
        SetMaxHP(enemyHP);

        agent.updateRotation = false;
    }

    private void Update()
    {
        
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if(!playerInSightRange && !playerInAttackRange) HoldMovement();
        if(playerInSightRange && !playerInAttackRange) ChasePlayer(Time.deltaTime);
        if(playerInSightRange && playerInAttackRange) AttackPlayer(Time.deltaTime);

    }

    private void HoldMovement()
    {
        agent.SetDestination(transform.position);
        // Debug.Log("Stop movement");
    }

    private void ChasePlayer(float deltaTime)
    {
        // smooth rotation
        Vector3 directionToPlayer = (transform.position - _player.position).normalized;
        directionToPlayer.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, deltaTime);

        // Debug.Log(_player.position);
        agent.SetDestination(_player.position);
       // Debug.Log("Chasing Player!");
    }

    private void AttackPlayer(float deltaTime)
    {
        agent.SetDestination(transform.position);

        if(!_alreadyAttack)
        {
            audioSource.Play();
            Instantiate(projectile, hole.position, hole.rotation);
            // Debug.Log("Attack!!!!!");

            _alreadyAttack = true;
            _onRecoil = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
            Invoke(nameof(ResetRecoil), recoilTime);
        }
        
        if(!_onRecoil)
        {
            // point to player to attack
            Vector3 directionToPlayer =(transform.position - _player.position).normalized;
            directionToPlayer.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, deltaTime * 5);

        }

       // Debug.Log("Trying to attack Player!");
    }

    private void ResetAttack()
    {
        _alreadyAttack = false;
    }

    private void ResetRecoil()
    {
        _onRecoil = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    protected override void die()
    {
        SetMaxHP(enemyHP);
        // turn off but keep this player for the future use
        EnemyPool.Instance.ReturnObject(this.gameObject);

        // let it out load
        OnEnemyDeath?.Invoke(100);

    }
}
