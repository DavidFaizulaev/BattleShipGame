//Unity course Summer 2015 - David Faizulaev
using UnityEngine;
using System.Collections;
using System;
using com.shephertz.app42.gaming.multiplayer.client.events;
using com.shephertz.app42.gaming.multiplayer.client.command;
using com.shephertz.app42.gaming.multiplayer.client.listener;
using System.Collections.Generic;
using System.Text.RegularExpressions;

//This class is responsible for performing the connection to the Appwarp server, create/join a room and send 'moves' respectivly between the players.
public class SC_Logic : MonoBehaviour
{

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
    public int game_result;

    public MultiplayerBoardManager myPlayerBoard_script;

    public MultiEnemyBoardManager enemyPlayerBoard_script;

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

    void Start()
    {
        SC_App42Kit.App42Init(apiKey, secretKey);
        SC_AppWarpKit.WarpInit(apiKey, secretKey);

        game_result = -9999;
    }

    void Update()
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

            myPlayerBoard_Ready = true;

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
            myPlayerBoard_script.turn_msg.text = "Waiting for second player to join";
          /*  if (!created_or_joined)
                SC_AppWarpKit.StartGame();*/
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
            
            if (opponentName != userName)
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
            SC_AppWarpKit.CreateTurnBaseRoom("BattleShips" + UnityEngine.Random.Range(0, 1000), userName, 2, null, 60);
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
            isMyTurn = false;
            myPlayerBoard_script.turn_msg.text = "Opponent turn - Places " + SgameInfo.max_number_of_ships + " battleships on their game board";
        }

        //If game room was created by user - then he should initiate game start
        if (!created_or_joined)
        {
            Debug.Log("I created room - i start game - my turn");
            isMyTurn = true;
            myPlayerBoard_script.turn_msg.text = "Your turn - Place " + SgameInfo.max_number_of_ships + " battleships on your game board\n \t\tFirst of 4 squars \n\t\tSecond of 3 squars and so on";
            SC_AppWarpKit.StartGame();
        }
    }

    public void OnPrivateUpdateReceived(string sender, byte[] eventObj, bool fromUdp)
    {
    }

    public void OnGameStarted(string sender, string roomId, string nextTurn)
    {
        Debug.Log("game successfully started");
//        isMyTurn = true;
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
        Debug.Log("move recieved from " + move.getSender());
        Debug.Log("OnMoveCompleted next turn is of player" + " " + move.getNextTurn());
        
        if (move.getSender() != userName)
        {
            Debug.Log("received move   " + move.getMoveData().ToString());
            ParseEnemyMove(move.getMoveData());
        }
    }

    public bool IsItMine()
    {
        return isMyTurn;
    }

    public void MakeMyMove(string str)
    {
        Debug.Log("MakeMyMove !!   " + str);
        isMyTurn = false;
        SC_AppWarpKit.sendMove(str);
    }

    private void ParseEnemyMove(string str)
    {
        Debug.Log("enemy move!!!!!!!!!!!!!!!!!!!!!!         " + str);
        Debug.Log("setting turn to me - true");
        isMyTurn = true;

        //enemy tried to attack battleship
        if (str.Contains("EnemyMove"))
        {
            ConnStater.canWeFight = true;
            myPlayerBoard_script.turn_msg.text = "Opponnet's turn - To attack";
            parseToVector(str);
            myPlayerBoard_script.EnemyMove(vc);
        }
        else
        {
            //other player completed ship structure
            if (str.Contains("structure complete"))
            {
                if (myPlayerBoard_script.first_Completion)
                {
                    Debug.Log("other player completed ship structure - so did you");
                    myPlayerBoard_script.turn_msg.text = "Your turn - Time to attack";
                    ConnStater.canWeFight = true;
                }
                else
                {
                    myPlayerBoard_script.turn_msg.text =  "Your turn - Place " + SgameInfo.max_number_of_ships + " battleships on your game board\n \t\tFirst of 4 squars \n\t\tSecond of 3 squars and so on";
                }
            }

            else
            {
                if (str.Contains("AttackResultSuccess"))
                {
                    Debug.Log("previous attack attempt success");
                    Debug.Log("value of isMyTurn " + isMyTurn);
                    parseToVector(str);
                    enemyPlayerBoard_script.MarkAttackResult(vc,true);
                    Debug.Log("MakeMyMove MarkedAttackResult ");
                    MakeMyMove("MarkedAttackResult");
                }

                else
                {
                    if (str.Contains("AttackResultMiss"))
                    {
                        Debug.Log("previous attack attempt failed");
                        Debug.Log("value of isMyTurn " + isMyTurn);
                        parseToVector(str);
                        enemyPlayerBoard_script.MarkAttackResult(vc,false);
                        Debug.Log("MakeMyMove MarkedAttackResult ");
                        MakeMyMove("MarkedAttackResult");
                    }
                }
            }
        }
    }

    private void parseToVector(string str)
    {
        int aXPosition = -1;
        int aYPosition = -1;
        var regex_sp_chrs = new Regex(@"X:(?<X>10|\d)\s+Y:(?<Y>10|\d)");
        var ms = regex_sp_chrs.Matches(str);
        foreach (Match m in ms)
        {
            aXPosition = (Int32.Parse(m.Groups["X"].Value));
            aYPosition = (Int32.Parse(m.Groups["Y"].Value));
        }

        vc = new Vector2(aXPosition, aYPosition);
    }
}