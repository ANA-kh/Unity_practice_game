using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private PlayerContro controller;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<PlayerContro>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("speed",Mathf.Abs( rb.velocity.x));
        anim.SetFloat("velocityY", rb.velocity.y);
        anim.SetBool("jump",controller.isJump);
        anim.SetBool("ground",controller.isGround);
        //anim.SetBool("attack",controller.isAttack);
        anim.SetBool("dash",controller.isDash);
        anim.SetBool("climb",controller.isClimb);
        anim.SetBool("sdashpre",controller.isSDashPre);
        anim.SetBool("sdash",controller.isSDash);
    }

    
}
