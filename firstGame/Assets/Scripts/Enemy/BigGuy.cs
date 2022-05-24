using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigGuy : Enemy
{
    public Transform pickupPoint;
    public int power;
    
    public override void SkillAction()
    {
        base.SkillAction();
        //AttackAction();
    }

    public void PickupBomb()//Animation event
    {
        if(targetPoint.CompareTag("Bomb") && !hasBomb)
        {
            targetPoint.gameObject.transform.position = pickupPoint.position;
            targetPoint.SetParent(pickupPoint);
            targetPoint.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            //attackList.Remove(targetPoint);
            hasBomb = true;
        }
    }

    public void ThrowAway()//Animation event
    {
        if(hasBomb)
        {
            targetPoint.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            targetPoint.SetParent(transform.parent.parent);

            if(FindObjectOfType<PlayerController>().gameObject.transform.position.x-transform.position.x <0)
                targetPoint.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1,1) * power,ForceMode2D.Impulse);
            else
                targetPoint.GetComponent<Rigidbody2D>().AddForce(new Vector2(1,1) * power,ForceMode2D.Impulse);
            
        }
        hasBomb = false;
    }
}
