using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//delegate void NumberChanger(Vector3 n);
public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private CharacterStates characterStates;
    private GameObject attackTarget;
    private float lastAttackTime;
    private bool isDead;
    private float stopDistance;
        //NumberChanger nc;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStates = GetComponent<CharacterStates>();

        stopDistance = agent.stoppingDistance;
    }

    private void Start()
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnEnemyClicked += EventAttack;  //  this指针没报错
        // nc +=MoveToTarget;
        // nc +=EventAttack;  // this 指针报错
        GameManager.Instance.RigisterPlayer(characterStates);
    }



    // Update is called once per frame
    void Update()
    {
        isDead = characterStates.CurrentHealth == 0;
        if (isDead)
            GameManager.Instance.NotifyObservers();

        SwichAnimation();
        lastAttackTime -= Time.deltaTime;
    }

    void SwichAnimation()
    {
        anim.SetFloat("speed", agent.velocity.sqrMagnitude);
        anim.SetBool("Death", isDead);
    }

    public void MoveToTarget(Vector3 target)  //static
    {
        // StopCoroutine("MoveToAttackTarget");
        // if (isDead) return;
        // agent.stoppingDistance = stopDistance;
        // agent.isStopped = false;
        // agent.destination = target;
    }

    private void EventAttack(GameObject target)
    {
        if (isDead) return;
        if (target != null)
        {
            attackTarget = target;
            characterStates.isCritical = UnityEngine.Random.value < characterStates.attackData.criticalChance;
            StartCoroutine("MoveToAttackTarget");
        }
    }

    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;
        agent.stoppingDistance = characterStates.attackData.attackRange;
        transform.LookAt(attackTarget.transform);


        while (Vector3.Distance(attackTarget.transform.position, transform.position) > characterStates.attackData.attackRange)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;
        }
        agent.isStopped = true;

        if (lastAttackTime < 0)
        {

            anim.SetBool("Critical", characterStates.isCritical);
            anim.SetTrigger("attack");

            lastAttackTime = characterStates.attackData.coolDown;
        }
    }

    //Animation Event
    void Hit()
    {
        if (attackTarget.CompareTag("Attackable"))
        {
            if(attackTarget.GetComponent<Rock>() && attackTarget.GetComponent<Rock>().rockStates == Rock.RockStates.HitNothing)
            {
                attackTarget.GetComponent<Rock>().rockStates = Rock.RockStates.HitEnemy;
                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20,ForceMode.Impulse);
            }
        }
        else
        {
            var targetStats = attackTarget.GetComponent<CharacterStates>();
            targetStats.TakeDamage(characterStates, targetStats);
        }
    }
}
