using IO;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class RoomPlayer : PlayerRepresentation
{
    public string Name { get => Owner.Name; set => Owner.Name = value; }
    public int Team { get => Owner.Team; set { Owner.Team = value; Owner.C = Lobby.GetRoom(value).Color; } }

    public Graphic Graphic;
    public InputField InputField;
    public Vector2 TargetPosition;
    public Color TargetColor;
    public float Speed = 5;
    public bool Ready;

    public static RoomLobbyManager Lobby { 
        get
        { 
            if(_Lobby == null)
                _Lobby = FindObjectOfType<RoomLobbyManager>();
            return _Lobby;
        }
    }
    private static RoomLobbyManager _Lobby;

    private void Update()
    {
        RectTransform Rect = transform as RectTransform;
        Rect.anchoredPosition = Vector2.Lerp(Rect.anchoredPosition, TargetPosition, Time.deltaTime * Speed);
        if(!Ready)
        Graphic.color = Color.Lerp(Graphic.color, TargetColor, Time.deltaTime * Speed);
    }

    private void OnDestroy()//clean up
    {
        if (LocalPlayer)
            Lobby.Clear();
        else
        {
            if(Lobby != null && Lobby.RoomTeams != null)
                Lobby.GetRoom(Team).LeavePlayer(this);
            Lobby.Players.Remove(this);        
        }
    }
    
    public override void OnStartLocalPlayerRepresentation()
    {        
    }
    private void Start()
    {
        Debug.Log("RoomPlayer "+ PlayerNetID +" has Owner: " + (Owner != null));
        if (Owner == null)
        {
            GameManager.GM.Players[PlayerNetID].Representation = this;
        }
        InputField.interactable = LocalPlayer;
        if(!LocalPlayer)
        Lobby.AnyPlayerJoin(this);
    }
    public override void OnStartAuthority()
    {
        Debug.Log("Authority");
        Lobby.OnClinetJoin(this);
    }

    [Command]
    public void CmdChangeLevel(string levelName)
    {
        Debug.Log("Load Level: "+ levelName);
        if (File.Exists(Path.Combine(Application.persistentDataPath, "SaveData\\Levels", levelName,"Level.dat")))
            StartCoroutine(SendLevel(levelName));
        else
            Debug.Log($"Files for Level {levelName} do not exist at path {Path.Combine(Application.persistentDataPath, "SaveData\\Levels", levelName, "Level.dat")}");
    }

    public void sub(ref int i) 
    {
        i--;
    }

    public IEnumerator SendLevel(string levelName)
    {
        int c = 0;
        Debug.Log("Send Level to people");
        foreach (NetworkConnectionToClient C in NetworkServer.connections.Values)
        {
            Debug.Log("Send Level to " + C);
            c += 3;
            Lobby.GM.FS.SendFile(C, Path.Combine("SaveData\\Levels", levelName, "Icon.png"), () => sub(ref c));
            Lobby.GM.FS.SendFile(C, Path.Combine("SaveData\\Levels", levelName, "Level.dat"), () => sub(ref c));
            Lobby.GM.FS.SendFile(C, Path.Combine("SaveData\\Levels", levelName, "map.dat"), () => sub(ref c));
        }
        float wait = 10;
        while (c != 0) 
        {
            Debug.Log("c: " + c);
            wait -= Time.deltaTime;
            if (wait < 0)
            {
                Debug.Log("Timed out");
                break;
            }
            yield return null;
        }

        if (wait > 0)
        {
            Lobby.Level = levelName;
            Debug.Log("sent Level");
            RpcChangeLevel(levelName);
        }
    }

    [ClientRpc]
    public void RpcChangeLevel(string levelName)
    {
        Lobby.Level = levelName;
        Lobby.LevelPreview.texture = SaveSystem.LoadTexture(Path.Combine(Application.persistentDataPath + "/SaveData/Levels/" + levelName, "Icon.png"));
        //Lobby.GM.DM.UpdateLobbyActivity(Lobby);
        Debug.Log("got Level");
    }
    [Command]
    public void StartNewGame(string level)
    {
        if (level == "")
        {
            Debug.LogError("Level Empty");
            return;
        }
        StartGame(level);//"This One 2");

        GameManager GM = Lobby.GM;
        //KeyValuePair<uint,uint>[] Players = new KeyValuePair<uint, uint>[Lobby.Players.Count];
        Debug.Log(Lobby.Players.Count);
        for (int i = 0; i < Lobby.Players.Count; i++)
        {
            //or Netclient.spawend
            NetworkIdentity NI = NetworkServer.spawned[Lobby.Players[i].PlayerNetID];
            NetworkPlayer NP = NI.GetComponent<NetworkPlayer>();
            NP.Representation = Instantiate(GM.GamePlayerPrefab);
            //Players[i] = new KeyValuePair<uint,uint>(NP.netId,NP.Representation.netId);
            NetworkServer.Spawn(NP.Representation.gameObject,NI.connectionToClient);
            NP.Representation.Setup(NP.netIdentity);
        }
        //SetupPlayers(Players);
    }
    [ClientRpc]
    public void StartGame(string GameFolder)
    {
        //Debug.Log("start make game");
        Lobby.GM.DoCo(IStartGame(GameFolder));
    }
    //[ClientRpc]
    //public void SetupPlayers(KeyValuePair<uint, uint>[] Players)
    //{
    //    for (int i = 0; i < Players.Length; i++)
    //    {
    //        GameManager.GM.Players[Players[i].Key].re
    //    }
    //}
    public IEnumerator IStartGame(string GameFolder)
    {
        GameManager GM = Lobby.GM;

        //Debug.Log("New Game");

        leveldata ld = SaveSystem.LoadFile<leveldata>(Application.persistentDataPath + "/SaveData/Levels/" + GameFolder, "Level.dat");
        if(GM.Level == null)
            GM.Level = (new GameObject("Created Level")).AddComponent<Level>();
        Level L = GM.Level;
        //GM.Players = new Dictionary<uint, NetworkPlayer>(Lobby.Players.Count);
        //L.Player = FindObjectsOfType<PlayerController>().First((x) => x.PlayerNetID == GM.LocalPlayer.netId);
        //GM.LocalPlayer.Representation = L.Player;
        PlayerController PC = null;
        //Debug.Log("start loop! " + PC);
        while (PC == null) 
        {
            PC = GM.LocalPlayer.Representation as PlayerController;
            //Debug.Log("LOOP!");
            yield return null;
        }

        //Debug.Log("Exit LOOP!");

        PC.MM = GM.MainMenu;
        PC.C.ChangeName(Name);
        PC.gamemenuall = GM.MainMenu.transform.parent;
        PC.gamemenuall.gameObject.SetActive(false);
        L.Player = PC;//null
        //for (int i = 0; i < Lobby.Players.Count; i++)
        //{
        //    if (Lobby.Players[i].LocalPlayer)
        //    {
        //        L.Player = Instantiate(GM.inGameRepFab) as PlayerController;
        //        NetworkServer.Spawn(L.Player.gameObject, GM.LocalPlayer.netIdentity.connectionToClient);
        //        GM.LocalPlayer.Representation = L.Player;
        //    }
        //    //else
        //    //    Instantiate(GM.inGameRepFab);
        //    /*NetworkPlayer Player = *///GM.SwitchToGamePlayer(Lobby.Players[i]);
        //    //GM.Players.Add(Player.netId, Player);
        //}
        //MainMenu main = L.Player.MM;
        //= GameObject.FindObjectOfType<MainMenu>();// (LocalPlayer.Representation as PlayerController).MM;
        //main.Light.gameObject.SetActive(false);
        L.LoadFile(ld.Name);
        L.Load();

        //GM.DM.JoinGame(L); //KILL DISCORD
        GM.MainMenu.inGame = true;
        GM.MainMenu.game.CurrentLevel = ld.Name;
        GM.MainMenu.Camera.gameObject.SetActive(false);

        //ServerChangeScene(GameScene);

        //StartGame();
        Debug.Log($"Send Msg:{GM.GameScene}");
        //SceneManager.LoadSceneAsync(GameScene,LoadSceneMode.Single);
        GM.GameMode = Lobby.GameMode;
        //GM.Teams = Lobby.GameMode.Teams;
        Lobby.BackMenu.main.inGame = true;
        Lobby.BackMenu.main.allOff();
        Lobby.BackMenu.main.multi.Lobby.gameObject.SetActive(false);
        Lobby.BackMenu.main.game.gameObject.SetActive(true);
        //Lobby.gameObject.SetActive(false);
    }
    public void OnEditName(string name)
    {
        Debug.Log($"OnEditName \n{Owner} | {Owner?.name}");
        if (name != Name)
        {
            Debug.Log("RequestNameChange");
            PlayerPrefs.SetString("Name", name);
            RequestNameChange(name);
        }
    }
    [Command]
    public void RequestNameChange(string name, NetworkConnectionToClient sender = null)
    {
        Debug.Log("CommandNameChange");
        for (int i = 0; i < Lobby.Players.Count; i++)
        {
            if (Lobby.Players[i].Name == name) 
            {
                NameTaken(name);
                return;                
            }
        }
        
        ChangeName(sender.identity.netId, name);
        Lobby.GetPlayer(sender.identity.netId).UpdateName(name);
    }
    [ClientRpc]
    public void ChangeName(uint PlayerID, string name)
    {
        Debug.Log("RPC ChangeName");
        Lobby.GetPlayer(PlayerID).UpdateName(name);
    }
    public void UpdateName(string name)
    {
        Debug.Log("UpdateName");
        InputField.text = name;
        Name = name;
        Debug.Log("UpdateName Done");
    }


    public void OnSelectTeam(int team)
    {
        LeaveTeam(Team);
        JoinTeam(team);
    }

    [Command]
    public void JoinTeam(int team, NetworkConnectionToClient sender = null)
    {
        Team = team;
        Ready = false;
        PlayerJoinedTeam(sender.identity.netId, team);
    }
    [ClientRpc]
    public void PlayerJoinedTeam(uint PlayerID, int Team) 
    {
        RoomPlayer Player = Lobby.GetPlayer(PlayerID);
        Lobby.GetRoom(Team).JoinPlayer(Player);
        Player.Ready = false;
    }
    public void DoChangeReadyState()
    {
        if (Name == "")
        {
            InputField.placeholder.GetComponent<Text>().text = "Name Required";
            return;
        }
        ChangeReadyState(!Ready);
    }
    [Command]
    public void ChangeReadyState(bool ready, NetworkConnectionToClient sender = null)
    {
        if (Name == "" || Name == null)
        {
            NoName();
            return;
        }
        if (Team < 0)
        {
            //NoTeam();
            return;
        }
        if (Lobby.Level == "")
        {
            //NoLevel();
            return;
        }
        //if (Lobby.Players.TrueForAll((x) => x.Name != Name))
        //{
        //    NameTaken(Name);
        //    return;
        //}
        if (Ready == ready)
            return;
        Ready = ready;
        OnChangeReadyState(sender.identity.netId,ready);
    }
    [ClientRpc]
    public void OnChangeReadyState(uint NetId, bool ready)
    {
        RoomPlayer Player = Lobby.GetPlayer(NetId);
        Player.Ready = ready;
        Player.Graphic.color = Color.Lerp(Player.TargetColor, Color.black, .5f);
    }
    [TargetRpc]
    public void NoName()
    {
        InputField.placeholder.GetComponent<Text>().text = "Name Required";
    }
    [TargetRpc]
    public void NameTaken(string name)
    {
        Name = "";
        InputField.placeholder.GetComponent<Text>().text = $"Name {name} Taken";
        InputField.text = "";
    }

    [Command]
    public void RequestUpdateState(NetworkConnectionToClient sender = null)
    {
        uint[] netIds = new uint[Lobby.Players.Count];
        int[] Teams = new int[Lobby.Players.Count];
        string[] names = new string[Lobby.Players.Count];
        for (int i = 0; i < Lobby.Players.Count; i++) 
        {
            RoomPlayer P = Lobby.Players[i];
            netIds[i] = P.PlayerNetID;
            Teams[i] = P.Team;
            names[i] = P.Name;
        }
        UpdateState(sender,netIds, Teams, names);
    }
    [TargetRpc]
#pragma warning disable IDE0060 // Remove unused parameter
    public void UpdateState(NetworkConnection target, uint[] netIds, int[] Teams, string[] names)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        for (int i = 0; i < netIds.Length; i++) 
        {
            RoomPlayer P = Lobby.GetPlayer(netIds[i]);
            RoomTeam T = Lobby.GetRoom(Teams[i]);
            T.JoinPlayer(P);
            P.UpdateName(names[i]);
        }
    }

    [Command]
    public void LeaveTeam(int Team, NetworkConnectionToClient sender = null)
    {
        PlayerLeftTeam(sender.identity.netId, Team);
    }
    [ClientRpc]
    public void PlayerLeftTeam(uint PlayerID, int Team)
    {
        Lobby.GetRoom(Team).LeavePlayer(Lobby.GetPlayer(PlayerID));
    }
}
