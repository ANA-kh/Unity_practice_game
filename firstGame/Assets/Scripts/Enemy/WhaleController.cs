using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhaleController : Enemy
{
    public void Swalow()
    {
        if (targetPoint.GetComponent<Bomb>())
        {
            targetPoint.GetComponent<Bomb>().TurnOff();
            Destroy(targetPoint.gameObject);
        }
    }
}
