//Unity course Summer 2015 - David Faizulaev
using UnityEngine;
using System.Collections;
using System;
using com.shephertz.app42.gaming.multiplayer.client.events;
using com.shephertz.app42.gaming.multiplayer.client.command;
using com.shephertz.app42.gaming.multiplayer.client.listener;
using System.Collections.Generic;

//This class is responsible for performing the connection to the Appwarp server, create/join a room and send 'moves' respectivly between the players.
public class SC_Logic : MonoBehaviour {

    private string apiKey = "d8babde27e9310a6f141e850003e1b61bc68f7ddb02b90f71e79b7512d08be4c";
    private string secretKey = "37b40d5fc19109edd30a651ac6d424a2fcf4e088707953e5d1792efa43f4826c";
	private string email = "@gmail.com";
	private string userName = "";
    private string roomId = "";
	private List<string> rooms;
	private string opponentName = "";

    //false = created room
    //true  = joined room
    private bool created_or_joined = false;
	private bool isMyTurn = false;
    private bool myPlayerBoard_Ready = false;
    
    public bool updateboards = false;
    public bool updateattackresult = false;
    public string enemy_move;
    public string my_last_move;
    public int my_enemy_attack_res;
    public int game_result;

    private GameObject myPlayerBoard;
    private MultiplayerBoardManager myPlayerBoard_script;

    private GameObject enemyPlayerBoard;
    private MultiEnemyBoardManager enemyPlayerBoard_script;

    private GameObject bckgrnd_music;
    private BackroundMusic bckgrnd_music_sc;

    private Vector2 vc;

    void OnEnable()
	{
            SC_Listener_App42.OnExceptionFromApp42 += OnExceptionFromApp42;
		
			SC_Listener_AppWarp.onConnectToAppWarp += onConnectToAppWarp;
			SC_Listener_AppWarp.onDisconnectFromAppWarp += onDisconnectFromAppWarp;
			SC_Listener_AppWarp.OnMatchedRooms += OnGetMatchedRoomsDone;
			SC_Listener_AppWarp.OnSubscribeToRoom += onSubscribeToRoom;
			SC_Listener_AppWarp.OnUnSubscribeToRoom += onUnSubscribeToRoom;
			SC_Listener_AppWarp.OnJoinToRoom += OnJoinToRoom;
			SC_Listener_AppWarp.OnLeaveFromRoom += OnLeaveFromRoom;
			SC_Listener_AppWarp.OnCreateRoomDone += OnCreateRoomDone;
			SC_Listener_AppWarp.onGetLiveRoomInfo += OnGetLiveRoomInfo;
			SC_Listener_AppWarp.OnSendPrivateUpdate += OnSendPrivateUpdate;
			SC_Listener_AppWarp.OnStartGameDone += OnStartGameDone;
			SC_Listener_AppWarp.OnStopGameDone += OnStopGameDone;
			SC_Listener_AppWarp.OnRoomCreated += OnRoomCreated;
			SC_Listener_AppWarp.OnUserJoinRoom += OnUserJoinRoom;
			SC_Listener_AppWarp.OnUserLeftRoom += OnUserLeftRoom;
			SC_Listener_AppWarp.OnPrivateUpdateReceived += OnPrivateUpdateReceived;
			SC_Listener_AppWarp.OnGameStarted += OnGameStarted;
			SC_Listener_AppWarp.OnGameStopped += OnGameStopped;
			SC_Listener_AppWarp.OnSendMove += OnSendMove;
			SC_Listener_AppWarp.OnMoveCompleted += OnMoveCompleted;
	}
		
	void OnDisable()
	{
		SC_Listener_App42.OnExceptionFromApp42 -= OnExceptionFromApp42;
		
		SC_Listener_App42.OnExceptionFromApp42 -= OnExceptionFromApp42;
		SC_Listener_AppWarp.onConnectToAppWarp -= onConnectToAppWarp;
		SC_Listener_AppWarp.onDisconnectFromAppWarp -= onDisconnectFromAppWarp;
		SC_Listener_AppWarp.OnMatchedRooms -= OnGetMatchedRoomsDone;
		SC_Listener_AppWarp.OnSubscribeToRoom -= onSubscribeToRoom;
		SC_Listener_AppWarp.OnUnSubscribeToRoom -= onUnSubscribeToRoom;
		SC_Listener_AppWarp.OnJoinToRoom -= OnJoinToRoom;
		SC_Listener_AppWarp.OnLeaveFromRoom -= OnLeaveFromRoom;
		SC_Listener_AppWarp.OnCreateRoomDone -= OnCreateRoomDone;
		SC_Listener_AppWarp.onGetLiveRoomInfo -= OnGetLiveRoomInfo;
		SC_Listener_AppWarp.OnSendPrivateUpdate -= OnSendPrivateUpdate;
		SC_Listener_AppWarp.OnStartGameDone -= OnStartGameDone;
		SC_Listener_AppWarp.OnStopGameDone -= OnStopGameDone;
		SC_Listener_AppWarp.OnRoomCreated -= OnRoomCreated;
		SC_Listener_AppWarp.OnUserJoinRoom -= OnUserJoinRoom;
		SC_Listener_AppWarp.OnUserLeftRoom -= OnUserLeftRoom;
		SC_Listener_AppWarp.OnPrivateUpdateReceived -= OnPrivateUpdateReceived;
		SC_Listener_AppWarp.OnGameStarted -= OnGameStarted;
		SC_Listener_AppWarp.OnGameStopped -= OnGameStopped;
		SC_Listener_AppWarp.OnSendMove -= OnSendMove;
		SC_Listener_AppWarp.OnMoveCompleted -= OnMoveCompleted;
	}
	
