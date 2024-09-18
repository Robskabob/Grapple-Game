using KeySpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardLook : MonoBehaviour
{
	public float sensitivityX = 2F;
	public float sensitivityY = 2F;
	public Quaternion originalRotation = new Quaternion(0,0,0,1);
	public float rotationX = 0F;
	public float rotationY = 0F;

	// Start is called before the first frame update
	void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		//originalRotation = transform.rotation;
	}

    // Update is called once per frame
    void Update()
	{
		//rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
		//rotationX += Input.GetAxis("Mouse X") * sensitivityX;

		rotationY += KeySystem.GetLookY() * sensitivityY;
		rotationX += KeySystem.GetLookX() * sensitivityX;

		//if (rotationY) 
		//{
		//
		//}

		Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);
		//Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);

		//Rotate
		transform.localRotation = originalRotation * yQuaternion;// * xQuaternion;
	}
}
