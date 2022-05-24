using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private Animator anim;
    private Collider2D coll;
    private Rigidbody2D rb;
    public float startTime;
    public float waitTime;
    public float bombForce;

    [Header("Check")]
    public float radius;
    public LayerMask targetLayer;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        startTime = Time.time;
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!anim.GetCurrentAnimatorStateInfo(0).IsName("bomb_off"))
        {
            if (Time.time > startTime + waitTime)
            {
                anim.Play("bomb_onn");
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public void Explotion()//Animation event
    {
        coll.enabled = false;
        rb.gravityScale = 0;
        Collider2D[] aroundObjects = Physics2D.OverlapCircleAll(transform.position, radius, targetLayer);
        foreach (var item in aroundObjects)
        {
            Vector3 pos = transform.position - item.transform.position;

            item.GetComponent<Rigidbody2D>().AddForce((-pos + Vector3.up) * bombForce, ForceMode2D.Impulse);
            if(item.CompareTag("Bomb") && item.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("bomb_off"))
            {
                item.GetComponent<Bomb>().TurnOn();
            }
            if(item.CompareTag("Player"))
                item.GetComponent<IDamageable>().GetHit(3);
        }
    }

    public void DestroyThis()
    {
        Destroy(gameObject);
    }

    public void TurnOff()
    {
        anim.Play("bomb_off");
        gameObject.layer = LayerMask.NameToLayer("NPC");
    }

    public void TurnOn()
    {
        startTime = Time.time;
        anim.Play("bomb");
        gameObject.layer = LayerMask.NameToLayer("Bomb");
    }
}
