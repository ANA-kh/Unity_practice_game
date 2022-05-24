using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates { GUARD, PATROL, CHASE, DEAD }

[RequireComponent(typeof(NavMeshAgent))]  //如果要绑定的物体上没有这个组件，则会自动添加。
[RequireComponent(typeof(CharacterStates))]
public class EnemyController : MonoBehaviour, IEndGameObserver
{
    private Animator anim;
    private EnemyStates enemyStates;
    protected CharacterStates characterStates;
    private NavMeshAgent agent;
    private Collider coll;

    [Header("Basic Setting")]
    public float sightRadius;
    private float speed;
    public bool isGuard;
    protected GameObject attackTarget;
    public float lookAtTime;
    private float remainLookAtTime;
    private float lastAttackTime;
    private Quaternion guardRotation;

    [Header("Patrol State")]
    public float patrolRange;
    private Vector3 wayPoint;
    private Vector3 guardPos;

    private bool playerDead;

    //动画
    bool isWalk;
    bool isFollow;
    bool isChase;
    bool isDead;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStates = GetComponent<CharacterStates>();
        coll = GetComponent<Collider>();
        speed = agent.speed;
        guardPos = transform.position;
        guardRotation = transform.rotation;
        remainLookAtTime = lookAtTime;
    }
    // Start is called before the first frame update
    void Start()
    {
        if (isGuard)
            enemyStates = EnemyStates.GUARD;
        else
        {
            enemyStates = EnemyStates.PATROL;
            GetNewWayPosition();
        }

        //FIXME:场景切换后修改
        GameManager.Instance.AddObserver(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (characterStates.CurrentHealth == 0)
            isDead = true;
        if (!playerDead)
        {
            SwichStates();
            SwichAnimation();
            lastAttackTime -= Time.deltaTime;
        }
    }

    // private void OnEnable()
    // {
    //     GameManager.Instance.AddObserver(this);
    // }

    private void OnDisable()
    {
        if(!GameManager.IsInitialized) return;
        GameManager.Instance.RemoveObserver(this);
    }

    void SwichAnimation()
    {

        anim.SetBool("Death", isDead);
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Critical", characterStates.isCritical);
    }
    void SwichStates()
    {
        if (isDead)
            enemyStates = EnemyStates.DEAD;
        else if (FindPlayer())
        {
            enemyStates = EnemyStates.CHASE;
        }


        switch (enemyStates)
        {
            case EnemyStates.GUARD:
                isChase = false;

                if (transform.position != guardPos)
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPos;

                    if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
                    {
                        isWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.05f);
                    }
                }
                break;
            case EnemyStates.PATROL:
                isChase = false;
                agent.speed = speed * 0.5f;
                if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
                {
                    isWalk = false;
                    if (remainLookAtTime > 0)
                        remainLookAtTime -= Time.deltaTime;
                    else
                        GetNewWayPosition();
                }
                else
                {
                    isWalk = true;
                    agent.destination = wayPoint;
                }
                break;
            case EnemyStates.CHASE:
                agent.speed = speed;
                isWalk = false;
                isChase = true;

                if (!FindPlayer())
                {
                    isFollow = false;
                    if (remainLookAtTime > 0)
                    {
                        remainLookAtTime -= Time.deltaTime;
                        agent.destination = transform.position;
                    }
                    else if (isGuard)
                        enemyStates = EnemyStates.GUARD;
                    else
                        enemyStates = EnemyStates.PATROL;
                }
                else
                {
                    isFollow = true;
                    agent.isStopped = false;
                    agent.destination = attackTarget.transform.position;
                }
                //攻击范围内则攻击
                if (TargetInAttackRange() || TargetInSkillRange())
                {
                    isFollow = false;
                    agent.isStopped = true;

                    if (lastAttackTime < 0)
                    {
                        lastAttackTime = characterStates.attackData.coolDown;

                        characterStates.isCritical = Random.value < characterStates.attackData.criticalChance;

                        Attack();
                    }
                }
                break;
            case EnemyStates.DEAD:
                coll.enabled = false;
                //agent.enabled = false;
                agent.radius =0;
                Destroy(gameObject, 2f);
                break;
        }
    }

    void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if (TargetInAttackRange())
        {
            anim.SetTrigger("Attack");
        }
        if (TargetInSkillRange())
        {
            anim.SetTrigger("Skill");
        }
    }

    bool TargetInAttackRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStates.attackData.attackRange;
        else
            return false;
    }

    bool TargetInSkillRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStates.attackData.skillRange;
        else
            return false;
    }
    bool FindPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }

    void GetNewWayPosition()
    {
        remainLookAtTime = lookAtTime;
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);
        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y, guardPos.z + randomZ);
        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }

    void Hit()
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStates>();
            targetStats.TakeDamage(characterStates, targetStats);
        }
    }

    public void EndNotify()
    {
        playerDead = true;
        anim.SetBool("Win", true);
        isChase = false;
        isWalk = false;
        attackTarget = null;
        SwichAnimation();
    }
}
