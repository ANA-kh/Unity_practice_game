using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawMove : MonoBehaviour
{


    //Platform movement speed.平台移动速度
    public float speed;

    //This is the position where the platform will move
    public Transform MovePosition;

    private Vector3 StartPosition;
    private Vector3 EndPosition;
    private bool OnTheMove;

    // Use this for initialization
    void Start()
    {
        //Store the start and the end position. Platform will move between these two points.储存左右两端点位置
        StartPosition = this.transform.position;
        EndPosition = MovePosition.position;
    }

    void FixedUpdate()
    {

        float step = speed * Time.deltaTime;

        if (OnTheMove == false)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, EndPosition, step);
        }
        else
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, StartPosition, step);
        }

        //When the platform reaches end. Start to go into other direction.
        if (Mathf.Abs(this.transform.position.x - EndPosition.x)<0.01 && Mathf.Abs(this.transform.position.y - EndPosition.y) <0.01 && OnTheMove == false)
        {
            OnTheMove = true;
        }
        else if (Mathf.Abs(this.transform.position.x - StartPosition.x)<0.01 && Mathf.Abs(this.transform.position.y - StartPosition.y) <0.01&& OnTheMove == true)
                 
        {
            OnTheMove = false;
        }
    }



}
