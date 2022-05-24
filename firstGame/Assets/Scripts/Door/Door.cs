using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    Animator anim;
    BoxCollider2D coll;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();

        coll.enabled = false;
        GameManager.instance.IsDoor(this);
    }

    public void OpenDoor() // Game Manager 调用
    {
        anim.Play("open");
        coll.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            GameManager.instance.NextLevel();
        }
    }
}