	void Start () 
	{
		SC_App42Kit.App42Init(apiKey,secretKey);
        SC_AppWarpKit.WarpInit(apiKey,secretKey);

        game_result = -9999;
        myPlayerBoard = GameObject.Find("MyBoard");
        myPlayerBoard_script = myPlayerBoard.GetComponent<MultiplayerBoardManager>();
        myPlayerBoard_Ready = true;

        enemyPlayerBoard = GameObject.Find("EnemyBoard");
        enemyPlayerBoard_script = enemyPlayerBoard.GetComponent<MultiEnemyBoardManager>();
	}
	
	void Update () 
	{

        //bckgrnd_music_sc.CheckifPlaying();

        if (ConnStater.Get_connection_status() == false)
        {
            if (ConnStater.Get_login_ready())
            {
                userName = ConnStater.get_Username();
                SC_AppWarpKit.connectToAppWarp(userName);
            }
        }

        if (myPlayerBoard_Ready)
        {
            if (IsItMine())
            {
                if (myPlayerBoard_script.ShipsPlaced())
                        myPlayerBoard_script.turn_msg.text = "Your turn - try to attack the enemies ships";
            }

            if (!IsItMine())
            {
                if (myPlayerBoard_script.ShipsPlaced())
                        myPlayerBoard_script.turn_msg.text = "Opponent's turn - will now try attacking your ships";
            }
        }
	}
	
	public void OnExceptionFromApp42(Exception error)
	{
		Debug.Log("onConnectToApp42: " + error.Message);
	}
	
	public void onConnectToAppWarp(ConnectEvent eventObj)
	{
        if (eventObj.getResult() == 0)
        {
            Debug.Log("onConnectToAppWarp " + eventObj.getResult());
            ConnStater.Set__connection_status(true);
            SC_AppWarpKit.GetRoomsInRange(1, 1);
        }
	}
	
	public void onDisconnectFromAppWarp(ConnectEvent eventObj)
	{
		Debug.Log("onDisconnectFromAppWarp " + eventObj.getResult());
	}
	
	public void OnCreateRoomDone(RoomEvent eventObj)
	{
		Debug.Log("OnCreateRoomDone " + eventObj.getResult() + " room Owner " + eventObj.getData().getRoomOwner() + " " + eventObj.getData().getRoomOwner());
        if (eventObj.getResult() == WarpResponseResultCode.SUCCESS)
        {
            roomId = eventObj.getData().getId();
            SC_AppWarpKit.JoinToRoom(eventObj.getData().getId());
        }
	}
	
	//only room creator will get the notification
	public void OnDeleteRoomDone(RoomEvent eventObj)
	{
		Debug.Log("OnDeleteRoomDone " + eventObj.getResult());
	}
	
	public void onSubscribeToRoom(RoomEvent eventObj)
	{
        if (eventObj.getResult() == WarpResponseResultCode.SUCCESS)
        {
            Debug.Log("onSubscribeRoomDone : " + eventObj.getResult());
            if(!created_or_joined)
                SC_AppWarpKit.StartGame();
        }
	}
	
	public void onUnSubscribeToRoom(RoomEvent eventObj)
	{
		Debug.Log("onUnSubscribeToRoom " + eventObj.getResult());
	}
	
	public void OnJoinToRoom(RoomEvent eventObj)
	{
		if (eventObj.getResult() == WarpResponseResultCode.SUCCESS)
		{
			Debug.Log("OnJoinToRoom " + eventObj.getResult());
			opponentName = eventObj.getData().getRoomOwner();
			Debug.Log("roomId: " + roomId + ", OpponentName: " + opponentName);
            SC_AppWarpKit.RegisterToRoom(roomId);
		}
		else
		{
			SC_AppWarpKit.JoinToRoom(roomId);
		}
	}
	public void OnLeaveFromRoom(RoomEvent eventObj)
	{
		Debug.Log("OnLeaveFromRoom " + eventObj.getResult());
	}
	
	public void OnGetLiveRoomInfo(LiveRoomInfoEvent eventObj)
	{
		
	}
	
