using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skripts
{
	public class MovingPlatform : LogicObject
	{
		public override List<Wire> In { get => _in; set => _in = value; }
		public override List<Wire> Out { get => _out; set => _out = value; }
		public List<Vector3> Positions;
		[SerializeField]
		private int _pos;
		public float Speed;

		//options
		public bool Loop;
		public bool stateBased;
		public bool Relitive;

		//variables
		[SerializeField]
		private List<Wire> _in;
		private List<Wire> _out;

		//properties
		[SerializeField]
		private bool _direction;
		public bool Direction
		{
			get => _direction; set
			{
				if (value != _direction)
					if (_direction)
						Pos++;
					else
						Pos--;
				_direction = value;
			}
		}
		public bool Active
		{
			get
			{
				if (stateBased)
				{
					if(Loop)
						if(Relitive)
							return (transform.localPosition == Positions[0])^In[0].Value;
						else
							return (transform.position == Positions[0]) ^ In[0].Value;
					else
						return Direction ^ In[0].Value;
				}
				else
				return In[0].Value;
			}
		}
		public int Pos
		{
			get => _pos;

			set
			{
				if (value < Positions.Count)
					if (value >= 0)
						_pos = value;
					else if (Loop)
						_pos = Positions.Count - 1;
					else
						_direction = !_direction;
				else if (Loop)
					_pos = 0;
				else
					_direction = !_direction;
			}
		}

		//public Rigidbody rb;
		public List<Rigidbody> Riders = new List<Rigidbody>();

		[ExecuteAlways]
		private void Awake()
		{ 
			In = new List<Wire> { new Wire(this) };
			Out = new List<Wire> { };
			In[0].Value = true;
		}

		public void State()
		{

		}

		public void Goal()
		{

		}

		private void OnCollisionEnter(Collision collision)
		{
			if (collision.gameObject.GetComponent<PlayerController>() != null)
				if (!Riders.Contains(collision.rigidbody))
					Riders.Add(collision.rigidbody);
		}

		private void OnCollisionExit(Collision collision)
		{
			Rigidbody r = collision.rigidbody;
			if (Riders.Contains(r))
			{
				r.velocity += (Positions[Pos] - transform.position).normalized * Speed;
				Riders.Remove(r);
			}
		}

		// Update is called once per frame
		void Update()
		{
			if (Active)
			{
				Vector3 targ;
				if (Relitive)
					targ = Positions[Pos] - transform.localPosition;
				else
					targ = Positions[Pos] - transform.position;

				if (targ.magnitude < Speed * Time.deltaTime)
				{
					if (Relitive)
						transform.localPosition = Positions[Pos];
					else
						transform.position = Positions[Pos];
					//rb.velocity = Vector2.zero;
					if (Riders != null)
					{
						for(int i = 0; i < Riders.Count; i++)
							Riders[i].transform.position += targ;
							//if (Relitive)
							//	Riders[i].transform.Position += transform.TransformVector(targ);
							//else
							//	Riders[i].transform.position += targ;
					}
					if (Direction)
						Pos++;
					else
						Pos--;
				}
				else
				{
					//rb.velocity = targ.normalized * Speed;
					if (Relitive)
						transform.localPosition += targ.normalized * Speed * Time.deltaTime;
					else
						transform.position += targ.normalized * Speed * Time.deltaTime;
					if (Riders != null)
					{
						for (int i = 0; i < Riders.Count; i++)
						{
							//Riders[i].velocity = targ.normalized * Speed;
							Riders[i].transform.position += targ.normalized * Speed * Time.deltaTime;
						}
					}
				}
			}
		}
	}
}
