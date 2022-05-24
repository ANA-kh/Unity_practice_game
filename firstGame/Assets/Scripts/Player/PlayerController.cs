using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable
{
    private Rigidbody2D rb;
    private Animator anim;
    


    public float speed;
    public float jumpForce;

    [Header("Player State")]
    public float health;
    public bool isDead;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask groudLayer;

    [Header("States Check")]
    public bool isGround;
    public bool isJump;
    public bool canJump;

    [Header("Jump Fx")]
    public GameObject jumpFX;
    public GameObject landFX;

    [Header("Attack Setting")]
    public GameObject bombPrefab;
    public float nextAttack = 0;
    public float attackRate;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        
        canJump = false;

        health = GameManager.instance.LoadHealth();

        GameManager.instance.IsPlayer(this);
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("dead", isDead);
        if (isDead)
            return;
        CheckInput();

    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        PhysicsCheck();
        MoveMent();
        jump();
    }

    void CheckInput()
    {
        if (Input.GetButtonDown("Jump") && isGround)
        {
            canJump = true;
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            Attack();
        }
        
        
        if(Input.GetKey(KeyCode.S))
        {
            gameObject.layer = LayerMask.NameToLayer("PlayerDown");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Player");
        }
    }

    void MoveMent()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");//-1 0 1 不包含小数
        //float horizontalInput = Input.GetAxis("Horizontal");//-1 ~ 1

        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);

        if (horizontalInput != 0)
        {
            transform.localScale = new Vector3(horizontalInput, 1, 1);
        }
    }

    void jump()
    {
        if (canJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            canJump = false;
            rb.gravityScale = 3;
            isJump = true;
            jumpFX.SetActive(true);
            jumpFX.transform.position = transform.position + new Vector3(0, -0.45f, 0);
        }
    }

    public void Attack()
    {
        if (Time.time > nextAttack)
        {
            Instantiate(bombPrefab, transform.position, bombPrefab.transform.rotation);

            nextAttack = Time.time + attackRate;
        }
    }

    void PhysicsCheck()
    {
        isGround = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groudLayer);
        if (isGround)
        {
            rb.gravityScale = 3;
            isJump = false;
        }
    }

    public void LandFX()//Animation event
    {
        landFX.SetActive(true);
        landFX.transform.position = transform.position + new Vector3(0, -0.75f, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }

    public void GetHit(float damage)
    {
        if (!anim.GetCurrentAnimatorStateInfo(1).IsName("player_hit"))
        {
            health -= damage;

            if (health < 1)
            {
                health = 0;
                isDead = true;
            }
            anim.SetTrigger("hit");

            UIManager.instance.UpdateHealth(health);
        }
    }

}
