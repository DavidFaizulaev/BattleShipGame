using UnityEngine;
using System.Collections;
using System;
using com.shephertz.app42.gaming.multiplayer.client.events;
using com.shephertz.app42.gaming.multiplayer.client.command;
using System.Collections.Generic;
using Facebook.MiniJSON;


public class SC_Multiplayer : MonoBehaviour
{
	private int usersInRoom = 0;
    private string apiKey = "c7f9c6c235dd6dae1ccbcb60ca58191599aa57b1e8c1a677da3c6ed747efa25d";
    private string secretKey = "051c143847f5317f782752ea415df62f10f5a2e43d878dc1a4fa9befc89d72e4";
    private string email = "@gmail.com";
    public string userName = "";
    private string roomId = "";
    private List<string> rooms;
    private string opponentName = "";
    private SC_App42Kit sc_App42Kit;
    private SC_AppWarpKit sc_AppWarpKit;
    public Dictionary<string, string>_dict = new Dictionary<string, string>();
    private bool isMyTurn = false;
	private string[] usersArray;
	public int numOfUsers = -1;
    public SC_game sc_game;
    public SC_controller sc_controller;
    public SC_slider sc_slider;
    void OnEnable()
    {
        SC_Listener_App42.onCreatedUserApp42 += onCreatedUserApp42;
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
        SC_Listener_AppWarp.OnSendPrivateChat += OnSendPrivateChat;
        SC_Listener_AppWarp.OnStartGameDone += OnStartGameDone;
        SC_Listener_AppWarp.OnStopGameDone += OnStopGameDone;
        SC_Listener_AppWarp.OnRoomCreated += OnRoomCreated;
        SC_Listener_AppWarp.OnUserJoinRoom += OnUserJoinRoom;
        SC_Listener_AppWarp.OnUserLeftRoom += OnUserLeftRoom;
        SC_Listener_AppWarp.OnPrivateUpdateReceived += OnPrivateUpdateReceived;
        SC_Listener_AppWarp.OnPrivateChatReceived += OnPrivateChatReceived;
        SC_Listener_AppWarp.OnGameStarted += OnGameStarted;
        SC_Listener_AppWarp.OnGameStopped += OnGameStopped;
        SC_Listener_AppWarp.OnSendMove += OnSendMove;
        SC_Listener_AppWarp.OnMoveCompleted += OnMoveCompleted;
		SC_Listener_AppWarp.OnGetLiveLobbyInfoDone += onGetLiveLobbyInfoDone;
    }

    void OnDisable()
    {
        SC_Listener_App42.onCreatedUserApp42 -= onCreatedUserApp42;
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
        SC_Listener_AppWarp.OnSendPrivateChat -= OnSendPrivateChat;
        SC_Listener_AppWarp.OnStartGameDone -= OnStartGameDone;
        SC_Listener_AppWarp.OnStopGameDone -= OnStopGameDone;
        SC_Listener_AppWarp.OnRoomCreated -= OnRoomCreated;
        SC_Listener_AppWarp.OnUserJoinRoom -= OnUserJoinRoom;
        SC_Listener_AppWarp.OnUserLeftRoom -= OnUserLeftRoom;
        SC_Listener_AppWarp.OnPrivateUpdateReceived -= OnPrivateUpdateReceived;
        SC_Listener_AppWarp.OnPrivateChatReceived -= OnPrivateChatReceived;
        SC_Listener_AppWarp.OnGameStarted -= OnGameStarted;
        SC_Listener_AppWarp.OnGameStopped -= OnGameStopped;
        SC_Listener_AppWarp.OnSendMove -= OnSendMove;
        SC_Listener_AppWarp.OnMoveCompleted -= OnMoveCompleted;
		SC_Listener_AppWarp.OnGetLiveLobbyInfoDone -= onGetLiveLobbyInfoDone;
    }

    void Start()
    {
     
        if (sc_controller == null)
            sc_controller = GameObject.Find("SC_controller").GetComponent<SC_controller>();
       

        SC_App42Kit.App42Init(apiKey, secretKey);
        SC_AppWarpKit.WarpInit(apiKey, secretKey);
		sc_AppWarpKit = new SC_AppWarpKit();
        
    }
    public Boolean runMultyplayer(string username)
    {
        Debug.Log("multyplayer user name is:" + username);
        if (username == null)
        {
            return false;
        }
        this.userName = username;
        SC_App42Kit.InitUser(userName, "1234", userName.Replace(" ","") + "@gmail.com");
        sc_AppWarpKit.connectToAppWarp(userName);

        return true;
    }



