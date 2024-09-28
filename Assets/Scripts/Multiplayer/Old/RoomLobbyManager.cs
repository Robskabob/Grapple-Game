using IO;
using Mirror;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class RoomLobbyManager : MonoBehaviour , LevelSelectUI.LevelRequester 
{
    public RoomPlayer LocalPlayer;

    public GameMode GameMode;
    public GameManager GM;

    public RectTransform TeamBox;
    public RoomTeam RoomTeamPrefab;
    public RoomTeam DefaultTeam;
    public List<RoomPlayer> Players;
    public List<RoomTeam> RoomTeams;

    public Text PlayerCount;
    public Text ReadyCount;

    public Text CountDown;

    public Fader Fade;

    public MultiMenu BackMenu;
    public string Level;
    public RawImage LevelPreview;

    private void Awake()
    {
        BackMenu.main.levels.UI.Requester = this;
    }

    public void SelectNewLevel()
    {
        BackMenu.main.allOff();
        BackMenu.main.levels.gameObject.SetActive(true);
        BackMenu.main.levels.UI.Requester = this;
        gameObject.SetActive(false);
    }
    public void OnLevelSelected(string levelName)
    {
        BackMenu.main.allOff();
        gameObject.SetActive(true);

        PlayerPrefs.SetString("LastMap", levelName);
        LocalPlayer.CmdChangeLevel(levelName);
    }
    public void doBack() 
    {
        BackMenu.main.allOff();
        BackMenu.gameObject.SetActive(true);
        BackMenu.main.multi.Lobby.gameObject.SetActive(false);

        Clear();
        foreach(NetworkPlayer NP in GM.Players.Values)
        {
            if(NP != null)
            Destroy(NP.gameObject);
        }
        GM.Players.Clear();

        if(NetworkServer.active)
        NetworkServer.DisconnectAll();
        else
        NetworkClient.Disconnect();
        GM.StopHost();
    }

    private void Start()
    {
    }

    private void Update()
    {
        if (LocalPlayer != null)
        {
            int Count = 0;
            for (int i = 0; i < Players.Count; i++) 
            {
                if (Players[i].Ready && Players[i].Team > -1)
                    Count++;
            }
            Fade.Active = Count == Players.Count;

            CountDown.text = $"{5 - Fade.Value}";
            PlayerCount.text = $"{Players.Count} / {GameMode.MaxPlayers}";
            ReadyCount.text = $"{Count} / {Players.Count}";
            if (LocalPlayer.isServer && DefaultTeam.Players.Count == 0 && Players.Count >= GameMode.MinPlayers)
            {
                //LocalPlayer.StartNewGame();// GameMode,Players);
                //gameObject.SetActive(false);
            }
        }
    }

    public void Clear()
    {
        if(DefaultTeam != null)
        Destroy(DefaultTeam.gameObject);
        for (int i = 0; i < RoomTeams.Count; i++)
        {
            Destroy(RoomTeams[i].gameObject);
        }
        Players.Clear();
        RoomTeams.Clear();        
    }

    internal void AnyPlayerJoin(RoomPlayer Player)
    {
        Players.Add(Player);
        RectTransform Rect = Player.transform as RectTransform;
        Rect.SetParent(DefaultTeam.transform);
        Rect.localScale = Vector3.one;
        Rect.anchorMin = new Vector2(0,1);
        Rect.anchorMax = Vector2.one;
        Rect.sizeDelta = new Vector2(-30,100);
        Rect.ForceUpdateRectTransforms();
        Rect.anchoredPosition = Vector3.zero;
    }


    public void GenerateLobby()
    {
        GameManager.GM.maxConnections = GameMode.MaxPlayers;
        DefaultTeam = Instantiate(RoomTeamPrefab, TeamBox);
        DefaultTeam.Color = Color.gray;
        DefaultTeam.Header.text = "No Team";
        DefaultTeam.TeamID = -1;
        SetupRoom(DefaultTeam, 0, GameMode.TeamCount + 1);
        RoomTeam OT = null;
        for (int i = 0; i < GameMode.TeamCount; i++)
        {
            RoomTeam T = Instantiate(RoomTeamPrefab,TeamBox);

            Navigation N = T.Button.navigation;
            N.mode = Navigation.Mode.Explicit;
            if(OT != null) 
            {
                N.selectOnDown = OT.Button;
                T.Button.navigation = N;

                N = OT.Button.navigation;
                N.selectOnUp = T.Button;
                OT.Button.navigation = N;
            } else T.Button.navigation = N;

            T.Color = GameMode.Teams[i].TeamColor;
            T.Header.text = $"Team {i + 1}";
            T.TeamID = i;
            SetupRoom(T,i+1 , GameMode.TeamCount + 1);
            RoomTeams.Add(T);
            OT = T;
        }
        //GM.DM.JoinLobby(this); //KILL DISCORD
    }

    public void SetupRoom(RoomTeam T,int slot,int slotCount)
    {
        T.Graphic.color = T.Color;
        RectTransform Rect = T.transform as RectTransform;
        //Rect.localScale = Vector3.one;
        Rect.anchorMin = new Vector2(slot / (float)slotCount, 0); 
        Rect.anchorMax = new Vector2((slot+1) / (float)slotCount,1);
        Rect.sizeDelta = new Vector2(-20,-20);
        T.TargetSize = new Vector2(-20, -20);
        Rect.ForceUpdateRectTransforms();
        Rect.anchoredPosition = Vector3.zero;
        T.Button.onClick.AddListener(() => LocalPlayer.OnSelectTeam(T.TeamID));
    }
    
    public void OnClinetJoin(RoomPlayer player)
    {
        LocalPlayer = player;
        if (LocalPlayer.isServer)
        {
            Fade.LocalPlayer = player;
            LocalPlayer.CmdChangeLevel(PlayerPrefs.GetString("Level", ""));
        }
        GenerateLobby();
        AnyPlayerJoin(player);
        player.RequestUpdateState();
        //removed bc null refrance to Owner, probably not set yet
        //LocalPlayer.OnEditName(PlayerPrefs.GetString("Name",""));
        //LocalPlayer.JoinTeam(-1);

        Athu();
    }

    //public override void OnStartAuthority()
    //{   
    //}

    public void Athu()
    {
        //netIdentity.AssignClientAuthority(LocalPlayer.netIdentity.connectionToClient);
    }

    public RoomTeam GetRoom(int Team) => Team == -1 ? DefaultTeam : RoomTeams[Team];
    public RoomPlayer GetPlayerO(uint netId) => Players.Find((x) => x.PlayerNetID == netId);
    public RoomPlayer GetPlayer(uint netId) => GM.Players[netId].Representation as RoomPlayer;

}

