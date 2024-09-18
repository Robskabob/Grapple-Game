using KeySpace;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelEditorManager : MonoBehaviour
{
    public Level L;
    public Hierarchy H;
    public Camera C;
    // Start is called before the first frame update
    void Start()
    {
        L.LoadFile(L.ld.Name);
        L.Load();
        H.Load(L);
    }
    public Vector3 oldMouse;
    public Vector3 oldMousePos;
    public Vector3 oldCenterPos;
    public Vector3 oldPos;
    public Quaternion oldRot;
    public Vector3 HitPos;

    public Vector3 Diff;
    public Vector3 NewPos;
    public float A;
    public float B;

    public Transform T1;
    public Transform T2;
    public Transform T3;
    public float Dist;
    public float nDist;
    public float Speed;
    // Update is called once per frame
    void Update()
    {
        Vector3 V = Vector3.zero;
        if (KeySystem.GetBind(KeyBinds.Forward))
        {
            V += C.transform.forward * Speed;
        }
        if (KeySystem.GetBind(KeyBinds.Left))
        {
            V += C.transform.right * -Speed;
        }
        if (KeySystem.GetBind(KeyBinds.Backward))
        {
            V += C.transform.forward * -Speed;
        }
        if (KeySystem.GetBind(KeyBinds.Right))
        {
            V += C.transform.right * Speed;
        }
        if (KeySystem.GetBind(KeyBinds.Jump))
        {
            V += C.transform.up * Speed;
        }
        if (KeySystem.GetBind(KeyBinds.Crouch))
        {
            V += C.transform.up * -Speed;
        }
        C.transform.position += V;
        C.transform.position += C.transform.forward * Input.mouseScrollDelta.y;
        if(Input.GetMouseButtonDown(0))
		{
			if (Physics.Raycast(C.ScreenPointToRay(Input.mousePosition), out RaycastHit hit,1000, 33554432)) 
            {
                Transform T = hit.collider.transform;
                Transform P = T.parent;
                if (P == null)
                    return;
				//if (P.GetComponent<MoveHandle>() is MoveHandle M)
    //            {
    //                M.StartDrag(T.localPosition);
    //                Debug.Log(T.localPosition);
    //                return;
    //            }
            }
			else if (Physics.Raycast(C.ScreenPointToRay(Input.mousePosition), out hit,1000,-33554433)) 
            {
                Transform T = hit.collider.transform;
                Transform P = T.parent;
                if (P == null)
                    return;
                while (P.GetComponent<SaveFolder>() == null && P.GetComponent<Level>() == null) 
                {
                    T = P;
                    P = P.parent;
                    if (P == null)
                    {
                        Debug.LogError("Went to Top :(");
                        return;
                    }
                }
                Hierarchy.Element E = H.elements.Find((x) => x.T == T);
                //if(Window.Context.Selected == E) { }
                //E.Select();
            }
		}
        if(Input.GetMouseButtonDown(2))
        {
            if (Physics.Raycast(C.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
                Dist = hit.distance;
            else
                Dist = 0;
            oldMouse = Input.mousePosition;
            oldMousePos = C.ScreenPointToRay(Input.mousePosition).GetPoint(Dist);
            oldPos = C.transform.position;
            oldCenterPos = C.ScreenPointToRay(new Vector3(Screen.width/2,Screen.height/2)).GetPoint(Dist);
            A = 90 + Vector3.Angle(C.ScreenPointToRay(Input.mousePosition).direction, C.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2)).direction);

            //T3.position = oldCenterPos;
        } 
        else if(Input.GetMouseButton(2))
		{
            C.transform.position = oldPos;

            B = Vector3.Angle(C.ScreenPointToRay(Input.mousePosition).direction, C.ScreenPointToRay(oldMouse).direction);
            nDist = (Mathf.Sin(A * (Mathf.Deg2Rad)) * (Dist + C.nearClipPlane)) / Mathf.Sin((180 - B - A) * (Mathf.Deg2Rad));

            NewPos = C.ScreenPointToRay(Input.mousePosition).GetPoint(nDist - C.nearClipPlane);
           // Diff = (oldMousePos - NewPos);
            Diff = (Input.mousePosition - oldMouse);
            Diff *= -1;
            //T1.position = NewPos;
            //T2.position = oldMousePos;
            C.transform.Translate(Diff / 50,C.transform);
            //C.transform.position = (oldMousePos - C.ScreenPointToRay(Input.mousePosition).GetPoint(Dist)) + oldPos;
        }

        if(Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(C.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
            {
                HitPos = hit.point;
                Dist = hit.distance;
            }
			else 
            {
                Dist = 1;
                HitPos = C.transform.position;
            }
            oldMousePos = C.ScreenPointToRay(Input.mousePosition).GetPoint(Dist);
            oldMouse = Input.mousePosition;
            oldPos = C.transform.position;
            oldRot = C.transform.rotation;
		} 
        else if(Input.GetMouseButton(1))
        {
            oldPos += V;
            HitPos += V;
            C.transform.position = oldPos;
            C.transform.rotation = oldRot;
            oldMousePos = Input.mousePosition;
            Diff = (Input.mousePosition - oldMouse);
            C.transform.RotateAround(HitPos,transform.up, Diff.x/10);
            C.transform.RotateAround(HitPos,C.transform.right, -Diff.y/10);
            //C.transform.RotateAround(HitPos,Vector3.ro Diff, Diff.magnitude/50);
            //C.transform.position = (oldMousePos - C.ScreenPointToRay(Input.mousePosition).GetPoint(Dist)) + oldPos;
		}
    }
}
public class PreFabButton : UIBehaviour
{

}