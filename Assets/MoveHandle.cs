using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class MoveHandle : MonoBehaviour
{
    public Camera Cam;
    public float mult;
    public Vector3 dir;
    public Vector3 pos1;
    public Vector2 mpos;
    public Vector3 mPosN;
    public Vector3 RPos;
    public Vector3 off;

    void Start()
    {
        
    }

    public void StartDrag(Vector3 vec) 
    {
        dir = vec;
        pos1 = Window.Context.Selected.T.position;
        mpos = Input.mousePosition;
    }

    public void EndDrag() 
    {
    
    }



    void Update()
    {
        mPosN = Input.mousePosition;
        mPosN.z = Vector3.Distance(transform.position, pos1);
        RPos = Cam.ScreenToWorldPoint(mPosN);
        off = RPos - pos1;
        off.Scale(dir);
        if(Window.Context.Selected != null) 
        {
            //Window.Context.Selected.T.position ;
            transform.position = Window.Context.Selected.T.position + off;
        }
        transform.localScale = Vector3.one * (mult * Vector3.Distance(transform.position , Cam.transform.position));
    }
}
