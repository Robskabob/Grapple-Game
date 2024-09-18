using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;
using System.Net;
using UnityEngine.Networking;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if true //UNITY_WEBGL
public class DiscordManager : MonoBehaviour
{
    public void JoinLobby(RoomLobbyManager roomLobbyManager)
	{

    }

    public void UpdateLobbyActivity(RoomLobbyManager lobby)
	{

	}

	public void JoinGame(Level name)
	{

	}
}
#else
public class DiscordManager : MonoBehaviour, ISerializationCallbackReceiver
{
    public float hmm; 
    public Discord.Discord D;
    public LobbyManager LM;
    public UserManager UM;
    public OverlayManager OM;
    public Discord.NetworkManager NM;
    public VoiceManager VM;
    public ActivityManager AM;

    public long LobbyID;

    private string GetPublicIpAddress()
    {
        var request = (HttpWebRequest)WebRequest.Create("http://ifconfig.me");
        //WWWForm form = new WWWForm();
        //form.
        //var urequest = UnityWebRequest.Post("http://ifconfig.me",form);


        request.UserAgent = "curl"; // this will tell the server to return the information as if the request was made by the linux "curl" command

        string publicIPAddress;

        request.Method = "GET";
        using (WebResponse response = request.GetResponse())
        {
            using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
            {
                publicIPAddress = reader.ReadToEnd();
            }
        }

        return publicIPAddress.Replace("\n", "");
    }
    void Start()
    {
        Debug.Log("Start");
        D = new Discord.Discord(get from file idk, (ulong)CreateFlags.Default);

        D.SetLogHook(LogLevel.Debug,(a,b) => {
            Debug.Log($"Level: {a} , {b}");
        });

        LM = D.GetLobbyManager();
        UM = D.GetUserManager();
        VM = D.GetVoiceManager();
        OM = D.GetOverlayManager();
        NM = D.GetNetworkManager();
        SetupActivity();

        Activity A = new Activity()
        {
            Name = "Grapple Game",
            Details = "",
            State = "Main Menu",
            Assets = new ActivityAssets() { LargeImage = "fallen",LargeText = "this is a map",SmallImage = "haloreach_dot",SmallText = "TRAITOR"},
            //Timestamps = new ActivityTimestamps() { Start = System.DateTime.Now.Ticks, End = System.DateTime.Now.Ticks + 10000 },
            //Party = new ActivityParty() { Id = "IDK", Size = new PartySize() { CurrentSize = 1, MaxSize = 9 } },
            //Secrets = new ActivitySecrets() { Join = "Join", Match = "match", Spectate = "spec" },
            //Type = ActivityType.Playing,
            Instance = true,
        };

        AM.UpdateActivity(A, (res) =>
        {
            if (res == Result.Ok)
                Debug.Log("yay");
            else
                Debug.LogError("Fail");
        });

        UM.OnCurrentUserUpdate += UM_OnCurrentUserUpdate;
        //Debug.Log(D.GetUserManager().GetCurrentUser().Username);
    }
    private void SetupActivity()
    {
        AM = D.GetActivityManager();
        AM.OnActivityInvite += AM_OnActivityInvite;
        AM.OnActivityJoin += AM_OnActivityJoin;
        AM.OnActivityJoinRequest += AM_OnActivityJoinRequest;
        //AM.on
    }

    private void AM_OnActivityJoinRequest(ref User user)
    {
        Debug.Log($"Request: {user.Username} #{user.Discriminator}");
    }

