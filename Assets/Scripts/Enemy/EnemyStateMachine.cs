using System.Security;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class EnemyStateMachine : Entity
{
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

    protected override void Awake()
    {
        base.Awake();

        _player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
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
        Vector3 directionToPlayer = (_player.position - transform.position).normalized;
        directionToPlayer.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, deltaTime);

        agent.SetDestination(_player.position);
       // Debug.Log("Chasing Player!");
    }

    private void AttackPlayer(float deltaTime)
    {
        agent.SetDestination(transform.position);
        
        

        if(!_alreadyAttack)
        {
            // attack code
            Debug.Log("Attack!!!!!");

            _alreadyAttack = true;
            _onRecoil = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
            Invoke(nameof(ResetRecoil), recoilTime);
        }
        
        if(!_onRecoil)
        {
            // turn off auto rotation for manual rotation
            if(agent.isActiveAndEnabled)
            {
                agent.updateRotation = false;
            }

            // point to player to attack
            Vector3 directionToPlayer = (_player.position - transform.position).normalized;
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
}
