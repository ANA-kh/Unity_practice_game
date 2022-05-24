using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    EnemyBaseState currentState;

    private GameObject alarmSign;


    public Animator anim;
    public int animState;


    [Header("Base State")]
    public float health = 10;
    public bool isDead;
    public bool hasBomb = false;
    public bool isBoss;

    [Header("Movement")]
    public float speed;
    public Transform pointA, pointB;
    public Transform targetPoint;

    [Header("Attack Setting")]
    public float attackRate;
    public float attackRange, skillRange;
    private float nextAttack = 0;

    public List<Transform> attackList = new List<Transform>();

    public PatrolState patrolState = new PatrolState();
    public AttackState attackState = new AttackState();
    // Start is called before the first frame update
    public virtual void Init()
    {
        anim = GetComponent<Animator>();
        alarmSign = transform.GetChild(0).gameObject;
        
    }

    public void Awake()
    {
        Init();
    }
    void Start()
    {
        GameManager.instance.IsEnemy(this);
        TransitionToState(patrolState);
        if (isBoss)
            UIManager.instance.SetBossHealth(health);
    }

    // Update is called once per frame
    void Update()
    {

        if (isBoss)
            UIManager.instance.UpdateBossHealth(health);
        anim.SetBool("dead", isDead);
        if (isDead)
        {
            GameManager.instance.EnemyDead(this);
            return;
        }
        currentState.OnUpdate(this);
        anim.SetInteger("state", animState);



    }

    public void TransitionToState(EnemyBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }

    public void MoveToTarget()
    {
        if (Mathf.Abs(transform.position.x - targetPoint.position.x) > 0.01f)
            transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);
        FlipDirection();
    }

    public void AttackAction()
    {
        if (Vector2.Distance(transform.position, targetPoint.position) < attackRange)
        {
            if (Time.time > nextAttack)
            {
                anim.SetTrigger("attack");
                nextAttack = Time.time + attackRate;
            }
        }
    }

    public virtual void SkillAction()
    {
        if (Vector2.Distance(transform.position, targetPoint.position) < skillRange)
        {
            if (Time.time > nextAttack)
            {
                anim.SetTrigger("skill");
                nextAttack = Time.time + attackRate;
            }
        }
    }

    public void FlipDirection()
    {
        if (transform.position.x <= targetPoint.position.x)
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        else
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            //Debug.Log(2);
        }
    }

    public void SwichPoint()
    {
        if (Mathf.Abs(pointA.position.x - transform.position.x) > Mathf.Abs(pointB.position.x - transform.position.x))
        {
            targetPoint = pointA;
        }
        else
        {
            targetPoint = pointB;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!attackList.Contains(other.transform) && !hasBomb && !isDead && !GameManager.instance.gameOver)
            attackList.Add(other.transform);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        attackList.Remove(other.transform);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isDead && !GameManager.instance.gameOver)
            StartCoroutine(OnAlarm());
    }

    IEnumerator OnAlarm()
    {
        alarmSign.SetActive(true);
        yield return new WaitForSeconds(alarmSign.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.length);
        alarmSign.SetActive(false);
    }

    public void GetHit(float damage)
    {
        health -= damage;
        if (health < 1)
        {
            health = 0;
            isDead = true;
        }
        anim.SetTrigger("hit");
    }
}
