using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBillboard : MonoBehaviour
{
    public Image Board;
    public Text Text;
    private void OnEnable()
    {
        //NetworkPlayer P = GameManager.GM.LocalPlayer;
        //Target = P.Representation.transform;
    }

    public static Transform Target;

    private void Update()
    {
        transform.forward = (Target.transform.position - transform.position);
    }
}