	public void OnSendPrivateUpdate(byte result)
	{
	}
	
	//onky room creator will get that
	public void OnStartGameDone(byte result)
	{
        Debug.Log("OnStartGameDone : " + result);
	}

	public void OnStopGameDone(byte result)
	{
		Debug.Log("OnStopGameDone : " + result);
	}
	
	public void OnGetMatchedRoomsDone(MatchedRoomsEvent eventObj)
	{
       
		Debug.Log("OnGetMatchedRoomsDone : " + eventObj.getResult());
		rooms = new List<string>();
		foreach (var roomData in eventObj.getRoomsData())
		{
		    rooms.Add(roomData.getId()); // add to the list of rooms id
		}
		
		Debug.Log("Number of rooms found: " + rooms.Count);
        
        if (rooms.Count > 0)
        {
            roomId = rooms[0];
            //false = created room
            //true  = joined room
            created_or_joined = true;
            SC_AppWarpKit.JoinToRoom(rooms[0]); //rooms[0] is the last created room in the list
        }
        else
        {
            Debug.Log("no room exists - need to create");
            SC_AppWarpKit.CreateTurnBaseRoom("BattleShips"+UnityEngine.Random.Range(0,1000), userName, 2, null, 60);
        }
	}
	
	public void OnRoomCreated(RoomData eventObj)
	{
		Debug.Log("OnRoomCreated : " + eventObj.getName());
	}
	
	//both player get the notification
	public void OnUserLeftRoom(RoomData eventObj, string nameOfUser)
	{
		Debug.Log("OnUserLeftRoom : " + eventObj.getName());
	}
	
	//only host recieve it
	public void OnUserJoinRoom(RoomData eventObj, string userName)
	{
		Debug.Log("OnUserJoinRoom" + " " + eventObj.getRoomOwner() + " User connected" + userName);
		
        //checking if room was created by other user - if so, it will be set as the opponent.
        if (eventObj.getRoomOwner() != userName)
        {
            opponentName = eventObj.getRoomOwner();
        }

        //If game room was created by user - then he should initiate game start
        if (!created_or_joined)
            SC_AppWarpKit.StartGame();
	}
	
	public void OnPrivateUpdateReceived(string sender, byte[] eventObj, bool fromUdp)
	{
		//Debug.Log("OnPrivateUpdateReceived" + " " + messageReceived);
	}
	
	public void OnGameStarted(string sender, string roomId, string nextTurn)
	{
        Debug.Log("game successfully started");
		isMyTurn = true;
	}
	
	public void OnGameStopped(string sender, string roomId)
	{
		Debug.Log("OnGameStopped" + " " + sender + " " + roomId);
	}

	public void OnSendMove(byte result)
	{
		Debug.Log("OnSendMove : " + result);
	}

	public void OnMoveCompleted(MoveEvent move)
	{     
		Debug.Log("OnMoveCompleted" + " " + move.getNextTurn());
        if (move.getNextTurn() == userName)
        {
            ParseEnemyMove(move.getMoveData());
            isMyTurn = true;
        }
        else isMyTurn = false;
	}

    public bool IsItMine()
    {
        return isMyTurn;
    }

    public void MakeMyMove(string str)
    {
        Debug.Log("MakeMyMove !!");
        my_last_move = str;
        SC_AppWarpKit.sendMove(str);
    }

    private void ParseEnemyMove(string str)
    {
        //if (str == null) str = "";
        Debug.Log("enemy move!!!!!!!!!!!!!!!!!!!!!!         "+str);
        //enemy tried to attack battleship
        if ((str.Contains("X")&&str.Contains("Y")))
        {
            parseToVector(str);
            myPlayerBoard_script.EnemyMove(vc);
        }
        else
        {
            //other player completed ship structure
            if (str.Contains("2"))
                Debug.Log("other player completed ship structure");

            else
            {
                if (str.Contains("AttackResultSuccess"))
                {
                    Debug.Log("previous attack attempt success");
                    parseToVector(str);
                    enemyPlayerBoard_script.MarkAttackResult(vc);
                }

                if (str.Contains("AttackResultMiss"))
                {
                    Debug.Log("previous attack attempt failed");
                    parseToVector(str);
                    enemyPlayerBoard_script.MarkAttackResult(vc);
                }
            }
        }
    }

    private void parseToVector(string str) 
    {
        int startInd = str.IndexOf("X:") + 2;
        float aXPosition = float.Parse(str.Substring(startInd, str.IndexOf(" Y") - startInd));//x
        startInd = str.IndexOf("Y:") + 2;
        float aYPosition = float.Parse(str.Substring(startInd, str.IndexOf("}") - startInd));//y

        vc = new Vector2(aXPosition, aYPosition);
        Debug.Log("xxxxxxxxxxxxxxxxxxxxxxxxxx"+vc.ToString());
    }
}