using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feet : MonoBehaviour
{
	public PlayerController PC;
	public List<Collider> Colliding = new List<Collider>();
	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (Colliding.Count == 0)
		{
			PC.OnGround = false;
		}
		else
		{
			PC.OnGround = true;
		}
    }

	private void OnTriggerEnter(Collider col)
	{
		Colliding.Add(col);
	}

	private void OnTriggerExit(Collider col)
	{
		Colliding.Remove(col);
	}
}
