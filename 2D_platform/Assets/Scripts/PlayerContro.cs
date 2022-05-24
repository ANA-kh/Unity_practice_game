using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class PlayerContro : MonoBehaviour
{
    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask ground;
    public Vector2 checkRadius;
    public LayerMask wall;
    public Transform LWallCheck;
    public Transform RWallCheck;
    private float wallDirection;
    public Vector2 wallCheckRadius;
    public LayerMask trap;


    [Header("State")]
    public bool isClimb;
    public bool isGround;
    public bool inputEnable;
    public bool dead;

    [Header("Attack")]
    public float attackCoolTime;
    private float attackBeginTime;
    public bool isAttack;
    private int comboStep = 1;

    [Header("AttackEffect")]
    public float shakeTime;
    public int lightPause;
    public float lightStrength;


    [Header("Jump")]
    public bool jumpPressed;
    public int jumpCount;
    public bool isJump;
    public float jumpContinue;
    public float jumpForce;

    [Header("Run")]
    public float moveSpeed;

    [Header("Climb")]
    public float climbSpeed;

    [Header("Dash")]
    public float dashSpeed;
    public bool isDash;
    public float dashTime;
    public int dashCount;

    [Header("SupperDash")]
    public float sDashSpeed;
    public bool isSDash;
    private float sDashPreStart;
    public float sDashPreTime;
    public bool isSDashPre;

    //public float sDashTime;



    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D coll;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        jumpCount = 2;
        jumpPressed = false;
        dead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump") && jumpCount > 0)
        {
            jumpPressed = true;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        if (Input.GetKeyUp(KeyCode.L))
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
    }

    private void FixedUpdate()
    {
        isGround = Physics2D.OverlapBox(groundCheck.position, checkRadius, 0, ground);
        if (Physics2D.OverlapBox(LWallCheck.position, wallCheckRadius, 0, wall) && !isGround)
        {
            //wallDirection = 1;
            isClimb = true;
        }
        else if (Physics2D.OverlapBox(RWallCheck.position, wallCheckRadius, 0, wall) && !isGround)
        {
            //wallDirection = -1;
            isClimb = true;
        }
        else
        {
            isClimb = false;
        }
        //isGround = Physics2D.OverlapCircle(groundCheck.position, 0.1f, ground);
        //Physics2D.OverlapBox()
        if (!isDash && !isSDash)
        {
            //Debug.Log("dead"+dead);
            if (coll.IsTouchingLayers(trap) && !dead)
            {
                dead = true;
                Debug.Log("die");
                rb.velocity = Vector2.zero;
                anim.SetTrigger("die");
                ShakeScreen(2);
                return;
            }
            Climb();
            LRMove();
            Jump();
            Attack();

        }
        SupperDash();
        Dash();
    }

    public void Die()
    {
        SceneManager.LoadScene(0);
    }


    public void Climb()
    {
        if (isClimb && !isGround)
        {
            rb.gravityScale = 0;
            float vertical = Input.GetAxis("Vertical");
            //Debug.Log("Vertical" + vertical);
            //float horizontalRaw = Input.GetAxisRaw("Vertical");
            anim.SetFloat("climbspeed", Mathf.Abs(vertical));
            rb.velocity = new Vector2(rb.velocity.x, vertical * climbSpeed);
            jumpCount = 2;

        }
        else
        {
            rb.gravityScale = 1;
        }
    }

    //移动
    public void LRMove()
    {

        float horizontal = Input.GetAxis("Horizontal");
        float horizontalRaw = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);
        if (horizontalRaw != 0)
        {
            transform.localScale = new Vector3(horizontalRaw, 1, 1);
        }
    }

    //跳
    public void Jump()
    {
        if (isGround && rb.velocity.y <= 0)
        {
            isJump = false;
            jumpCount = 2;
            rb.gravityScale = 1;
        }
        if (jumpPressed)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpPressed = false;
            jumpCount--;
            rb.gravityScale = 1;
            isJump = true;
        }
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = 1.4f;
            isJump = false;
        }
        else if (Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * jumpContinue;
        }
    }

    //攻击
    public void Attack()
    {
        if (Input.GetKey(KeyCode.J))
        {
            if (Time.time > attackBeginTime + attackCoolTime)
            {
                isAttack = true;
                attackBeginTime = Time.time;
                anim.SetTrigger("attack");
            }
        }
        if (Input.GetKey(KeyCode.Return))
        {
            if (Time.time > attackBeginTime + attackCoolTime)
            {
                isAttack = true;
                attackBeginTime = Time.time;
                comboStep++;
                if (comboStep > 3)
                    comboStep = 1;
                anim.SetTrigger("LightAttack");
                anim.SetInteger("ComboStep", comboStep);
            }
        }
    }

    //冲刺
    public void Dash()
    {
        if (Input.GetKey(KeyCode.K) && !isDash && !isSDash)
        {
            StartCoroutine(IEDash());
        }
    }

    //超级冲刺
    public void SupperDash()
    {
        if (isClimb)
        {
            if (!isSDashPre && Input.GetKey(KeyCode.L))
            {
                sDashPreStart = Time.time;
                isSDashPre = true;
            }
            else if (Input.GetKey(KeyCode.L))
            {
                if (Time.time > sDashPreStart + sDashPreTime)
                {
                    isSDash = true;
                    isSDashPre = false;
                    isClimb = false;
                }
            }
            else
            {
                isSDashPre = false;
            }

        }

        if (isSDash)
        {
            rb.gravityScale = 0;
            rb.velocity = new Vector2(sDashSpeed * transform.localScale.x * -1, 0);
            if (jumpPressed)
            {
                isSDash = false;
                rb.velocity = new Vector2(0, 0);
                Jump();
            }
            if (Physics2D.OverlapBox(LWallCheck.position, wallCheckRadius, 0, wall) && !isGround
            )
            {
                isSDash = false;
                transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag == "Trap")
        {

            rb.velocity = new Vector2(rb.velocity.x, jumpForce * 1.5f);
            jumpCount = 1;
            dashCount = 1;
            rb.gravityScale = 1;
        }
        else if (other.CompareTag("Enemy"))
        {
            AttackSense.Instance.HitPause(lightPause);
            //AttackSense.Instance.CameraShake(shakeTime, lightStrength);
            ShakeScreen(lightStrength);
            if (transform.localScale.x > 0)
                other.GetComponent<Enemy>().GetHit(Vector2.right);
            else if (transform.localScale.x < 0)
                other.GetComponent<Enemy>().GetHit(Vector2.left);
        }
    }
    public  void ShakeScreen(float strength)
    {
        var ImpulseSource = GetComponent<CinemachineImpulseSource>();
        ImpulseSource.GenerateImpulse(strength);
    }

    IEnumerator IEDash()
    {
        rb.velocity = transform.localScale.x * transform.right * dashSpeed;
        rb.gravityScale = 0;
        isDash = true;
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = 2;
        isDash = false;
        rb.velocity = Vector2.zero;
    }


    private void OnDrawGizmos()//Transform position,float radius
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.position, checkRadius);
        Gizmos.DrawWireCube(LWallCheck.position, wallCheckRadius);
        Gizmos.DrawWireCube(RWallCheck.position, wallCheckRadius);
    }
}
