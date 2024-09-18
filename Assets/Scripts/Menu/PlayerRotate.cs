using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    public float YrotSpeed = 0;
    public float XrotSpeed = 0;
    public float ZrotSpeed = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(XrotSpeed, YrotSpeed, ZrotSpeed * Time.deltaTime);
    }
}