[Serializable]
public class GameMode
{
    public int MinPlayers;
    public int MaxPlayers;
    public byte TeamCount;//0 = free for all//

    public List<TeamData> Teams;
    [Serializable]
    public class TeamData 
    {
        public Color TeamColor;
    }
}

public class ADVGameMode 
{
    public ModeLoop Loop;
    public class Stats 
    {
        public string[] TeamStats;
        public Dictionary<string,int> TeamStatsDict;
        public string[] PlayerStats;


        public Dictionary<int, Stat> TeamsStats;
        public Dictionary<int, Stat> PlayersStats;

        public class StatHeader 
        {
            public Dictionary<string, int> stats;
            public int this[string i]
            {
                get { return stats[i]; }
                set { stats[i] = value; }
            }
        }

        public class Stat
        {
            public float[] Values;

            public Stat(int length)
            {
                Values = new float[length];
            }

            public float this[int i] 
            {
                get { return Values[i]; }
                set { Values[i] = value; }
            }
        }

        public void SetUp(string[] Team, string[] Player,int Teams, int Players) 
        {
            TeamStats = Team;
            PlayerStats = Player;

            for (int i = 0; i < Teams; i++) 
            {
                TeamsStats.Add(i,new Stat(TeamStats.Length));
            }
            for (int i = 0; i < Players; i++) 
            {
                PlayersStats.Add(i,new Stat(PlayerStats.Length));
            }
        }

        public float GetTeamStat(string name,int TeamID) 
        {
            return TeamsStats[TeamID][TeamStatsDict[name]];
        }
    }
    public abstract class ModeLoop 
    {
        public abstract void SetUp();
        public abstract void Loop();
    }
}