    private void AM_OnActivityJoin(string secret)
    {
        Debug.Log($"Secret: {secret}");


        LM.ConnectLobbyWithActivitySecret(secret, (Result result, ref Lobby lobby) =>
        {
            LM.ConnectVoice(lobby.Id, (Discord.Result voiceResult) => {

                if (voiceResult == Discord.Result.Ok)
                {
                    Debug.Log($"New User Connected to Voice! Say Hello! Result: {voiceResult}");
                }
                else
                {
                    Debug.Log($"Failed with Result: {voiceResult}");
                };
            });
            LobbyID = lobby.Id;
            //Debug.Log($"Result: {result} lobby {lobby.Id} created with secret {lobby.Secret}");
            //Debug.Log(LM.GetLobbyMetadataValue(lobby.Id,"Server_IP"));

            GameManager GM = FindObjectOfType<GameManager>();
            
            GM.networkAddress = LM.GetLobbyMetadataValue(lobby.Id, "Server_IP");
            GM.StartClient();
            GM.MainMenu.allOff();
            GM.MainMenu.multi.Lobby.gameObject.SetActive(true);
            // Connect to voice chat, used in this case to actually know in overlay if your successful in connecting.
            LM.ConnectVoice(lobby.Id, (Result voiceResult) =>
            {

                if (voiceResult == Result.Ok)
                {
                    Debug.Log($"Connected Voice: {voiceResult}");
                }
                else
                {
                    Debug.Log($"Failed Voice: {voiceResult}");
                };
            });
            //Connect to given lobby with lobby Id
            LM.ConnectNetwork(lobby.Id);
            LM.OpenNetworkChannel(lobby.Id, 0, true);
            foreach (var user in LM.GetMemberUsers(lobby.Id))
            {
                //Send a hello message to everyone in the lobby
                LM.SendNetworkMessage(lobby.Id, user.Id, 0,
                    System.Text.Encoding.UTF8.GetBytes(string.Format("Hello, {0}!", user.Username)));
            }
            //Sends this off to a Activity callback named here as 'UpdateActivity' passing in the discord instance details and lobby details
            //UpdateActivity(discord, lobby);
        });
    }

    private void AM_OnActivityInvite(ActivityActionType type, ref User user, ref Activity activity)
    {
        Debug.Log($"Type: {type} User: {user.Username} #{user.Discriminator} Activity: {activity.Name}, {activity.Secrets.Join}");
    }

    public void JoinLobby(RoomLobbyManager RLM)
    {
        if (RLM.LocalPlayer.isServer) 
        {
            LobbyTransaction LT = LM.GetLobbyCreateTransaction();
            LT.SetCapacity((uint)RLM.GameMode.MaxPlayers);
            LT.SetType(LobbyType.Public);

            LT.SetMetadata("Server_IP", GetPublicIpAddress());
            LM.CreateLobby(LT, (Result result, ref Lobby lobby) =>
            {
                LobbyID = lobby.Id;
                Debug.Log($"lobby {lobby.Id} created with secret {lobby.Secret}");

                // We want to update the capacity of the lobby
                // So we get a new transaction for the lobby
                var newTxn = LM.GetLobbyUpdateTransaction(lobby.Id);
                newTxn.SetCapacity(3);

                LM.UpdateLobby(lobby.Id, newTxn, (result2) =>
                {
                    Debug.Log("lobby Result: " + result2);
                });
        Activity A = new Activity()
        {
            Name = "Grapple Game",
            Details = "Level: MultMap",
            State = "Lobby",
            //Timestamps = new ActivityTimestamps() { Start = System.DateTime.Now.Ticks, End = System.DateTime.Now.Ticks + 10000 },
            Party = new ActivityParty() { Id = "abc", Size = new PartySize() { CurrentSize = RLM.Players.Count, MaxSize = RLM.GameMode.MaxPlayers } },
            Secrets = new ActivitySecrets() { Join = LM.GetLobbyActivitySecret(lobby.Id)},
            //Type = ActivityType.Playing,
            Instance = true,
        };

        AM.UpdateActivity(A, (res) => {
            if (res == Result.Ok)
                Debug.Log("yay");
            else
                Debug.LogError("Fail" + res);
        });
            });
        }
    }
    
