using UnityEngine;
using System.Collections;
using System;
using com.shephertz.app42.gaming.multiplayer.client.events;
using com.shephertz.app42.gaming.multiplayer.client.command;
using com.shephertz.app42.gaming.multiplayer.client.listener;
using System.Collections.Generic;

public class SC_Logic : MonoBehaviour {

    private string apiKey = "d8babde27e9310a6f141e850003e1b61bc68f7ddb02b90f71e79b7512d08be4c";
    private string secretKey = "37b40d5fc19109edd30a651ac6d424a2fcf4e088707953e5d1792efa43f4826c";
	private string email = "@gmail.com";
	private string userName = "";
    private string roomId = "";
	private List<string> rooms;
	private string opponentName = "";

	private bool isMyTurn = false;

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
	}
	
	void Update () 
	{

        if(ConnStater.Get_login_ready())
        {
            userName = ConnStater.get_Username();
            SC_AppWarpKit.connectToAppWarp(userName);
        }

        if ((Input.GetKeyDown(KeyCode.O) && isMyTurn)&&(ConnStater.Get_connection_status()))
        {
            Debug.Log("Move turn !!");

            SC_AppWarpKit.sendMove(userName);
        }
	}

	
	public void OnExceptionFromApp42(Exception error)
	{
		Debug.Log("onConnectToApp42: " + error.Message);
		GetComponent<GUIText>().text += error.Message + System.Environment.NewLine;
	}
	
	public void onConnectToAppWarp(ConnectEvent eventObj)
	{
        if (eventObj.getResult() == 0)
        {
            Debug.Log("onConnectToAppWarp " + eventObj.getResult());
            ConnStater.Set__connection_status(true);
            SC_AppWarpKit.CreateTurnBaseRoom("BattleShips", userName, 2, null, 15);
        }
	}
	
	public void onDisconnectFromAppWarp(ConnectEvent eventObj)
	{
		Debug.Log("onDisconnectFromAppWarp " + eventObj.getResult());
		GetComponent<GUIText>().text += "Disconnected from AppWrap" + System.Environment.NewLine;
	}
	
	public void OnCreateRoomDone(RoomEvent eventObj)
	{
		Debug.Log("OnCreateRoomDone " + eventObj.getResult() + " room Owner " + eventObj.getData().getRoomOwner() + " " + eventObj.getData().getRoomOwner());
		if(eventObj.getResult() == 	WarpResponseResultCode.SUCCESS)
		{
			roomId = eventObj.getData ().getId ();
			//GetComponent<GUIText>().text += "Room created! " +  eventObj.getData ().getId () + System.Environment.NewLine;
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
		// Debug.Log("OnGetLiveRoomInfo " + eventObj.getResult() + " " + eventObj.getData().getId() + " " + eventObj.getJoinedUsers().Length);
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
		    Debug.Log("Room ID:" + roomData.getId() + ", " + roomData.getRoomOwner());
			GetComponent<GUIText>().text += "Room ID:" + roomData.getId() + ", " + roomData.getRoomOwner() + System.Environment.NewLine;
			rooms.Add(roomData.getId()); // add to the list of rooms id
		}
		
		Debug.Log("Rooms Amount: " + rooms.Count);
		if(rooms.Count > 0)
		{
			roomId = rooms[0];
			SC_AppWarpKit.JoinToRoom (rooms[0]);
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
		opponentName = userName;
		SC_AppWarpKit.StartGame();
	}
	
	public void OnPrivateUpdateReceived(string sender, byte[] eventObj, bool fromUdp)
	{
		//Debug.Log("OnPrivateUpdateReceived" + " " + messageReceived);
	}
	
	public void OnPrivateChatReceived(string sender, string message)
	{
//		Debug.Log("OnPrivateChatReceived (" + sender + ") " + message + " " + sc_GuiManager.currentUser.CurrentUserState + " " + haveStart + " " + haveStartApproved);
	
		if(sender != userName)
		{
			Debug.Log ("Message Recived, (" + sender + ") " + message);
			GetComponent<GUIText>().text += "Message Recived, (" + sender + ") " + message;
		}
	}
	
	public void OnGameStarted(string sender, string roomId, string nextTurn)
	{
//		Debug.Log("OnGameStarted" + " " + sender + " " + roomId + " " + nextTurn + " " + sc_GuiManager.currentUser.CurrentUserState);
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
		if (move.getNextTurn () == userName)
			isMyTurn = true;
		else isMyTurn = false;
	}
}