    void Update()
    {

    }
	public void onGetLiveLobbyInfoDone(LiveRoomInfoEvent eventObj){
		Debug.Log ("onGetLiveLobbyInfoDone");
		if (eventObj.getResult () == WarpResponseResultCode.SUCCESS) {

			if(eventObj.getJoinedUsers()==null){ //getJoinedusers is a string[] type, if length = null that means there are no users connected to the game
				Debug.Log("no players at the game");
                sc_AppWarpKit.CreateTurnBaseRoom("room" + userName, userName, 2, null, 10); //then create a room
			}
			else{
				sc_AppWarpKit.GetRoomsInRange(1, 1); //else run room search
			}
		}
	}
    public void OnJoinToRoom(RoomEvent eventObj) // need to work on this one
    {
        if (eventObj.getResult() == WarpResponseResultCode.SUCCESS)
        {
            Debug.Log("OnJoinToRoom  result:" + eventObj.getResult());
            opponentName = eventObj.getData().getRoomOwner();

            Debug.Log("roomId: " + roomId + ", OpponentName: " + opponentName);
            sc_AppWarpKit.RegisterToRoom(roomId);
        }
        else
        {
            sc_AppWarpKit.JoinToRoom(roomId);
        }
    }
    public void onCreatedUserApp42(object respond)
    {
        Debug.Log("onCreatedUserApp42");

    }

    public void OnExceptionFromApp42(Exception error)
    {
        Debug.Log("App42 Exception: " + error.Message);

    }

    public void onConnectToAppWarp(ConnectEvent eventObj)
    {

        if (eventObj.getResult() == WarpResponseResultCode.SUCCESS)
        {
            Debug.Log("connected to appwrap successfully");
			//sc_AppWarpKit.getLiveLobbyInfo (); //this func call to the ongetlobbyinfodone answare
			sc_AppWarpKit.GetRoomsInRange(1, 1);
            
        }
        else if (eventObj.getResult() == WarpResponseResultCode.UNKNOWN_ERROR)
        {
            Debug.Log("onConnectToAppWrap error");
        }
    }

    public void onDisconnectFromAppWarp(ConnectEvent eventObj)
    {
        Debug.Log("disconnecting from AppWrap result:" + eventObj.getResult());
    }

    public void OnCreateRoomDone(RoomEvent eventObj) //after getting response that the room was created add the player to a room
    {
        Debug.Log("OnCreateRoomDone");
        if (eventObj.getResult() == WarpResponseResultCode.SUCCESS)
        {
            roomId = eventObj.getData().getId();
            sc_AppWarpKit.JoinToRoom(eventObj.getData().getId());

        }

    }

    //only room creator will get the notification
    public void OnDeleteRoomDone(RoomEvent eventObj)
    {
        Debug.Log("OnDeleteRoomDone result:" + eventObj.getResult());
    }

    public void onSubscribeToRoom(RoomEvent eventObj)
    {

        Debug.Log("onSubscribeToRoom result:" + eventObj.getResult());

    }

    public void onUnSubscribeToRoom(RoomEvent eventObj)
    {
        Debug.Log("onUnSubscribeToRoom " + eventObj.getResult());
    }


    public void OnLeaveFromRoom(RoomEvent eventObj)
    {
        Debug.Log("OnLeaveFromRoom " + eventObj.getResult());
    }

    public void OnGetLiveRoomInfo(LiveRoomInfoEvent eventObj)
    {
		Debug.Log ("OnGetLiveRoomInfo");
		string[] users;
		users = eventObj.getJoinedUsers ();
		Debug.Log (users.Length);
    }

    public void OnSendPrivateUpdate(byte result)
    {
    }

