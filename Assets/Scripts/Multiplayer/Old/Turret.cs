using Mirror;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Turret : Damageable , SavableFab , IDamaging
{
    public Projectile Projectile;
    public Transform Head;
    public Rigidbody Target;
    public float Power;
    public float Range;
    public float FireRate;
    public float _fireRate;
    public int Team { get; set; }

	private void Start()
	{
        OnDeath += Destroyed;
	}

	private void Destroyed(object sender, System.EventArgs e)
	{
        gameObject.SetActive(false);
        Debug.Log("T ded");
	}

	private void Update()
    {
        if (Target != null)
        {
            float Dist = Vector3.Distance(Head.position, Target.position);
            if(Dist > Range)
            {
                Target = null;
                return;
            }
            Head.forward = (Target.position + (Target.velocity * Dist / Power)) - Head.position;

            if (isServer) 
            {
                _fireRate -= Time.deltaTime;
                if(_fireRate < 0)
                {
                    _fireRate = FireRate;
                    Projectile P = Instantiate(Projectile);
                    P.Shoot(Head.position + Head.forward*3, Head.rotation, Power, this);
                    NetworkServer.Spawn(P.gameObject);
                }
            }
        }
        else if(Random.Range(0,60) == 0)
		{
            Collider[] cols = Physics.OverlapSphere(Head.position,Range,1024);
            if (cols.Length == 0)
                return;
            float dist = Range + 1;
            int index = 0;
            for(int i = 0; i < cols.Length; i++) 
            {
                float NewDist = Vector3.Distance(Head.position,cols[i].transform.position);
                if(NewDist < dist) 
                {
                    dist = NewDist;
                    index = i;
                }
            }
            Target = cols[index].attachedRigidbody;
		}
    }
	public void LoadFab(mapdata.savedata data)
	{
        turretsavedata Data = (turretsavedata)data;

        Power = Data.Power;
        Range = Data.Range;
        FireRate = Data.FireRate;
    }

	public mapdata.savedata SaveFab()
	{
        return new turretsavedata(this);
    }
    [System.Serializable]
	public struct turretsavedata : mapdata.savedata
    {
        public float Power;
        public float Range;
        public float FireRate;

		public turretsavedata(Turret T)
		{
			Power = T.Power;
			Range = T.Range;
			FireRate = T.FireRate;
		}

		public int BoolNum()
		{
            return 0;
		}

        #if UNITY_EDITOR
        public void Draw(ref bool[] boolObs, ref int boolIndex)
        {
            EditorGUILayout.FloatField("Power", Power);
            EditorGUILayout.FloatField("Range", Range);
            EditorGUILayout.FloatField("FireRate", FireRate);
        }
        #endif
		public Transform LoadObject()
        {
            GameObject G = new GameObject();
            Turret T = G.AddComponent<Turret>();

            return G.transform;
        }
	}

}