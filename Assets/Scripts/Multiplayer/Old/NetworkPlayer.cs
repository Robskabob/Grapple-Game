using Mirror;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    public PlayerRepresentation Representation 
    {
        get 
        {
            return _Representation;
        } 
        set
        {
            //OnChangeRepresentation(value.netIdentity);
            if (_Representation != null) { 
                _Representation.LocalPlayer = false;
            }
            _Representation = value;
            if (_Representation == null)
                return;
            _Representation.LocalPlayer = isLocalPlayer;
            //if(isLocalPlayer)
            //    _Representation.OnStartLocalPlayerRepresentation();
            _Representation.PlayerNetID = netId;
            _Representation.Owner = this;
        }
    }


    [SerializeField]
    private PlayerRepresentation _Representation;

    public string Name = "";
    public int Team = -1;
    public Color C;
    public uint ID;
    public bool local;


    public void SetUp(GameManager Gm, PlayerRepresentation representation)
    {
        //GM = Gm;
        Debug.Log("setup: "+netId);
        GameManager.GM.Players.Add(netId,this);
        ID = netId;
        local = isLocalPlayer;
        Representation = representation;
        representation.Owner = this;
        representation.PlayerNetID = netId;
        representation.LocalPlayer = isLocalPlayer;
    }
    private void Start()
    {
        Debug.Log("Start");
        if (isLocalPlayer)
        {
            GameManager.GM.LocalPlayer = this;
        }
    }

	private void Initialize(GameManager gM, NetworkPlayer networkPlayer)
	{

	}
}

/*

public class Brain 
{
    public float Fear;
    public float Search;
    public float Anger;

    public class Sense
    {
        public float Fear;
        public float Search;
        public float Anger;
    }
    public class Behaviour
    {
        public float FearMin;
        public float FearMax;
        public float SearchMin;
        public float SearchMax;
        public float AngerMin;
        public float AngerMax;

        public virtual void Action()
        {
        
        }

        public void CheckActivation(Brain B) 
        {
            if (Within(FearMin, FearMax, B.Fear) && Within(SearchMin, SearchMax, B.Search) && Within(AngerMin, AngerMax, B.Anger)) 
            {
                Action();
            }
        }

        public static bool Within(float min, float max, float v) 
        {
            return min > v && v < max;
        }
    }
}
public class BadGuy 
{
    public PlayerController Target;

}

public class AI 
{
    public List<AITask> Tasks;


    public class AITask 
    {
        public float ThoutCost;
        public float Priority;

    }

    public class SearchTask 
    {
        public void Pasive() { }
        public void Active() { }
    }
}

public class AI2 
{
    public PlayerController Targets;
    public Vector2 TargetPos;

    public float HP;

    public enum State 
    {
        idle,
        attack,
        flee,
    }

    public void Search() 
    {
        
    }
}
//*/