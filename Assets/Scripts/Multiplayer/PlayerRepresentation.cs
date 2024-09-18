using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class PlayerRepresentation : NetworkBehaviour 
{
	public bool LocalPlayer;
    [SyncVar]
    public uint PlayerNetID;

    //public NetworkPlayer Owner { 
    //    get
    //    {
    //        Debug.Log($"Players: {GameManager.GM.Players.Count} Key {PlayerNetID} Exists {GameManager.GM.Players.ContainsKey(PlayerNetID)}");
    //        if(owner == null) 
    //            owner = GameManager.GM.Players[PlayerNetID];
    //        return owner; 
    //    } 
    //}
    //[SerializeField]
    //NetworkPlayer owner;
    public NetworkPlayer Owner;

    [ClientRpc]
    public void Setup(NetworkIdentity Owner)
    {
        (Owner.GetComponent<NetworkPlayer>()).Representation = this;
        Debug.Log("Player Set up");
    }
    public virtual void OnStartLocalPlayerRepresentation()
    {

    }
}

public class Manager : NetworkManager 
{

}