    public void OnSendPrivateChat(byte result)
    {
        Debug.Log("onSendPrivateChatDone : " + result);

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

    public void OnGetMatchedRoomsDone(MatchedRoomsEvent eventObj) // checks if the room is avilable, not, will create new room
    {

        Debug.Log("OnGetMatchedRoomsDone : " + eventObj.getResult());
        rooms = new List<string>();
        foreach (var roomData in eventObj.getRoomsData())
        {
           rooms.Add(roomData.getId()); // add current room to our list of rooms
        }

        Debug.Log("Rooms Amount: " + rooms.Count);
        if (rooms.Count > 0)
        {
            Debug.Log("rooms list:" + rooms);
            Debug.Log("roomID:" + roomId);
            roomId = rooms[0];// why
            Debug.Log("new roomID:" + roomId);
            sc_AppWarpKit.JoinToRoom(rooms[0]);//rooms[0] is the last created room in the list
        }
        else
        {
            sc_AppWarpKit.CreateTurnBaseRoom("room" + userName, userName, 2, null, 10); //that means that the are no rooms at all, create new turnBaseRoom

        }
        Debug.Log("Rooms Amount: " + rooms.Count);
    }


    public void OnRoomCreated(RoomData eventObj)
    {
        Debug.Log("OnRoomCreated : " + eventObj.getName());
        

    }

    //both player get the notification
    public void OnUserLeftRoom(RoomData eventObj, string nameOfUser)
    {
        Debug.Log("OnUserLeftRoom : " + eventObj.getName());
        GameObject.Find("SC_controller").GetComponent<SC_controller>().whenGameEnd();
    }

    //only host receives it
    public void OnUserJoinRoom(RoomData eventObj, string userName)
    {
        Debug.Log("OnUserJoinRoom owner" + " " + eventObj.getRoomOwner() + " User connected (guest)" + userName);
		//this.userName = userName;
		opponentName = userName;
        sc_AppWarpKit.StartGame();
    }

    public void OnPrivateUpdateReceived(string sender, byte[] eventObj, bool fromUdp)
    {
 
    }

    public void OnPrivateChatReceived(string sender, string message)
    {
		if (sender != userName)
        {
            Debug.Log("Message Received, (" + sender + ") " + message);
        }
    }
    public void OnGameStarted(string sender, string roomId, string nextTurn)
    {

        Debug.Log("OnGameStarted");//+ " " + sender + " " + roomId + " " + nextTurn + " " + sc_GuiManager.currentUser.CurrentUserState);
		Debug.Log("player1(guest): " + userName + " player2 (owner): " + nextTurn);
		
        turnQuery();
        deActivateLoading();
    }
    public void activateLoading()
    {
        sc_controller.panelDictionary["loading"].SetActive(true);
        Debug.Log("LOADING");
    }
    public void deActivateLoading()
    {
        sc_controller.panelDictionary["loading"].SetActive(false);
        Debug.Log("STOP LOADING");
    }
    public void OnGameStopped(string sender, string roomId)
    {
        Debug.Log("OnGameStopped" + " " + sender + " " + roomId);
    }

    public void OnSendMove(byte result)
    {
        //Debug.Log("OnSendMove : " + result);
		if (result == 4) {
			Debug.Log ("error while sending");
		}
    }

    public void turnQuery()
    { 
		string myString = Json.Serialize(_dict);
		sc_AppWarpKit.sendMove(myString);
    }
    public void mySend(Dictionary<string, string> dict)
    {
       

        string myString = Json.Serialize(dict);
       
        sc_AppWarpKit.sendMove(myString);

    }
    public void OnMoveCompleted(MoveEvent move)// this function is handeling the response from the opponnent
    {
        Debug.Log(this.userName);
		Debug.Log ("user name: " + move.getSender());
		Debug.Log ("next turn: "+move.getNextTurn());
        Debug.Log ("move: " + move.getMoveData());
        if (move.getSender().Contains(userName))
        {
            isMyTurn = false;
        }
        if (move.getNextTurn().Contains(userName))
        {
            isMyTurn = true;
        }
        sc_game.blackTurn = !sc_game.blackTurn;
       
        myRecive(move.getMoveData());
        
    }
    public void myRecive(string myString)
    {

        if (myString == null) myString = "";

        var my_dict = SimpleJSON.JObject.Parse(myString); //this do the deserialization, my_dict = new dictionary
        
        if (my_dict != null)
        {
            _dict = ObjectToDictionary(my_dict);
           sc_game.updateOtherPlayer();
        }
        
    }
    public bool getIsMyTurn()
    {
        return isMyTurn;
    }
    public void changeIsMyTurn()
    {
        isMyTurn = !isMyTurn;
    }
    public void setIsMyTurn(bool t)
    {
        isMyTurn = t;
    }
    public Dictionary<string, string> getDict()
    {
        return _dict;
    }


    public Dictionary<string, string> ObjectToDictionary(SimpleJSON.JObject obj)
    {
        Dictionary<string, string> temp = new Dictionary<string, string>();

        string[] arrayOfStrings;
        arrayOfStrings = obj.ToString().Split(new char[] { '"','{','}',':',',' });
       
        for (var i = 0; i < arrayOfStrings.Length; i++)
        {
            if (arrayOfStrings[i].Contains("pressedPlayer") || arrayOfStrings[i].Contains("nextIsKing") || arrayOfStrings[i].Contains("next") || arrayOfStrings[i].Contains("somthingToEat"))
            {
                temp.Add(arrayOfStrings[i].ToString(), arrayOfStrings[i + 3].ToString());
            }
        }
           
            
       
        return temp;

    }

}



