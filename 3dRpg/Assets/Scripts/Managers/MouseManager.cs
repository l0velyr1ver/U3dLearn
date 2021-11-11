using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

/*[System.Serializable]
public class EventVector3 : UnityEvent<Vector3> { }*/

public class MouseManager : Singleton<MouseManager>
{
    public Texture2D point, doorway, attack, target, arrow;
    RaycastHit hitInfo;
    //public EventVector3 OnMouseClicked;//这里注释掉拖拽

    public event Action<Vector3> OnMouseClicked;
    public event Action<GameObject> onEnemyClicked;

    protected override void Awake()
    {
        base.Awake();
        //DontDestroyOnLoad(this);
    }

    private void Update()
    {
        SetCursorTextrue();
        MouseControl();
    }

    void SetCursorTextrue()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray,out hitInfo))
        {
            switch (hitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target,new Vector2(16,16),CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;
            }
        }
    }

    void MouseControl()
    {
        if (Input.GetMouseButtonDown(0) && hitInfo.collider != null)
        {
            if (hitInfo.collider.gameObject.CompareTag("Ground"))
            {
                OnMouseClicked?.Invoke(hitInfo.point);
            }

            if (hitInfo.collider.gameObject.CompareTag("Enemy"))
            {
                onEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            }

        }
    }

}
