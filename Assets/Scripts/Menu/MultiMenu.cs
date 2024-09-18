using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class MultiMenu : Menu
{
    public GameManager GM;

    public MainMenu main;
    public PlayMenu play;
    public Transform Lobby;

    public InputField ServerIP;
    public Text Displaytext;

	protected override void Start()
	{
        base.Start();
        GM = FindObjectOfType<GameManager>();
	}

	protected override void Update()
	{
        base.Update();
        // client ready
        //if (NetworkClient.isConnected && !ClientScene.ready)
        //{
        //    if (GUILayout.Button("Client Ready"))
        //    {
        //        ClientScene.Ready(NetworkClient.connection);
        //
        //        if (ClientScene.localPlayer == null)
        //        {
        //            ClientScene.AddPlayer(NetworkClient.connection);
        //        }
        //    }
        //}
        if (NetworkClient.isConnected && isJoin) 
        {
            isJoin = false;
            EnterLobby();
        }
        if(isJoin && !NetworkClient.active)
        {
            Displaytext.text = $"Connection to :{GM.networkAddress} Failed :(";
            isJoin = false;
        }
    }
    bool isJoin = false;

	public void HostGame()//could add canceling
    {
        GM.StartHost();
        EnterLobby();
    }

    public void JoinGame()//could add canceling
    {
        GM.networkAddress = ServerIP.text;
        GM.StartClient();
        Displaytext.text = $"Connecting to :" + GM.networkAddress;
        isJoin = true;
    }

    public void EnterLobby() 
    {
        main.allOff();
        Lobby.gameObject.SetActive(true);    
    }

    public void doBack()
    {
        main.allOff();
        play.gameObject.SetActive(true);
    }
}