    public void JoinGame(Level Level)
    {
        Activity A = new Activity()
        {
            //Name = "Grapple Game",
            Details = "Level: "+Level.ld.Name,
            State = "In Game",
            Assets = new ActivityAssets() { LargeImage = Level.ld.Name.ToLower(), LargeText = Level.ld.Name, SmallImage = "haloreach_dot", SmallText = "Team Color?" },
            //Timestamps = new ActivityTimestamps() { Start = System.DateTime.Now.Ticks, End = System.DateTime.Now.Ticks + 10000 },
            //Party = new ActivityParty() { Id = "abc", Size = new PartySize() { CurrentSize = RLM.Players.Count, MaxSize = RLM.GameMode.MaxPlayers } },
            //Secrets = new ActivitySecrets() { Join = LM.GetLobbyActivitySecret(lobby.Id)},
            //Type = ActivityType.Playing,
            Instance = true,
        };

        AM.UpdateActivity(A, (res) => {
            if (res == Result.Ok)
                Debug.Log("yay");
            else
                Debug.LogError("Fail" + res);
        });
    }
    
    public void UpdateLobbyActivity(RoomLobbyManager RLM)
    {
        Activity A = new Activity()
        {
            Name = "Grappling Game",
            Details = "Level: "+RLM.Level,
            State = "Lobby",
            //Timestamps = new ActivityTimestamps() { Start = System.DateTime.Now.Ticks, End = System.DateTime.Now.Ticks + 10000 },
            Assets = new ActivityAssets() { LargeImage = RLM.Level.ToLower(), LargeText = RLM.Level, SmallImage = "haloreach_dot", SmallText = RLM.LocalPlayer.Name },
            Party = new ActivityParty() { Id = LobbyID.ToString(), Size = new PartySize() { CurrentSize = RLM.Players.Count, MaxSize = RLM.GameMode.MaxPlayers } },
            Secrets = new ActivitySecrets() { Join = LM.GetLobbyActivitySecret(LobbyID)},
            //Type = ActivityType.Playing,
            Instance = true,
        };

        AM.UpdateActivity(A, (res) => {
            if (res == Result.Ok)
                Debug.Log("yay");
            else
                Debug.LogError("Fail" + res);
        });
    }

    private void UM_OnCurrentUserUpdate()
    {
        User U = UM.GetCurrentUser();
        Debug.Log($"Name: {U.Username} #{U.Discriminator} ID: {U.Id}");
    }
	private void Reset()
	{
        Debug.LogError("Test Reset ");
	}
	void Update()
    {
        //FIX
        //Update Discord sdk
        //if(D != null)
        //D.RunCallbacks();
    }
    private void OnDisable()
    {
        if (D != null)
        {
            Debug.Log("Not Done");
            new WaitForSeconds(2);
            if (D != null)
                Debug.Log("still Not Done");
            else
                Debug.Log("Done");
        }
        else
            Debug.Log("Done");
    }

    private void OnApplicationQuit()
    {
        AM.ClearActivity((res) => {
            
            Debug.Log("Disposed Discord");
            if (res == Result.Ok)
                Debug.Log("yay!!");
            else
                Debug.LogError("Fail " + res);

        });
            D.Dispose();
        Debug.Log("Exit");
    }

    public void OnBeforeSerialize()
    {
        if(D != null)
            D.Dispose();
    }

    public void OnAfterDeserialize()
    {
        //Start();
    }
    //*/
}

//#if UNITY_EDITOR
//[CustomEditor(typeof(DiscordManager))]
//public class DiscordManagerInspector : Editor 
//{
//    DiscordManager DM;
//
//    private void OnEnable()
//    {
//        DM = target as DiscordManager;
//    }
//
//    public override void OnInspectorGUI()
//    {
//        //base.OnInspectorGUI();
//        EditorGUILayout.LongField(DM.LobbyID, "Lobby ID");//,);
//    }
//}
//#endif

#endif