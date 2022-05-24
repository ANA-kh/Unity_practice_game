using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rock : MonoBehaviour
{
    private Rigidbody rb;
    public enum RockStates { HitPlayer, HitEnemy, HitNothing }
    public RockStates rockStates;
    [Header("Basic Setting")]
    public float force;
    public int damage;
    public GameObject target;
    private Vector3 direction;
    public GameObject breakEffect;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one;
        rockStates = RockStates.HitPlayer;
        FlyToTarget();
    }

    private void FixedUpdate()
    {
        if(rb.velocity.sqrMagnitude<1f)
        {
            rockStates = RockStates.HitNothing;
        }
    }
    public void FlyToTarget()
    {
        direction = (target.transform.position - transform.position + Vector3.up).normalized;
        rb.AddForce(direction * force, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other)
    {
        switch (rockStates)
        {
            case RockStates.HitPlayer:
                if (other.gameObject.CompareTag("Player"))
                {
                    other.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    other.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;
                    other.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");
                    other.gameObject.GetComponent<CharacterStates>().TakeDamage(damage, other.gameObject.GetComponent<CharacterStates>());
                    rockStates = RockStates.HitNothing;
                }
                break;
            case RockStates.HitEnemy:
                if(other.gameObject.GetComponent<Golem>())
                {
                    var otherStats = other.gameObject.GetComponent<CharacterStates>();
                    otherStats.TakeDamage(damage,otherStats);
                    Instantiate(breakEffect,transform.position,Quaternion.identity);
                    Destroy(gameObject);
                }
                break;
        }
    }

}