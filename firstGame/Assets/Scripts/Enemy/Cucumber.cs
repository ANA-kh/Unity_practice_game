using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cucumber : Enemy
{


    public void SetOff()//Animation event
    {
        if (targetPoint.GetComponent<Bomb>())
        {
            targetPoint.GetComponent<Bomb>().TurnOff();
        }
    }

}
