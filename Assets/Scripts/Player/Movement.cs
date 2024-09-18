using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using KeySpace;
public class Movement : MonoBehaviour
{
	public float speed = 25;
	public float runSpeed = 2;
	public float airSpeed = 10;
	public float jump = 150;
	public float jumpWait = 1;
	public float _jumpWait;
	public bool Crouch;

	public PlayerController PC;
	
    void Update()
    {
		if (!PC.LocalPlayer)
			return;
        Vector3 V = Vector3.zero;

		if (KeySystem.GetBind(KeyBinds.Crouch))//Input.GetKey(KeyCode.D))
		{
			transform.localScale = new Vector3(1,.5f,1);
		}
        else 
		{
			transform.localScale = new Vector3(1,1,1);		
		}

		if (PC.OnGround)
		{
			//if (KeySystem.GetBind(KeyBinds.Right))//Input.GetKey(KeyCode.D))
			//{
			//	V += Vector3.right;
			//}
			//if (KeySystem.GetBind(KeyBinds.Left))//Input.GetKey(KeyCode.A))
			//{
			//	V += Vector3.left;
			//}
			//if (KeySystem.GetBind(KeyBinds.Forward))//Input.GetKey(KeyCode.W))
			//{
			//	V += Vector3.forward;
			//}
			//if (KeySystem.GetBind(KeyBinds.Backward))//Input.GetKey(KeyCode.S))
			//{
			//	V += Vector3.back;
			//}
			//V = V.normalized * speed;
			V = KeySystem.GetMove() * speed;
			if (KeySystem.GetBind(KeyBinds.Sprint))//Input.GetKey(KeyCode.LeftShift))
			{
				V *= runSpeed;
			}
			if (_jumpWait < 0)
			{ 
				if (KeySystem.GetBind(KeyBinds.Jump))//Input.GetKey(KeyCode.Space))
				{
					V += Vector3.up * jump;
					_jumpWait = jumpWait;
				}
			}
			else 
			{
				_jumpWait -= Time.deltaTime;
			}
		}
		else
		{
			//if (KeySystem.GetBind(KeyBinds.Right))//Input.GetKey(KeyCode.D))
			//{
			//	V += Vector3.right * airSpeed;
			//}
			//if (KeySystem.GetBind(KeyBinds.Left))//Input.GetKey(KeyCode.A))
			//{
			//	V += Vector3.left * airSpeed;
			//}
			//if (KeySystem.GetBind(KeyBinds.Forward))//Input.GetKey(KeyCode.W))
			//{
			//	V += Vector3.forward * airSpeed;
			//}
			//if (KeySystem.GetBind(KeyBinds.Backward))//Input.GetKey(KeyCode.S))
			//{
			//	V += Vector3.back * airSpeed;
			//}
			V = KeySystem.GetMove() * airSpeed;
		}

        PC.rb.AddRelativeForce(V * Time.deltaTime * 60,ForceMode.VelocityChange);
    }
}