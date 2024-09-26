using IO;
using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Ionic.Zip;
using System.Net;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : NetworkManager
{
    public static GameManager GM { get => (GameManager)singleton; }

    public NetworkPlayer PlayerPrefab;
    public DiscordManager DM;
    public FileShare FS;
    public NetworkPlayer LocalPlayerPrefab;
    public PlayerRepresentation inGameRepFab;
    [Scene]
    public string GameScene;

    public GameMode GameMode;
    public Dictionary<uint, NetworkPlayer> Players = new Dictionary<uint, NetworkPlayer>();
    public RoomPlayer roomPlayerPrefab;
    public PlayerController GamePlayerPrefab;

    public NetworkPlayer LocalPlayer;

    public MainMenu MainMenu;
    public Level Level;

    
    private void OnEnable()
    {
        string LevelPath = Application.persistentDataPath + "/SaveData/";
        string TempPath = Application.persistentDataPath + "/temp/Levels.zip";
        string Download = "https://drive.google.com/uc?export=download&id=1SEnsbiKazx4Ikz9GCFZdiDncpyKkOvzV";
        if (!Directory.Exists(LevelPath))
        {
            Directory.CreateDirectory(LevelPath);

            Directory.CreateDirectory(Application.persistentDataPath + "/temp/");
            WebClient webClient = new WebClient();
            webClient.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
            webClient.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
            webClient.DownloadFile(new Uri(Download), TempPath);

            using (var zipFile = new ZipFile(TempPath))
            {
                zipFile.ExtractAll(LevelPath);
            }
        }
    //}public override void OnStartServer() { base.OnStartServer();

        Debug.Log("Logs Function");
        NetworkServer.RegisterHandler<ReplacePlayerMessager>(OnReplacePlayer);
        NetworkServer.RegisterHandler<NewPlayerMessager>(OnCreateRoomPlayer);
        NetworkClient.RegisterPrefab(GamePlayerPrefab.gameObject, SpawnDelegate, UnspawnHandler);
        NetworkClient.RegisterPrefab(LocalPlayerPrefab.gameObject, SpawnNetPlayerDelegate, UnspawnHandler);
        Debug.Log("Handlers Registered");
        //GM = this;
    }


    GameObject SpawnDelegate(SpawnMessage msg)
    {
        PlayerController PC = Instantiate(GamePlayerPrefab,msg.position,msg.rotation);
            Debug.Log(PC.PlayerNetID+"|"+msg.netId);
        if (msg.isOwner)
            LocalPlayer.Representation = PC;
        string str = "";
        for(int i = 0; i < msg.payload.Count; i++) 
        {
            str += (char)msg.payload.Array[i];        
        }
        Debug.Log("PayLoad: [" + str +"]");
        return PC.gameObject;
    }
    GameObject SpawnNetPlayerDelegate(SpawnMessage msg)
    {
        Debug.Log("Spawn " + msg.netId);
        NetworkPlayer NP = Instantiate(LocalPlayerPrefab, msg.position,msg.rotation);
        Players.Add(msg.netId, NP);
        Debug.Log("RoomPlayers: " + FindObjectOfType<RoomLobbyManager>().Players.Count);
        NP.ID = msg.netId;
        NP.local = msg.isLocalPlayer;
        Debug.Log("payLoad " + msg.payload.Array.Length + " | " + msg.payload.Array.ToString());
        //NP.SetUp(null,FindObjectOfType<RoomLobbyManager>().Players.Find((x) => x.PlayerNetID == (msg.netId)));
        return NP.gameObject;
    }
    void UnspawnHandler(GameObject spawned)
    {
        spawned.SetActive(false);
        //Destroy(spawned);
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        NewPlayerMessager PM = new NewPlayerMessager();
        Debug.Log("OnClientConnect");
        PM.Team = -1;
        PM.Name = PlayerPrefs.GetString("Name", "");
        NetworkClient.connection.Send(PM);
    }

    public void DoCo(System.Collections.IEnumerator C)
    {
        StartCoroutine(C);
    }

    public interface PlayerMessager : NetworkMessage
    {
        int Team { get; set; }
        Color Color { get; set; }
        string Name { get; set; }

    }
    public struct NewPlayerMessager : PlayerMessager
    {
        public NewPlayerMessager(RoomPlayer OP)
        {
            Team = OP.Team;
            Color = OP.TargetColor;
            Name = OP.Name;
        }

		public int Team { get; set; }
        public Color Color { get; set; }
		public string Name { get; set; }
    }
    public struct ReplacePlayerMessager : PlayerMessager
    {
        public ReplacePlayerMessager(RoomPlayer OP)
        {
            Team = OP.Team;
            Color = OP.TargetColor;
            Name = OP.Name;
        }

        public int Team { get; set; }
        public Color Color { get; set; }
        public string Name { get; set; }
    }

    public void OnReplacePlayer(NetworkConnectionToClient conn, ReplacePlayerMessager PM)
    {
        NetworkPlayer Player = Instantiate(PlayerPrefab, transform);// DM.transform); //KILL DISCORD
        Player.Team = PM.Team;
        Player.C = PM.Color;
        Player.Name = PM.Name;

        Player.SetUp(this,null);
        NetworkServer.ReplacePlayerForConnection(conn, Player.gameObject);


        Debug.Log("Replace " + PM.Name);
    }
    public void OnCreateRoomPlayer(NetworkConnectionToClient conn, NewPlayerMessager PM)
    {
        Debug.Log("OnCreateRoomPlayer");
        NetworkPlayer NetPlayer = Instantiate(LocalPlayerPrefab, transform);// DM.transform); //KILL DISCORD
        NetPlayer.Name = PM.Name;
        NetPlayer.Team = PM.Team;
        RoomPlayer Player = Instantiate(roomPlayerPrefab, transform);
        bool addP = NetworkServer.AddPlayerForConnection(conn, NetPlayer.gameObject);
        Debug.Log($"Player added {addP}");
        
        NetworkServer.Spawn(Player.gameObject, conn);
        NetPlayer.SetUp(this, Player);
        Player.name = "Dis hapenin " + UnityEngine.Random.Range(1,99);
        if(NetPlayer.isLocalPlayer)//LocalPlayer == null)
            LocalPlayer = NetPlayer;
        Debug.Log(NetPlayer);
        Debug.Log(Player);
        //Players.Add(NetPlayer.netId, NetPlayer);

        //Player.Team = PM.Team;
        //Player.TargetColor = PM.Color;
        //Player.Name = PM.Name;


        //Players.Add(Player.netId, Player);

        //Debug.Log("Create " + PM.Name);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GameManager))]
public class GameManagerInspecter : NetworkManagerEditor
{
    GameManager GM;
    bool players;
    private void OnEnable()
    {
        GM = target as GameManager;
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.indentLevel++;
        players = EditorGUILayout.Foldout(players,"Players");
        if(players)
            foreach(KeyValuePair<uint,NetworkPlayer> kv in GM.Players) 
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.IntField("Id: ",(int)kv.Key);
                EditorGUILayout.ObjectField("",kv.Value,typeof(NetworkPlayer),true);
                EditorGUILayout.EndHorizontal();
            }
        EditorGUI.indentLevel--;
        base.OnInspectorGUI();
    }
}
#endif