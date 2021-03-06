using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using UnityEngine.Events;

// [System.Serializable]
// public class EventVector3 : UnityEvent<Vector3>{}

public class MouseManager : Singleton<MouseManager>
{
    public Texture2D point,doorway,attack,target,arrow;
    RaycastHit hitInfo;
//    public EventVector3 onMouseClicked;
    public event Action<Vector3> OnMouseClicked;
    public event Action<GameObject> OnEnemyClicked;


    // Update is called once per frame
    void Update()
    {
        SetCursorTexture();
        MouseControl();
    }

    void SetCursorTexture()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray,out hitInfo))
        {
            //切换鼠标贴图
            switch(hitInfo.collider.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target,new Vector2(16,16),CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack,new Vector2(16,16),CursorMode.Auto);
                    break;
                case "1":
                    Cursor.SetCursor(target,new Vector2(16,16),CursorMode.Auto);
                    break;
            }
        }
    }

    void MouseControl()
    {
        if(Input.GetMouseButtonDown(0)&& hitInfo.collider != null)
        {
            if(hitInfo.collider.gameObject.CompareTag("Ground"))
                OnMouseClicked?.Invoke(hitInfo.point);
            if(hitInfo.collider.gameObject.CompareTag("Enemy"))
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            if(hitInfo.collider.gameObject.CompareTag("Attackable"))
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
        }
    }
}
