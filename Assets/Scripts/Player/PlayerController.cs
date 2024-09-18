using KeySpace;
using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using static HealthSystem;

public class PlayerController : PlayerRepresentation, IDamaging
{
	public Rigidbody rb;
	public Feet F;
	public HardLook HL;
	public Movement M;
	public ItemController HC;
	public DamageablePlayer DP;

	public Customization C;

	public HeadsUpDisplay HUD;
	public MainMenu MM;

	public Color color = Color.cyan;

	public float Death = -65;
	public Vector3 SpawnPoint;

	public bool OnGround;
	public Transform gamemenuall;

	public int Team { get { return Owner.Team; } }


	void Awake()
	{
		rb.freezeRotation = true;
	}
	private void Start()
	{
		UIBillboard Bill = GetComponentInChildren<UIBillboard>();
		if (!LocalPlayer)
		{
			HL.enabled = false;
			HL.GetComponentInChildren<Camera>().gameObject.SetActive(false);
			HUD.gameObject.SetActive(false);
			Bill.Board.color = Owner.C - new Color(0, 0, 0, .5f);
			Bill.Text.text = Owner.Name;
		}
		else
		{
			UIBillboard.Target = transform;
			Bill.gameObject.SetActive(false);
			//OnDeath += CmdOnDeath;
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (!LocalPlayer)
			return;
		if (transform.position.y < Death || KeySystem.GetBindDown(KeyBinds.Reset))
		{
			DP.CmdOnDeath(this, new EventArgs());
		}
		Vector3 v = transform.rotation.eulerAngles;
		transform.rotation = Quaternion.Euler(v.x, HL.rotationX, v.z);

		//Menu
		if (KeySystem.GetBindDown(KeyBinds.MainMenu))
		{
			if (Time.timeScale == 1)
			{
				Time.timeScale = 0;
				KeySystem.instance.isPaused = true;
				Cursor.lockState = CursorLockMode.None;
				MM.allOff();
				MM.game.gameObject.SetActive(true);
				gamemenuall.gameObject.SetActive(true);
			}
			else if (Time.timeScale == 0)
			{
				Time.timeScale = 1;
				KeySystem.instance.isPaused = false;
				Cursor.lockState = CursorLockMode.Confined;
				MM.allOff();
				gamemenuall.gameObject.SetActive(false);
			}
		}
		Look();
	}
	float lastLook;
	private void Look()
	{
		float newLook = HL.rotationX + HL.rotationY;
		if (Mathf.Abs(lastLook - newLook) > .1)
		{
			lastLook = newLook;
			if (isServer)
				UpdateLook(HL.rotationX, HL.rotationY);
			else
				SendLook(HL.rotationX, HL.rotationY);
		}
	}

	[Command]
	public void SendLook(float x, float y)
	{
		HL.rotationX = x;
		HL.rotationY = y;
		Quaternion yQuaternion = Quaternion.AngleAxis(y, Vector3.left);
		HL.transform.localRotation = HL.originalRotation * yQuaternion;
		UpdateLook(x, y);
	}

	[ClientRpc(includeOwner = false)]
	private void UpdateLook(float x, float y)
	{
		HL.rotationX = x;
		HL.rotationY = y;
		Quaternion yQuaternion = Quaternion.AngleAxis(y, Vector3.left);
		HL.transform.localRotation = HL.originalRotation * yQuaternion;
	}

	//public event EventHandler<EventArgs> OnDeath;
	//
	//[Command]
	//public void CmdOnDeath(object sender, EventArgs e)
	//{
	//	RpcOnDeath(sender, e);
	//}
	//[ClientRpc(excludeOwner = true)]
	//public void RpcOnDeath(object sender, EventArgs e)
	//{
	//	OnDeath(sender, e);
	//}
}
public class HealthManager : MonoBehaviour
{
	public float MaxHealth;
	public float Health;
	public float MaxArmor;
	public float Armor;
	public float MaxShield;
	public float Shield;

	public void Dammage(float value, DammageType type)
	{
		if (Shield < 0)
		{
			Health -= type.Data().HealthMult * value * ((1 - Armor / MaxArmor) * type.Data().ArmorPierce);
			Armor -= type.Data().ArmorMult * value;
		}
		Shield -= type.Data().ShieldMult * value;
	}
}
public static class HealthSystem
{
	public static Dictionary<DammageType, DammageTypeData> DammageTypeDict = new Dictionary<DammageType, DammageTypeData>
		{
			{ DammageType.Blunt, new DammageTypeData(1,.1f,.3f,.8f,.1f) },
			{ DammageType.Sharp, new DammageTypeData(1.2f,.4f,.2f,.2f,.2f) },
			{ DammageType.fire, new DammageTypeData(1.1f,.2f,.4f,.9f,0) },
			{ DammageType.Plasma, new DammageTypeData(.8f,.2f,1.2f,.6f,0) },
		};
	public static DammageTypeData Data(this DammageType dt)
	{
		return DammageTypeDict[dt];
	}
	public enum DammageType
	{
		Blunt,
		Sharp,
		fire,
		Plasma,
	}
	public struct DammageTypeData
	{
		public float HealthMult;
		public float ArmorMult;
		public float ShieldMult;
		public float ArmorPierce;
		public float ShieldPierce;

		public DammageTypeData(float healthMult, float armorMult, float shieldMult, float armorPierce, float shieldPierce)
		{
			HealthMult = healthMult;
			ArmorMult = armorMult;
			ShieldMult = shieldMult;
			ArmorPierce = armorPierce;
			ShieldPierce = shieldPierce;
		}
	}
}