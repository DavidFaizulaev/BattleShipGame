using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class SC_game : MonoBehaviour {
    public SC_controller sc_controller;
    public SC_Multiplayer sc_multiplayer;
	Dictionary <string,GameObject> players = new Dictionary <string,GameObject>();
	Dictionary <string,GameObject> nextMoveD = new Dictionary <string,GameObject>();
	string pressedPlayer = null;
	ArrayList somthingToEat = new ArrayList();
	ArrayList nextArray = new ArrayList();
	string next = null;
	bool computer = false;
    public bool blackTurn = true;
    bool aginstHumen = true;
    bool aginstComputer = true;
    string lastComputerChosen = "";
    public ParticleSystem myParticle;
	string[][] matrix = Enumerable.Range(0, 8).Select(i=>new string[8]).ToArray();
	// Use this for initialization
	void Awake () {
        if (sc_controller == null)
            sc_controller = GameObject.Find("SC_controller").GetComponent<SC_controller>();
        if (sc_multiplayer == null)
            sc_multiplayer = GameObject.Find("SC_Multiplayer").GetComponent<SC_Multiplayer>();
		foreach (var gameObj in FindObjectsOfType(typeof(GameObject)) as GameObject[]) {
			if(gameObj.tag=="PlayerBlack" || gameObj.tag=="PlayerRed"){
				players.Add(gameObj.name, gameObj);
				// to deactivate the unused player
				if (initGamePlayers(gameObj.name)){
					players[gameObj.name].SetActive(false);
				}
				else{
					initMatrix (gameObj.name);
				}
			}
			else if(gameObj.tag=="NextMove"){
				nextMoveD.Add(gameObj.name, gameObj);
				nextMoveD[gameObj.name].SetActive(false);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
        //if(aginstHumen )
            //updateOtherPlayer();
        if (pressedPlayer != null)
        {
            if (GameObject.Find("blackTurn") != null && GameObject.Find("redTurn") != null && aginstComputer)
            {
                GameObject.Find("blackTurn").GetComponent<UILabel>().text = "Your Turn";
                GameObject.Find("redTurn").GetComponent<UILabel>().text = "";
            }
			 if (pressedPlayer!= null && !players[pressedPlayer].GetComponent<UICheckbox>().isChecked){
				tureOffNextMove();
				pressedPlayer=null;
				return;
			}
			else findPositionToMark(getPlayerPosition (pressedPlayer));
		}
	    else if (computer && aginstComputer){
            if (GameObject.Find("blackTurn") != null && GameObject.Find("redTurn") != null)
            {
                GameObject.Find("blackTurn").GetComponent<UILabel>().text = "";
                GameObject.Find("redTurn").GetComponent<UILabel>().text = "Computer Turn";
            }            		
			doComputerMoves();											        	
		}
        if (aginstHumen && GameObject.Find("blackTurn") != null && GameObject.Find("redTurn")!=null )
        {															       
           if (blackTurn){
              GameObject.Find("blackTurn").GetComponent<UILabel>().text = "Black Turn";
              GameObject.Find("redTurn").GetComponent<UILabel>().text = "";
           }
           else if (!blackTurn)
           {
               GameObject.Find("blackTurn").GetComponent<UILabel>().text = "";
               GameObject.Find("redTurn").GetComponent<UILabel>().text = "White Turn";
           }
        }
	}

	public void updatePressedPlayer(string name){
        if (pressedPlayer != null)
            players[pressedPlayer].GetComponent<UICheckbox>().isChecked = false;
        if (!computer && aginstComputer)
        { 												                    	
            if (pressedPlayer != null )
				players[pressedPlayer].GetComponent<UICheckbox>().isChecked=false;
            if (name.Contains("red"))
            {										                            
			    players[name].GetComponent<UICheckbox>().isChecked=false;	    
			    return;                                                        
		    }															    	
			    pressedPlayer = name;
			    players[pressedPlayer].GetComponent<UICheckbox>().isChecked=true;
			    tureOffNextMove ();
			    next = null;
			    nextArray.Clear();
        }
        else if (aginstComputer)                                               
        {															            
			 players[name].GetComponent<UICheckbox>().isChecked=false;	        
		}
        else if (sc_multiplayer.getIsMyTurn() && aginstHumen && blackTurn)
        {
            if (name.Contains("red"))
            {
                players[name].GetComponent<UICheckbox>().isChecked = false;
                return;
            }
           
                pressedPlayer = name;
                players[pressedPlayer].GetComponent<UICheckbox>().isChecked = true;
                tureOffNextMove();
                next = null;
                nextArray.Clear();           
            
        }
        else if (sc_multiplayer.getIsMyTurn() && aginstHumen && !blackTurn)
        {
            if (name.Contains("black"))
            {
                players[name].GetComponent<UICheckbox>().isChecked = false;
                return;
            }
            
                pressedPlayer = name;
                players[pressedPlayer].GetComponent<UICheckbox>().isChecked = true;
                tureOffNextMove();
                next = null;
                nextArray.Clear();
            
        }
        else
        {
            players[name].GetComponent<UICheckbox>().isChecked = false;
        }
																               
	}

	public bool initGamePlayers(string name){
		int[] pos = getPlayerPosition (name);
		if (pos[0] > 2 && name.Contains("red"))
			return true;
		else if (pos[0] < 5 && name.Contains("black"))
			return true;
		return false;
	}

	public int[] getPlayerPosition(string name){
        if (name == null) return new int [2]{0,0};
		string[] arr = name.Split(' ');
		var x = (arr [1].Substring(0,1));
		var y = (arr [1].Substring(1,1));
		int [] pos = new int[2];
		pos[0]=	int.Parse (x);
		pos[1]=	int.Parse (y);
		return pos;
	}

	public bool checkIfBlack(string name){
		if (name.Contains("black"))
			return true;
		return false;
	}

	public void tureOffNextMove(){
		int redcount = 1;
		int blackcount = 1;
		foreach (GameObject gameObj in nextMoveD.Values) {
			if (gameObj.name.Contains("red")){
				gameObj.name="nextred"+redcount;
				redcount++;
			}
			else if (gameObj.name.Contains("black")){
				gameObj.name="nextblack"+blackcount;
				blackcount++;
			}
			gameObj.SetActive(false);
		}
	}

	public void selectNextMove(string name){
        
		next = name;
        if (players[pressedPlayer].GetComponent<UICheckbox>().transform.FindChild("Background").GetComponent<UISprite>().spriteName.Contains("redKing"))
        {
            players[next].GetComponent<UICheckbox>().checkSprite.spriteName = "redKingPressed";
            players[next].GetComponent<UICheckbox>().transform.FindChild("Background").GetComponent<UISprite>().spriteName = "redKing";
        }
        else if (players[pressedPlayer].GetComponent<UICheckbox>().transform.FindChild("Background").GetComponent<UISprite>().spriteName.Contains("blackKing"))
        {
            players[next].GetComponent<UICheckbox>().checkSprite.spriteName = "blackKingPressed";
            players[next].GetComponent<UICheckbox>().transform.FindChild("Background").GetComponent<UISprite>().spriteName = "blackKing";
        }
        backPlayerToNormal(pressedPlayer);
		players [pressedPlayer].GetComponent<UICheckbox> ().isChecked = false;
		players [pressedPlayer].SetActive (false);
        if(next != null)
		players [next].SetActive (true);
		checkIfKing ();
		updateMatrix ();
		somthingToEat.Clear();
		tureOffNextMove ();
		nextArray.Clear ();
        //blackTurn = !blackTurn;
        //sc_multiplayer.changeIsMyTurn();
		computer = !computer;
	}
	public void initMatrix(string name){

		int[] pos = getPlayerPosition (name);
		matrix[pos[0]][pos[1]] = name;
	}
	public void findPositionToMark(int[] pos){
		int rm = 1;
		int bm = -1;
		if (matrix[pos[0]][pos[1]]==null)return;
        if (matrix[pos[0]][pos[1]].Contains("red") && players[pressedPlayer].GetComponent<UICheckbox>().transform.FindChild("Background").GetComponent<UISprite>().spriteName != "redKing")
        {
			// has 2 options
			if (pos[1]<7 && pos[1]>0 && pos[0]!=7){
				if(matrix[pos[0]+rm][pos[1]-1]==null || matrix[pos[0]+rm][pos[1]+1]==null){
					//Debug.Log("has right and left");
					bool first=true;
					foreach (GameObject player in nextMoveD.Values) {
						if (player.name.Contains("red")){
							if (first){
								if (matrix[pos[0]+rm][pos[1]-1]==null){
									player.SetActive(true);
									var csharp = players["red "+(pos[0]+rm)+(pos[1]-1)].GetComponent("Transform");
									player.transform.position = new Vector3 (csharp.transform.position.x ,csharp.transform.position.y,0);
									player.name = "red "+(pos[0]+rm)+(pos[1]-1);
									nextArray.Add("red "+(pos[0]+rm)+(pos[1]-1));
								}
								first=false;
							}else{
								if (matrix[pos[0]+rm][pos[1]+1]==null){
									player.SetActive(true);
									var csharp = players["red "+(pos[0]+rm)+(pos[1]+1)].GetComponent("Transform");
									player.transform.position = new Vector3 (csharp.transform.position.x ,csharp.transform.position.y,0);
									player.name = "red "+(pos[0]+rm)+(pos[1]+1);
									nextArray.Add("red "+(pos[0]+rm)+(pos[1]+1));
									break;
								}
							}
						}
					}
				}
				checkIfCanEatPlayer();
			}
			//has only left
			else if (pos[1]==7 && pos[0]!=7){
				if(matrix[pos[0]+rm][pos[1]-1]==null){
					foreach (GameObject player in nextMoveD.Values) {
						if (player.name.Contains("red")){
							player.SetActive(true);
							var csharp = players["red "+(pos[0]+rm)+(pos[1]-1)].GetComponent("Transform");
							player.transform.position = new Vector3 (csharp.transform.position.x ,csharp.transform.position.y,0);
							player.name = "red "+(pos[0]+rm)+(pos[1]-1);
							nextArray.Add( "red "+(pos[0]+rm)+(pos[1]-1));
							break;
						}
					}
				}
				checkIfCanEatPlayerLeft();
			}
			//has only right
			else if (pos[1]==0 && pos[0]!=7){
				if(matrix[pos[0]+rm][pos[1]+1]==null){
					foreach (GameObject player in nextMoveD.Values) {
						if (player.name.Contains("red")){
							player.SetActive(true);
							var csharp = players["red "+(pos[0]+rm)+(pos[1]+1)].GetComponent("Transform");
							player.transform.position = new Vector3 (csharp.transform.position.x ,csharp.transform.position.y,0);
							player.name = "red "+(pos[0]+rm)+(pos[1]+1);
							nextArray.Add("red "+(pos[0]+rm)+(pos[1]+1));
							break;
						}
					}
				}
				checkIfCanEatPlayerRight();
			}

        }
        else if (players[pressedPlayer].GetComponent<UICheckbox>().transform.FindChild("Background").GetComponent<UISprite>().spriteName == "redKing")
        {
			// need to add king function to eat back left and back right
			doKingMoves();

		}else if (matrix[pos[0]][pos[1]].Contains("black")&& players[pressedPlayer].GetComponent<UICheckbox>().transform.FindChild ("Background").GetComponent<UISprite> ().spriteName != "blackKing"){
			// has 2 options
			if (pos[1]<7 && pos[1]>0 && pos[0]!=0){
				if(matrix[pos[0]+bm][pos[1]-1]==null || matrix[pos[0]+bm][pos[1]+1]==null){
					//Debug.Log("has right and left");
					bool first=true;
					foreach (GameObject player in nextMoveD.Values) {
						if (player.name.Contains("black")){
							if (first){
								if(matrix[pos[0]+bm][pos[1]-1]==null)
								{
									player.SetActive(true);
									var csharp = players["black "+(pos[0]+bm)+(pos[1]-1)].GetComponent("Transform");
									player.transform.position = new Vector3 (csharp.transform.position.x ,csharp.transform.position.y,0);
									player.name = "black "+(pos[0]+bm)+(pos[1]-1);
                                    nextArray.Add("black " + (pos[0] + bm) + (pos[1] - 1));
								}
								first=false;
							}else{
								if(matrix[pos[0]+bm][pos[1]+1]==null)
								{
									player.SetActive(true);
									var csharp = players["black "+(pos[0]+bm)+(pos[1]+1)].GetComponent("Transform");
									player.transform.position = new Vector3 (csharp.transform.position.x ,csharp.transform.position.y,0);
									player.name = "black "+(pos[0]+bm)+(pos[1]+1);
                                    nextArray.Add("black " + (pos[0] + bm) + (pos[1] + 1));
									break;
								}
							}
						}
					}
				}
				checkIfCanEatPlayer();
			}
			//has only left
			else if (pos[1]==7 && pos[0]!=0){
				if(matrix[pos[0]+bm][pos[1]-1]==null){
					foreach (GameObject player in nextMoveD.Values) {
						if (player.name.Contains("black")){
							player.SetActive(true);
							var csharp = players["black "+(pos[0]+bm)+(pos[1]-1)].GetComponent("Transform");
							player.transform.position = new Vector3 (csharp.transform.position.x ,csharp.transform.position.y,0);
							player.name = "black "+(pos[0]+bm)+(pos[1]-1);
                            nextArray.Add("black "+(pos[0]+bm)+(pos[1]-1));
							break;
						}
					}
				}
				checkIfCanEatPlayerLeft();
			}
			//has only right
			else if (pos[1]==0 && pos[0]!=0){
				if(matrix[pos[0]+bm][pos[1]+1]==null){
					foreach (GameObject player in nextMoveD.Values) {
						if (player.name.Contains("black")){
							player.SetActive(true);
							var csharp = players["black "+(pos[0]+bm)+(pos[1]+1)].GetComponent("Transform");
							player.transform.position = new Vector3 (csharp.transform.position.x ,csharp.transform.position.y,0);
							player.name = "black "+(pos[0]+bm)+(pos[1]+1);
                            nextArray.Add( "black " + (pos[0] + bm) + (pos[1] - 1));
							break;
						}
					}
				}
				checkIfCanEatPlayerRight();
			}
        }
        else if (players[pressedPlayer].GetComponent<UICheckbox>().transform.FindChild("Background").GetComponent<UISprite>().spriteName == "blackKing")
        {
			// need to add king function to eat back left and back right
			doKingMoves();
		}

	}
	public void checkIfCanEatPlayer (){
		int [] pos = getPlayerPosition (pressedPlayer);
		if (pressedPlayer.Contains("red")){
			if (matrix[pos[0]+1][pos[1]-1]!=null && matrix[pos[0]+1][pos[1]-1].Contains("black"))
				checkIfCanEatPlayerLeft();
			if (matrix[pos[0]+1][pos[1]+1]!=null &&matrix[pos[0]+1][pos[1]+1].Contains("black"))
				checkIfCanEatPlayerRight();
		}else if (pressedPlayer.Contains("black")){
			if (matrix[pos[0]-1][pos[1]-1]!=null &&matrix[pos[0]-1][pos[1]-1].Contains("red"))
				checkIfCanEatPlayerLeft();
			if (matrix[pos[0]-1][pos[1]+1]!=null &&matrix[pos[0]-1][pos[1]+1].Contains("red"))
				checkIfCanEatPlayerRight();
		}
	}
	public void checkIfCanEatPlayerRight (){
		int [] pos = getPlayerPosition (pressedPlayer);
		if (pressedPlayer.Contains("red")){
			if (pos[0]<6 && pos[1]<6){
				if (matrix[pos[0]+1][pos[1]+1]!=null && matrix[pos[0]+1][pos[1]+1].Contains("black") && matrix[pos[0]+2][pos[1]+2]==null ){
					foreach (GameObject player in nextMoveD.Values) {
						if (player.name.Contains("nextred")){
							player.SetActive(true);
							var csharp = players["red "+(pos[0]+2)+(pos[1]+2)].GetComponent("Transform");
							player.transform.position = new Vector3 (csharp.transform.position.x ,csharp.transform.position.y,0);
							player.name = "red "+(pos[0]+2)+(pos[1]+2);
                            //if is king
                            nextArray.Insert(0,"red "+(pos[0]+2)+(pos[1]+2));
                            nextArray.Add("red " + (pos[0] + 2) + (pos[1] + 2));
                            somthingToEat.Add(player.name);
                            somthingToEat.Add("black "+(pos[0]+1)+(pos[1]+1));
							break;
						}
					}
				}
			}
		}
		else if (pressedPlayer.Contains("black")){
			if (pos[0]>1 && pos[1]<6){
				if (matrix[pos[0]-1][pos[1]+1]!=null && matrix[pos[0]-1][pos[1]+1].Contains("red")&&matrix[pos[0]-2][pos[1]+2]==null){
					foreach (GameObject player in nextMoveD.Values) {
						if (player.name.Contains("nextblack")){
							player.SetActive(true);
							var csharp = players["black "+(pos[0]-2)+(pos[1]+2)].GetComponent("Transform");
							player.transform.position = new Vector3 (csharp.transform.position.x ,csharp.transform.position.y,0);
							player.name = "black "+(pos[0]-2)+(pos[1]+2);
                            nextArray.Add("black " + (pos[0] - 2) + (pos[1] + 2));
							somthingToEat.Add(player.name);
							somthingToEat.Add("red "+(pos[0]-1)+(pos[1]+1));
							break;
						}
					}
				}
			}
		}
	}
	public void checkIfCanEatPlayerLeft (){
		int [] pos = getPlayerPosition (pressedPlayer);
		if (pressedPlayer.Contains("red")){
			if (pos[0]<6 && pos[1]>1){
				if (matrix[pos[0]+1][pos[1]-1]!=null && matrix[pos[0]+1][pos[1]-1].Contains("black")&& matrix[pos[0]+2][pos[1]-2]==null){
					foreach (GameObject player in nextMoveD.Values) {
						if (player.name.Contains("nextred")){
							player.SetActive(true);
							var csharp = players["red "+(pos[0]+2)+(pos[1]-2)].GetComponent("Transform");
							player.transform.position = new Vector3 (csharp.transform.position.x ,csharp.transform.position.y,0);
							player.name = "red "+(pos[0]+2)+(pos[1]-2);
                            // if is king
                            nextArray.Insert(0,"red "+(pos[0]+2)+(pos[1]-2));
                            nextArray.Add("red " + (pos[0] + 2) + (pos[1] - 2));
                            somthingToEat.Add(player.name);
                            somthingToEat.Add("black "+(pos[0]+1)+(pos[1]-1));
							break;
						}
					}
				}
			}
		}
		else if (pressedPlayer.Contains("black")){
			if (pos[0]>1 && pos[1]>1){
				if (matrix[pos[0]-1][pos[1]-1]!=null && matrix[pos[0]-1][pos[1]-1].Contains("red") && matrix[pos[0]-2][pos[1]-2]==null){
					foreach (GameObject player in nextMoveD.Values) {
						if (player.name.Contains("nextblack")){
							player.SetActive(true);
							var csharp = players["black "+(pos[0]-2)+(pos[1]-2)].GetComponent("Transform");
							player.transform.position = new Vector3 (csharp.transform.position.x ,csharp.transform.position.y,0);
							player.name = "black "+(pos[0]-2)+(pos[1]-2);
                            nextArray.Add("black " + (pos[0] - 2) + (pos[1] - 2));
							somthingToEat.Add(player.name);
							somthingToEat.Add("red "+(pos[0]-1)+(pos[1]-1));
							break;
						}
					}
				}
			}
		}
	}
	public void updateMatrix (){

        Dictionary<string, string> dict = new Dictionary<string, string>();
		int[] pos = getPlayerPosition (next);
		matrix[pos[0]][pos[1]] = next;
		pos = getPlayerPosition (pressedPlayer);
        dict.Add("pressedPlayer", pressedPlayer);
        if (players[next].GetComponent<UICheckbox>().transform.FindChild("Background").GetComponent<UISprite>().spriteName.Contains("King"))
        {
            dict.Add("nextIsKing", next);
        }
        else 
        {
            dict.Add("next", next);
        }
		matrix[pos[0]][pos[1]] = null;
		for (var i=0 ; i < somthingToEat.Count;i++){
			if (next.Equals(somthingToEat[i].ToString())){
				pos = getPlayerPosition((string)somthingToEat[i+1]);
				matrix[pos[0]][pos[1]] = null;
				backPlayerToNormal((string)somthingToEat[i+1]);
				players[(string)somthingToEat[i+1]].SetActive(false);
                dict.Add("somthingToEat", somthingToEat[i + 1].ToString());// need to back player to normal
                //dict.Add("backPlayerToNormal", somthingToEat[i + 1].ToString());
                activatePartical(somthingToEat[i + 1].ToString());
				break;
			}
		}
        sc_multiplayer.mySend(dict);
		string isOver = checkIfGameIsOver ();
		if (isOver != ""){
            EditorUtility.DisplayDialog("The Game Is Over", isOver + " Win", "OK");
			Debug.Log("The Game Is Over "+isOver+" Win");
            reloadGame();
            sc_controller.whenGameEnd();
		}
	}
    public void activatePartical(string player)
    {
        //Instantiate(myParticle, players[player].GetComponent<Transform>().position,new Quaternion(0, -180, -90, 0));
    }
    public void updateOtherPlayer()
    {
        //if (sc_multiplayer._dict.Count>1)
        foreach (string player in sc_multiplayer._dict.Keys)
            {
                if (player == "pressedPlayer")
                {
                    Debug.Log("updateOtherPlayer pressedPlayer:" + players[sc_multiplayer._dict[player]].name);
                    int[] pos = getPlayerPosition(sc_multiplayer._dict[player]);
                    matrix[pos[0]][pos[1]] = null;
                    players[sc_multiplayer._dict[player]].SetActive(false);
                }
                else if (player == "nextIsKing")
                {
                    Debug.Log("updateOtherPlayer nextIsKing:" + players[sc_multiplayer._dict[player]].name);
                    int[] pos = getPlayerPosition(sc_multiplayer._dict[player]);
                    matrix[pos[0]][pos[1]] = sc_multiplayer._dict[player];
                    if (player.Contains("red"))
                    {
                        players[sc_multiplayer._dict[player]].GetComponent<UICheckbox>().checkSprite.spriteName = "redKingPressed";//redKingPressed
                        players[sc_multiplayer._dict[player]].GetComponent<UICheckbox>().transform.FindChild("Background").GetComponent<UISprite>().spriteName = "redKing";
                    }
                    else
                    {
                        players[sc_multiplayer._dict[player]].GetComponent<UICheckbox>().checkSprite.spriteName = "blackKingPressed";//redKingPressed
                        players[sc_multiplayer._dict[player]].GetComponent<UICheckbox>().transform.FindChild("Background").GetComponent<UISprite>().spriteName = "blackKing";
                    }
                    players[sc_multiplayer._dict[player]].SetActive(true);
                }
                else if (player == "next")
                {
                    Debug.Log("updateOtherPlayer next:" + players[sc_multiplayer._dict[player]].name);
                    int[] pos = getPlayerPosition(sc_multiplayer._dict[player]);
                    matrix[pos[0]][pos[1]] = sc_multiplayer._dict[player];
                    players[sc_multiplayer._dict[player]].SetActive(true);
                }
                else if (player == "somthingToEat")
                {
                    Debug.Log("updateOtherPlayer somthingToEat:" + players[sc_multiplayer._dict[player]].name);
                    int[] pos = getPlayerPosition(sc_multiplayer._dict[player]);
                    matrix[pos[0]][pos[1]] = null;
                    backPlayerToNormal(players[sc_multiplayer._dict[player]].name);
                    players[sc_multiplayer._dict[player]].SetActive(false);
                }
            }
            sc_multiplayer.getDict().Clear();
    }
	public string checkIfGameIsOver(){
		int red = 0;
		int black = 0;
		for (var i=matrix.Length-1 ;i>=0;i--){
			for (var j=matrix.Length-1 ;j>=0;j--){
				if (matrix[i][j]!=null && matrix[i][j].Contains("red"))
					red++;
				if (matrix[i][j]!=null && matrix[i][j].Contains("black"))
					black++;
			}
		}
		if (red == 0)return "Black";
		if (black == 0) return "White";
		return "";
	}

	public void checkIfKing(){
		// if king change selectedPlayer to background and checkmark 
		if (next.Contains("red")&&getPlayerPosition(next)[0]==7){ // problem
			string temp=next;
            players[next].GetComponent<UICheckbox>().checkSprite.spriteName = "redKingPressed";//redKingPressed
			players [next].GetComponent<UICheckbox> ().transform.FindChild ("Background").GetComponent<UISprite> ().spriteName = "redKing";
		}
		else if (next.Contains("black")&&getPlayerPosition(next)[0]==0){
			string temp=next;
            players[next].GetComponent<UICheckbox>().checkSprite.spriteName = "blackKingPressed";//redKingPressed
			players [next].GetComponent<UICheckbox> ().transform.FindChild ("Background").GetComponent<UISprite> ().spriteName = "blackKing";
		}

	}
	// computer is red
	public void doComputerMoves(){
		//computer STEPS
		getComputerSelection ();
		if(pressedPlayer==null){
			computer=false;
			return;
		}
		selectNextMove (next);
		updateMatrix ();
	}
	public void getComputerSelection(){
      
            for (var i = matrix.Length - 1; i >= 0; i--)
            {
                for (var j = matrix.Length - 1; j >= 0; j--)
                {
                    if (matrix[i][j] != null && matrix[i][j].Contains("red"))
                    {
                        pressedPlayer = "red " + i.ToString() + j.ToString();
                        findPositionToMark(getPlayerPosition(pressedPlayer));
                        next = getBestNextMove();
                        if (next != null)
                        {
                            if (somthingToEat.Count > 0)
                            {
                                return;
                            }
                            else if (lastComputerChosen != pressedPlayer)
                            {
                                lastComputerChosen = next;
                                return;
                            }
                            else
                            {
                                nextArray.Clear();
                            }
                            
                        }
                    }
                }
            }
            pressedPlayer = lastComputerChosen;
	}
	public string getBestNextMove (){
        //Debug.Log(Random.Range(0,nextArray.Count));
		if (nextArray.Count>0)return nextArray[Random.Range(0,nextArray.Count)].ToString();
		return null;
	}

	public void backPlayerToNormal(string name){
       
        if (name.Contains("red")){
			players [name].GetComponent<UICheckbox> ().checkSprite.spriteName = "redPressed";
			players [name].GetComponent<UICheckbox> ().transform.FindChild ("Background").GetComponent<UISprite> ().spriteName = "red";
		}
        else if (name.Contains("black"))
        {
			players [name].GetComponent<UICheckbox> ().checkSprite.spriteName = "blackPressed";
			players [name].GetComponent<UICheckbox> ().transform.FindChild ("Background").GetComponent<UISprite> ().spriteName = "black";
		}

	}
	public void doKingMoves(){
		//STEPS
		/*
			check if can eat
			send the selected choice
		*/
		int[] pos = getPlayerPosition(pressedPlayer);
		if (pressedPlayer.Contains("red")){
            getNextPosKing(pos, "red");
			
		}
		else{
            getNextPosKing(pos,"black");
			
		}
	}
    public void getNextPosKing(int []pos,string color)
    {
        if (color == "black")
        {
            if (pos[0] != 7 && pos[0] != 0 && pos[1] != 0 && pos[1] != 7)
            { // has 4 options
                if (matrix[pos[0] + 1][pos[1] - 1] == null || matrix[pos[0] + 1][pos[1] + 1] == null ||
                    matrix[pos[0] - 1][pos[1] - 1] == null || matrix[pos[0] - 1][pos[1] + 1] == null)
                {
                    //Debug.Log("has right and left");
                    int first = 0;
                    foreach (GameObject player in nextMoveD.Values)
                    {
                        if (player.name.Contains("black"))
                        {
                            if (first == 0)
                            {
                                if (matrix[pos[0] + 1][pos[1] - 1] == null)
                                {
                                    player.SetActive(true);
                                    var csharp = players["black " + (pos[0] + 1) + (pos[1] - 1)].GetComponent("Transform");
                                    player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                    player.name = "black " + (pos[0] + 1) + (pos[1] - 1);
                                    nextArray.Add("black " + (pos[0] + 1) + (pos[1] - 1));
                                }
                                else if (matrix[pos[0] + 1][pos[1] - 1].Contains("red") && pos[0] < 6 && pos[1] > 1 && matrix[pos[0] + 2][pos[1] - 2]==null)
                                {// can eat from top to bottom -- left 
                                    player.SetActive(true);
                                    var csharp = players["black " + (pos[0] + 2) + (pos[1] - 2)].GetComponent("Transform");
                                    player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                    player.name = "black " + (pos[0] + 2) + (pos[1] - 2);
                                    nextArray.Add("black " + (pos[0] + 2) + (pos[1] - 2));
                                    somthingToEat.Add(player.name);
                                    somthingToEat.Add("red " + (pos[0] + 1) + (pos[1] - 1));
                                    
                                }
                                first++;
                            }
                            else if (first == 1)
                            {
                                if (matrix[pos[0] + 1][pos[1] + 1] == null)
                                {
                                    player.SetActive(true);
                                    var csharp = players["black " + (pos[0] + 1) + (pos[1] + 1)].GetComponent("Transform");
                                    player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                    player.name = "black " + (pos[0] + 1) + (pos[1] + 1);
                                    nextArray.Add("black " + (pos[0] + 1) + (pos[1] + 1));
                                }
                                else if (matrix[pos[0] + 1][pos[1] + 1].Contains("red") && pos[0] < 6 && pos[1] < 6 && matrix[pos[0] + 2][pos[1] + 2] == null)
                                {// can eat from top to bottom -- right 
                                    player.SetActive(true);
                                    var csharp = players["black " + (pos[0] + 2) + (pos[1] + 2)].GetComponent("Transform");
                                    player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                    player.name = "black " + (pos[0] + 2) + (pos[1] + 2);
                                    nextArray.Add("black " + (pos[0] + 2) + (pos[1] + 2));
                                    somthingToEat.Add(player.name);
                                    somthingToEat.Add("red " + (pos[0] + 1) + (pos[1] + 1));
                                    
                                }
                                first++;
                            }
                            else if (first == 2)
                            {
                                if (matrix[pos[0] - 1][pos[1] + 1] == null)
                                {
                                    player.SetActive(true);
                                    var csharp = players["black " + (pos[0] - 1) + (pos[1] + 1)].GetComponent("Transform");
                                    player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                    player.name = "black " + (pos[0] - 1) + (pos[1] + 1);
                                    nextArray.Add("black " + (pos[0] - 1) + (pos[1] + 1));
                                }
                                else if (matrix[pos[0] - 1][pos[1] + 1].Contains("red") && pos[0] > 1 && pos[1] < 6 && matrix[pos[0] - 2][pos[1] + 2] == null)
                                {// can eat from bottom to top -- right 
                                    player.SetActive(true);
                                    var csharp = players["black " + (pos[0] - 2) + (pos[1] + 2)].GetComponent("Transform");
                                    player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                    player.name = "black " + (pos[0] - 2) + (pos[1] + 2);
                                    nextArray.Add("black " + (pos[0] - 2) + (pos[1] + 2));
                                    somthingToEat.Add(player.name);
                                    somthingToEat.Add("red " + (pos[0] - 1) + (pos[1] + 1));
                                    
                                }
                                first++;
                            }
                            else if (first == 3)
                            {
                                if (matrix[pos[0] - 1][pos[1] - 1] == null)
                                {
                                    player.SetActive(true);
                                    var csharp = players["black " + (pos[0] - 1) + (pos[1] - 1)].GetComponent("Transform");
                                    player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                    player.name = "black " + (pos[0] - 1) + (pos[1] - 1);
                                    nextArray.Add("black " + (pos[0] - 1) + (pos[1] - 1));
                                }
                                else if (matrix[pos[0] - 1][pos[1] - 1].Contains("red") && pos[0] > 1 && pos[1] > 1 && matrix[pos[0] - 2][pos[1] - 2] == null)
                                {// can eat from bottom to top -- left 
                                    player.SetActive(true);
                                    var csharp = players["black " + (pos[0] - 2) + (pos[1] - 2)].GetComponent("Transform");
                                    player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                    player.name = "black " + (pos[0] - 2) + (pos[1] - 2);
                                    nextArray.Add("black " + (pos[0] - 2) + (pos[1] - 2));
                                    somthingToEat.Add(player.name);
                                    somthingToEat.Add("red " + (pos[0] - 1) + (pos[1] - 1));
                                    
                                }
                                break;
                            }
                        }
                    }
                }
            }
            if (pos[0] == 7 && pos[1] != 0 && pos[1] != 7)
            { // has 2 options from bottom
                int first = 0;
                foreach (GameObject player in nextMoveD.Values)
                {
                    if (player.name.Contains("black"))
                    {
                        if (first == 0)
                        {
                            if (matrix[pos[0] - 1][pos[1] - 1] == null)
                            {
                                player.SetActive(true);
                                var csharp = players["black " + (pos[0] - 1) + (pos[1] - 1)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "black " + (pos[0] - 1) + (pos[1] - 1);
                                nextArray.Add("black " + (pos[0] - 1) + (pos[1] - 1));
                            }
                            else if (matrix[pos[0] - 1][pos[1] - 1].Contains("red") && pos[0] > 1 && pos[1] > 1 && matrix[pos[0] - 2][pos[1] - 2] == null)
                            {// can eat from bottom to top -- left 
                                player.SetActive(true);
                                var csharp = players["black " + (pos[0] - 2) + (pos[1] - 2)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "black " + (pos[0] - 2) + (pos[1] - 2);
                                nextArray.Add("black " + (pos[0] - 2) + (pos[1] - 2));
                                somthingToEat.Add(player.name);
                                somthingToEat.Add("red " + (pos[0] - 1) + (pos[1] - 1));
                              
                            }
                            first++;
                        }
                        else if (first == 1)
                        {
                            if (matrix[pos[0] - 1][pos[1] + 1] == null)
                            {
                                player.SetActive(true);
                                var csharp = players["black " + (pos[0] - 1) + (pos[1] + 1)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "black " + (pos[0] - 1) + (pos[1] + 1);
                                nextArray.Add("black " + (pos[0] - 1) + (pos[1] + 1));
                            }
                            else if (matrix[pos[0] - 1][pos[1] + 1].Contains("red") && pos[0] > 1 && pos[1] < 6 && matrix[pos[0] - 2][pos[1] + 2] == null)
                            {// can eat from bottom to top -- right 
                                player.SetActive(true);
                                var csharp = players["black " + (pos[0] - 2) + (pos[1] + 2)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "black " + (pos[0] - 2) + (pos[1] + 2);
                                nextArray.Add("black " + (pos[0] - 2) + (pos[1] + 2));
                                somthingToEat.Add(player.name);
                                somthingToEat.Add("red " + (pos[0] - 1) + (pos[1] + 1));
                                
                            }
                            break;
                        }
                    }
                }
            }
            if (pos[0] == 0 && pos[1] != 0 && pos[1] != 7)
            { // has 2 options from top
                int first = 0;
                foreach (GameObject player in nextMoveD.Values)
                {
                    if (player.name.Contains("black"))
                    {
                        if (first == 0)
                        {
                            if (matrix[pos[0] + 1][pos[1] - 1] == null)
                            {
                                player.SetActive(true);
                                var csharp = players["black " + (pos[0] + 1) + (pos[1] - 1)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "black " + (pos[0] + 1) + (pos[1] - 1);
                                nextArray.Add("black " + (pos[0] + 1) + (pos[1] - 1));
                            }
                            else if (matrix[pos[0] + 1][pos[1] - 1].Contains("red") && pos[0] < 6 && pos[1] > 1 && matrix[pos[0] + 2][pos[1] - 2] == null)
                            {// can eat from top to bottom -- left 
                                player.SetActive(true);
                                var csharp = players["black " + (pos[0] + 2) + (pos[1] - 2)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "black " + (pos[0] + 2) + (pos[1] - 2);
                                nextArray.Add("black " + (pos[0] + 2) + (pos[1] - 2));
                                somthingToEat.Add(player.name);
                                somthingToEat.Add("red " + (pos[0] + 1) + (pos[1] - 1));
                                
                            }
                            first++;
                        }
                        else if (first == 1)
                        {
                            if (matrix[pos[0] + 1][pos[1] + 1] == null)
                            {
                                player.SetActive(true);
                                var csharp = players["black " + (pos[0] + 1) + (pos[1] + 1)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "black " + (pos[0] + 1) + (pos[1] + 1);
                                nextArray.Add("black " + (pos[0] + 1) + (pos[1] + 1));
                            }
                            else if (matrix[pos[0] + 1][pos[1] + 1].Contains("red") && pos[0] < 6 && pos[1] < 6 && matrix[pos[0] + 2][pos[1] + 2] == null)
                            {// can eat from top to bottom -- right 
                                player.SetActive(true);
                                var csharp = players["black " + (pos[0] + 2) + (pos[1] + 2)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "black " + (pos[0] + 2) + (pos[1] + 2);
                                nextArray.Add("black " + (pos[0] + 2) + (pos[1] + 2));
                                somthingToEat.Add(player.name);
                                somthingToEat.Add("red " + (pos[0] + 1) + (pos[1] + 1));
                                
                            }
                            break;
                        }
                    }
                }
            }
            else if (pos[0] == 7 && pos[1] == 0)
            { // one option go to top right
                int first = 0;
                foreach (GameObject player in nextMoveD.Values)
                {
                    if (player.name.Contains("black"))
                    {
                        if (first == 0)
                        {
                            if (matrix[pos[0] - 1][pos[1] + 1] == null)
                            {
                                player.SetActive(true);
                                var csharp = players["black " + (pos[0] - 1) + (pos[1] + 1)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "black " + (pos[0] - 1) + (pos[1] + 1);
                                nextArray.Add("black " + (pos[0] - 1) + (pos[1] + 1));
                            }
                            else if (matrix[pos[0] - 1][pos[1] + 1].Contains("red") && pos[0] > 1 && pos[1] < 6 && matrix[pos[0] - 2][pos[1] + 2] == null)
                            {// can eat from bottom to top -- right 
                                player.SetActive(true);
                                var csharp = players["black " + (pos[0] - 2) + (pos[1] + 2)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "black " + (pos[0] - 2) + (pos[1] + 2);
                                nextArray.Add("black " + (pos[0] - 2) + (pos[1] + 2));
                                somthingToEat.Add(player.name);
                                somthingToEat.Add("red " + (pos[0] - 1) + (pos[1] + 1));
                                
                            }
                            break;
                        }

                    }
                }
            }
            else if (pos[0] == 7 && pos[1] == 7)
            {// one option go to top left
                int first = 0;
                foreach (GameObject player in nextMoveD.Values)
                {
                    if (player.name.Contains("black"))
                    {
                        if (first == 0)
                        {
                            if (matrix[pos[0] - 1][pos[1] - 1] == null)
                            {
                                player.SetActive(true);
                                var csharp = players["black " + (pos[0] - 1) + (pos[1] - 1)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "black " + (pos[0] - 1) + (pos[1] - 1);
                                nextArray.Add("black " + (pos[0] - 1) + (pos[1] - 1));
                            }
                            else if (matrix[pos[0] - 1][pos[1] - 1].Contains("red") && pos[0] > 1 && pos[1] > 1 && matrix[pos[0] - 2][pos[1] - 2] == null)
                            {// can eat from bottom to top -- left 
                                player.SetActive(true);
                                var csharp = players["black " + (pos[0] - 2) + (pos[1] - 2)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "black " + (pos[0] - 2) + (pos[1] - 2);
                                nextArray.Add("black " + (pos[0] - 2) + (pos[1] - 2));
                                somthingToEat.Add(player.name);
                                somthingToEat.Add("red " + (pos[0] - 1) + (pos[1] - 1));
                                
                            }
                            break;
                        }

                    }
                }
            }
            else if (pos[0] == 0 && pos[1] == 0)
            { // one option go to bottom right
                int first = 0;
                foreach (GameObject player in nextMoveD.Values)
                {
                    if (player.name.Contains("black"))
                    {
                        if (first == 0)
                        {
                            if (matrix[pos[0] + 1][pos[1] + 1] == null)
                            {
                                player.SetActive(true);
                                var csharp = players["black " + (pos[0] + 1) + (pos[1] + 1)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "black " + (pos[0] + 1) + (pos[1] + 1);
                                nextArray.Add("black " + (pos[0] + 1) + (pos[1] + 1));
                            }
                            else if (matrix[pos[0] + 1][pos[1] + 1].Contains("red") && pos[0] < 6 && pos[1] < 6 && matrix[pos[0] + 2][pos[1] + 2] == null)
                            {// can eat from top to bottom -- right 
                                player.SetActive(true);
                                var csharp = players["black " + (pos[0] + 2) + (pos[1] + 2)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "black " + (pos[0] + 2) + (pos[1] + 2);
                                nextArray.Add("black " + (pos[0] + 2) + (pos[1] + 2));
                                somthingToEat.Add(player.name);
                                somthingToEat.Add("red " + (pos[0] + 1) + (pos[1] + 1));
                                
                            }
                            break;
                        }

                    }
                }
            }
            else if (pos[0] == 0 && pos[1] == 7)
            {// one option go to bottom left
                int first = 0;
                foreach (GameObject player in nextMoveD.Values)
                {
                    if (player.name.Contains("black"))
                    {
                        if (first == 0)
                        {
                            if (matrix[pos[0] + 1][pos[1] - 1] == null)
                            {
                                player.SetActive(true);
                                var csharp = players["black " + (pos[0] + 1) + (pos[1] - 1)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "black " + (pos[0] + 1) + (pos[1] - 1);
                                nextArray.Add("black " + (pos[0] + 1) + (pos[1] - 1));
                            }
                            else if (matrix[pos[0] + 1][pos[1] - 1].Contains("red") && pos[0] < 6 && pos[1] > 1 && matrix[pos[0] + 2][pos[1] - 2] == null)
                            {// can eat from top to bottom -- left 
                                player.SetActive(true);
                                var csharp = players["black " + (pos[0] + 2) + (pos[1] - 2)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "black " + (pos[0] + 2) + (pos[1] - 2);
                                nextArray.Add("black " + (pos[0] + 2) + (pos[1] - 2));
                                somthingToEat.Add(player.name);
                                somthingToEat.Add("red " + (pos[0] + 1) + (pos[1] - 1));
                                
                            }
                            break;
                        }

                    }
                }
            }
            else if (pos[0] != 7 && pos[0] != 0 && pos[1] == 7)
            {// two options go to left
                int first = 0;
                foreach (GameObject player in nextMoveD.Values)
                {
                    if (player.name.Contains("black"))
                    {
                        if (first == 0)
                        {
                            if (matrix[pos[0] + 1][pos[1] - 1] == null)
                            {
                                player.SetActive(true);
                                var csharp = players["black " + (pos[0] + 1) + (pos[1] - 1)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "black " + (pos[0] + 1) + (pos[1] - 1);
                                nextArray.Add("black " + (pos[0] + 1) + (pos[1] - 1));
                            }
                            else if (matrix[pos[0] + 1][pos[1] - 1].Contains("red") && pos[0] < 6 && pos[1] > 1 && matrix[pos[0] + 2][pos[1] - 2] == null)
                            {// can eat from top to bottom -- left 
                                player.SetActive(true);
                                var csharp = players["black " + (pos[0] + 2) + (pos[1] - 2)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "black " + (pos[0] + 2) + (pos[1] - 2);
                                nextArray.Add("black " + (pos[0] + 2) + (pos[1] - 2));
                                somthingToEat.Add(player.name);
                                somthingToEat.Add("red " + (pos[0] + 1) + (pos[1] - 1));
                                
                            }
                            first++;
                        }
                        else if (first == 1)
                        {
                            if (matrix[pos[0] - 1][pos[1] - 1] == null)
                            {
                                player.SetActive(true);
                                var csharp = players["black " + (pos[0] - 1) + (pos[1] - 1)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "black " + (pos[0] - 1) + (pos[1] - 1);
                                nextArray.Add("black " + (pos[0] - 1) + (pos[1] - 1));
                            }
                            else if (matrix[pos[0] - 1][pos[1] - 1].Contains("red") && pos[0] > 1 && pos[1] > 1 && matrix[pos[0] - 2][pos[1] - 2] == null)
                            {// can eat from bottom to top -- left 
                                player.SetActive(true);
                                var csharp = players["black " + (pos[0] - 2) + (pos[1] - 2)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "black " + (pos[0] - 2) + (pos[1] - 2);
                                nextArray.Add("black " + (pos[0] - 2) + (pos[1] - 2));
                                somthingToEat.Add(player.name);
                                somthingToEat.Add("red " + (pos[0] - 1) + (pos[1] - 1));
                                
                            }
                            break;
                        }
                    }
                }
            }
            else if (pos[0] != 7 && pos[0] != 0 && pos[1] == 0)
            {// two options go to rigth
                int first = 0;
                foreach (GameObject player in nextMoveD.Values)
                {
                    if (player.name.Contains("black"))
                    {
                        if (first == 0)
                        {
                            if (matrix[pos[0] + 1][pos[1] + 1] == null)
                            {
                                player.SetActive(true);
                                var csharp = players["black " + (pos[0] + 1) + (pos[1] + 1)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "black " + (pos[0] + 1) + (pos[1] + 1);
                                nextArray.Add("black " + (pos[0] + 1) + (pos[1] + 1));
                            }
                            else if (matrix[pos[0] + 1][pos[1] + 1].Contains("red") && pos[0] < 6 && pos[1] < 6 && matrix[pos[0] + 2][pos[1] + 2] == null)
                            {// can eat from top to bottom -- right 
                                player.SetActive(true);
                                var csharp = players["black " + (pos[0] + 2) + (pos[1] + 2)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "black " + (pos[0] + 2) + (pos[1] + 2);
                                nextArray.Add("black " + (pos[0] + 2) + (pos[1] + 2));
                                somthingToEat.Add(player.name);
                                somthingToEat.Add("red " + (pos[0] + 1) + (pos[1] + 1));
                               
                            }
                            first++;
                        }
                        else if (first == 1)
                        {
                            if (matrix[pos[0] - 1][pos[1] + 1] == null)
                            {
                                player.SetActive(true);
                                var csharp = players["black " + (pos[0] - 1) + (pos[1] + 1)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "black " + (pos[0] - 1) + (pos[1] + 1);
                                nextArray.Add("black " + (pos[0] - 1) + (pos[1] + 1));
                            }
                            else if (matrix[pos[0] - 1][pos[1] + 1].Contains("red") && pos[0] > 1 && pos[1] < 6 && matrix[pos[0] - 2][pos[1] + 2] == null)
                            {// can eat from bottom to top -- right 
                                player.SetActive(true);
                                var csharp = players["black " + (pos[0] - 2) + (pos[1] + 2)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "black " + (pos[0] - 2) + (pos[1] + 2);
                                nextArray.Add("black " + (pos[0] - 2) + (pos[1] + 2));
                                somthingToEat.Add(player.name);
                                somthingToEat.Add("red " + (pos[0] - 1) + (pos[1] + 1));
                                
                            }
                            break;
                        }
                    }
                }
            }
        }
        else if (color == "red")
        {
            if (pos[0] != 7 && pos[0] != 0 && pos[1] != 0 && pos[1] != 7)
            { // has 4 options
                if (matrix[pos[0] + 1][pos[1] - 1] == null || matrix[pos[0] + 1][pos[1] + 1] == null ||
                    matrix[pos[0] - 1][pos[1] - 1] == null || matrix[pos[0] - 1][pos[1] + 1] == null)
                {
                    //Debug.Log("has right and left");
                    int first = 0;
                    foreach (GameObject player in nextMoveD.Values)
                    {
                        if (player.name.Contains("red"))
                        {
                            if (first == 0)
                            {
                                if (matrix[pos[0] + 1][pos[1] - 1] == null)
                                {
                                    player.SetActive(true);
                                    var csharp = players["red " + (pos[0] + 1) + (pos[1] - 1)].GetComponent("Transform");
                                    player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                    player.name = "red " + (pos[0] + 1) + (pos[1] - 1);
                                    nextArray.Add("red " + (pos[0] + 1) + (pos[1] - 1));
                                }
                                else if (matrix[pos[0] + 1][pos[1] - 1].Contains("black") && pos[0] < 6 && pos[1] > 1 && matrix[pos[0] + 2][pos[1] - 2] == null)
                                {// can eat from top to bottom -- left 
                                    player.SetActive(true);
                                    var csharp = players["red " + (pos[0] + 2) + (pos[1] - 2)].GetComponent("Transform");
                                    player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                    player.name = "red " + (pos[0] + 2) + (pos[1] - 2);
                                    nextArray.Add("red " + (pos[0] + 2) + (pos[1] - 2));
                                    somthingToEat.Add(player.name);
                                    somthingToEat.Add("black " + (pos[0] + 1) + (pos[1] - 1));
                                    
                                }
                                first++;
                            }
                            else if (first == 1)
                            {
                                if (matrix[pos[0] + 1][pos[1] + 1] == null)
                                {
                                    player.SetActive(true);
                                    var csharp = players["red " + (pos[0] + 1) + (pos[1] + 1)].GetComponent("Transform");
                                    player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                    player.name = "red " + (pos[0] + 1) + (pos[1] + 1);
                                    nextArray.Add("red " + (pos[0] + 1) + (pos[1] + 1));
                                }
                                else if (matrix[pos[0] + 1][pos[1] + 1].Contains("black") && pos[0] < 6 && pos[1] < 6 && matrix[pos[0] + 2][pos[1] + 2] == null)
                                {// can eat from top to bottom -- right 
                                    player.SetActive(true);
                                    var csharp = players["red " + (pos[0] + 2) + (pos[1] + 2)].GetComponent("Transform");
                                    player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                    player.name = "red " + (pos[0] + 2) + (pos[1] + 2);
                                    nextArray.Add("red " + (pos[0] + 2) + (pos[1] + 2));
                                    somthingToEat.Add(player.name);
                                    somthingToEat.Add("black " + (pos[0] + 1) + (pos[1] + 1));
                                    
                                }
                                first++;
                            }
                            else if (first == 2)
                            {
                                if (matrix[pos[0] - 1][pos[1] + 1] == null)
                                {
                                    player.SetActive(true);
                                    var csharp = players["red " + (pos[0] - 1) + (pos[1] + 1)].GetComponent("Transform");
                                    player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                    player.name = "red " + (pos[0] - 1) + (pos[1] + 1);
                                    nextArray.Add("red " + (pos[0] - 1) + (pos[1] + 1));
                                }
                                else if (matrix[pos[0] - 1][pos[1] + 1].Contains("black") && pos[0] > 1 && pos[1] < 6 && matrix[pos[0] - 2][pos[1] + 2] == null)
                                {// can eat from bottom to top -- right 
                                    player.SetActive(true);
                                    var csharp = players["red " + (pos[0] - 2) + (pos[1] + 2)].GetComponent("Transform");
                                    player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                    player.name = "red " + (pos[0] - 2) + (pos[1] + 2);
                                    nextArray.Add("red " + (pos[0] - 2) + (pos[1] + 2));
                                    somthingToEat.Add(player.name);
                                    somthingToEat.Add("black " + (pos[0] - 1) + (pos[1] + 1));
                                    
                                }
                                first++;
                            }
                            else if (first == 3)
                            {
                                if (matrix[pos[0] - 1][pos[1] - 1] == null)
                                {
                                    player.SetActive(true);
                                    var csharp = players["red " + (pos[0] - 1) + (pos[1] - 1)].GetComponent("Transform");
                                    player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                    player.name = "red " + (pos[0] - 1) + (pos[1] - 1);
                                    nextArray.Add("red " + (pos[0] - 1) + (pos[1] - 1));
                                }
                                else if (matrix[pos[0] - 1][pos[1] - 1].Contains("black") && pos[0] > 1 && pos[1] > 1 && matrix[pos[0] - 2][pos[1] - 2] == null)
                                {// can eat from bottom to top -- left 
                                    player.SetActive(true);
                                    var csharp = players["red " + (pos[0] - 2) + (pos[1] - 2)].GetComponent("Transform");
                                    player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                    player.name = "red " + (pos[0] - 2) + (pos[1] - 2);
                                    nextArray.Add("red " + (pos[0] - 2) + (pos[1] - 2));
                                    somthingToEat.Add(player.name);
                                    somthingToEat.Add("black " + (pos[0] - 1) + (pos[1] - 1));
                                    
                                }
                                break;
                            }
                        }
                    }
                }
            }
            if (pos[0] == 7 && pos[1] != 0 && pos[1] != 7)
            { // has 2 options from bottom
                int first = 0;
                foreach (GameObject player in nextMoveD.Values)
                {
                    if (player.name.Contains("red"))
                    {
                        if (first == 0)
                        {
                            if (matrix[pos[0] - 1][pos[1] - 1] == null)
                            {
                                player.SetActive(true);
                                var csharp = players["red " + (pos[0] - 1) + (pos[1] - 1)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "red " + (pos[0] - 1) + (pos[1] - 1);
                                nextArray.Add("red " + (pos[0] - 1) + (pos[1] - 1));
                            }
                            else if (matrix[pos[0] - 1][pos[1] - 1].Contains("black") && pos[0] > 1 && pos[1] > 1 && matrix[pos[0] - 2][pos[1] - 2] == null)
                            {// can eat from bottom to top -- left 
                                player.SetActive(true);
                                var csharp = players["red " + (pos[0] - 2) + (pos[1] - 2)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "red " + (pos[0] - 2) + (pos[1] - 2);
                                nextArray.Add("red " + (pos[0] - 2) + (pos[1] - 2));
                                somthingToEat.Add(player.name);
                                somthingToEat.Add("black " + (pos[0] - 1) + (pos[1] - 1));
                                
                            }
                            first++;
                        }
                        else if (first == 1)
                        {
                            if (matrix[pos[0] - 1][pos[1] + 1] == null)
                            {
                                player.SetActive(true);
                                var csharp = players["red " + (pos[0] - 1) + (pos[1] + 1)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "red " + (pos[0] - 1) + (pos[1] + 1);
                                nextArray.Add("red " + (pos[0] - 1) + (pos[1] + 1));
                            }
                            else if (matrix[pos[0] - 1][pos[1] + 1].Contains("black") && pos[0] > 1 && pos[1] < 6 && matrix[pos[0] - 2][pos[1] + 2] == null)
                            {// can eat from bottom to top -- right 
                                player.SetActive(true);
                                var csharp = players["red " + (pos[0] - 2) + (pos[1] + 2)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "red " + (pos[0] - 2) + (pos[1] + 2);
                                nextArray.Add("red " + (pos[0] - 2) + (pos[1] + 2));
                                somthingToEat.Add(player.name);
                                somthingToEat.Add("black " + (pos[0] - 1) + (pos[1] + 1));
                                
                            }
                            break;
                        }
                    }
                }
            }
            if (pos[0] == 0 && pos[1] != 0 && pos[1] != 7)
            { // has 2 options from top
                int first = 0;
                foreach (GameObject player in nextMoveD.Values)
                {
                    if (player.name.Contains("red"))
                    {
                        if (first == 0)
                        {
                            if (matrix[pos[0] + 1][pos[1] - 1] == null)
                            {
                                player.SetActive(true);
                                var csharp = players["red " + (pos[0] + 1) + (pos[1] - 1)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "red " + (pos[0] + 1) + (pos[1] - 1);
                                nextArray.Add("red " + (pos[0] + 1) + (pos[1] - 1));
                            }
                            else if (matrix[pos[0] + 1][pos[1] - 1].Contains("black") && pos[0] < 6 && pos[1] > 1 && matrix[pos[0] + 2][pos[1] - 2] == null)
                            {// can eat from top to bottom -- left 
                                player.SetActive(true);
                                var csharp = players["red " + (pos[0] + 2) + (pos[1] - 2)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "red " + (pos[0] + 2) + (pos[1] - 2);
                                nextArray.Add("red " + (pos[0] + 2) + (pos[1] - 2));
                                somthingToEat.Add(player.name);
                                somthingToEat.Add("black " + (pos[0] + 1) + (pos[1] - 1));
                                
                            }
                            first++;
                        }
                        else if (first == 1)
                        {
                            if (matrix[pos[0] + 1][pos[1] + 1] == null)
                            {
                                player.SetActive(true);
                                var csharp = players["red " + (pos[0] + 1) + (pos[1] + 1)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "red " + (pos[0] + 1) + (pos[1] + 1);
                                nextArray.Add("red " + (pos[0] + 1) + (pos[1] + 1));
                            }
                            else if (matrix[pos[0] + 1][pos[1] + 1].Contains("black") && pos[0] < 6 && pos[1] < 6 && matrix[pos[0] + 2][pos[1] + 2] == null)
                            {// can eat from top to bottom -- right 
                                player.SetActive(true);
                                var csharp = players["red " + (pos[0] + 2) + (pos[1] + 2)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "red " + (pos[0] + 2) + (pos[1] + 2);
                                nextArray.Add("red " + (pos[0] + 2) + (pos[1] + 2));
                                somthingToEat.Add(player.name);
                                somthingToEat.Add("black " + (pos[0] + 1) + (pos[1] + 1));
                                
                            }
                            break;
                        }
                    }
                }
            }
            else if (pos[0] == 7 && pos[1] == 0)
            { // one option from top right
                int first = 0;
                foreach (GameObject player in nextMoveD.Values)
                {
                    if (player.name.Contains("red"))
                    {
                        if (first == 0)
                        {
                            if (matrix[pos[0] - 1][pos[1] + 1] == null)
                            {
                                player.SetActive(true);
                                var csharp = players["red " + (pos[0] - 1) + (pos[1] + 1)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "red " + (pos[0] - 1) + (pos[1] + 1);
                                nextArray.Add("red " + (pos[0] - 1) + (pos[1] + 1));
                            }
                            else if (matrix[pos[0] - 1][pos[1] + 1].Contains("black") && pos[0] > 1 && pos[1] < 6 && matrix[pos[0] - 2][pos[1] + 2] == null)
                            {// can eat from bottom to top -- right 
                                player.SetActive(true);
                                var csharp = players["red " + (pos[0] - 2) + (pos[1] + 2)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "red " + (pos[0] - 2) + (pos[1] + 2);
                                nextArray.Add("red " + (pos[0] - 2) + (pos[1] + 2));
                                somthingToEat.Add(player.name);
                                somthingToEat.Add("black " + (pos[0] - 1) + (pos[1] + 1));
                               
                            }
                            break;
                        }

                    }
                }
            }
            else if (pos[0] == 7 && pos[1] == 7)
            {// one option from top left
                int first = 0;
                foreach (GameObject player in nextMoveD.Values)
                {
                    if (player.name.Contains("red"))
                    {
                        if (first == 0)
                        {
                            if (matrix[pos[0] - 1][pos[1] - 1] == null)
                            {
                                player.SetActive(true);
                                var csharp = players["red " + (pos[0] - 1) + (pos[1] - 1)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "red " + (pos[0] - 1) + (pos[1] - 1);
                                nextArray.Add("red " + (pos[0] - 1) + (pos[1] - 1));
                            }
                            else if (matrix[pos[0] - 1][pos[1] - 1].Contains("black") && pos[0] > 1 && pos[1] > 1 && matrix[pos[0] - 2][pos[1] - 2] == null)
                            {// can eat from bottom to top -- left 
                                player.SetActive(true);
                                var csharp = players["red " + (pos[0] - 2) + (pos[1] - 2)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "red " + (pos[0] - 2) + (pos[1] - 2);
                                nextArray.Add("red " + (pos[0] - 2) + (pos[1] - 2));
                                somthingToEat.Add(player.name);
                                somthingToEat.Add("black " + (pos[0] - 1) + (pos[1] - 1));
                                
                            }
                            break;
                        }

                    }
                }
            }
            else if (pos[0] == 0 && pos[1] == 0)
            { // one option from bottom right
                int first = 0;
                foreach (GameObject player in nextMoveD.Values)
                {
                    if (player.name.Contains("red"))
                    {
                        if (first == 0)
                        {
                            if (matrix[pos[0] + 1][pos[1] + 1] == null)
                            {
                                player.SetActive(true);
                                var csharp = players["red " + (pos[0] + 1) + (pos[1] + 1)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "red " + (pos[0] + 1) + (pos[1] + 1);
                                nextArray.Add("red " + (pos[0] + 1) + (pos[1] + 1));
                            }
                            else if (matrix[pos[0] + 1][pos[1] + 1].Contains("black") && pos[0] < 6 && pos[1] < 6 && matrix[pos[0] + 2][pos[1] + 2] == null)
                            {// can eat from top to bottom -- right 
                                player.SetActive(true);
                                var csharp = players["red " + (pos[0] + 2) + (pos[1] + 2)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "red " + (pos[0] + 2) + (pos[1] + 2);
                                nextArray.Add("red " + (pos[0] + 2) + (pos[1] + 2));
                                somthingToEat.Add(player.name);
                                somthingToEat.Add("black " + (pos[0] + 1) + (pos[1] + 1));
                               
                            }
                            break;
                        }

                    }
                }
            }
            else if (pos[0] == 0 && pos[1] == 7)
            {// one option from bottom left
                int first = 0;
                foreach (GameObject player in nextMoveD.Values)
                {
                    if (player.name.Contains("red"))
                    {
                        if (first == 0)
                        {
                            if (matrix[pos[0] + 1][pos[1] - 1] == null)
                            {
                                player.SetActive(true);
                                var csharp = players["red " + (pos[0] + 1) + (pos[1] - 1)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "red " + (pos[0] + 1) + (pos[1] - 1);
                                nextArray.Add("red " + (pos[0] + 1) + (pos[1] - 1));
                            }
                            else if (matrix[pos[0] + 1][pos[1] - 1].Contains("black") && pos[0] < 6 && pos[1] > 1 && matrix[pos[0] + 2][pos[1] - 2] == null)
                            {// can eat from top to bottom -- left 
                                player.SetActive(true);
                                var csharp = players["red " + (pos[0] + 2) + (pos[1] - 2)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "red " + (pos[0] + 2) + (pos[1] - 2);
                                nextArray.Add("red " + (pos[0] + 2) + (pos[1] - 2));
                                somthingToEat.Add(player.name);
                                somthingToEat.Add("black " + (pos[0] + 1) + (pos[1] - 1));
                               
                            }
                            break;
                        }

                    }
                }
            }
            else if (pos[0] != 7 && pos[0] != 0 && pos[1] == 7)
            {// two options from left
                int first = 0;
                foreach (GameObject player in nextMoveD.Values)
                {
                    if (player.name.Contains("red"))
                    {
                        if (first == 0)
                        {
                            if (matrix[pos[0] + 1][pos[1] - 1] == null)
                            {
                                player.SetActive(true);
                                var csharp = players["red " + (pos[0] + 1) + (pos[1] - 1)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "red " + (pos[0] + 1) + (pos[1] - 1);
                                nextArray.Add("red " + (pos[0] + 1) + (pos[1] - 1));
                            }
                            else if (matrix[pos[0] + 1][pos[1] - 1].Contains("black") && pos[0] < 6 && pos[1] > 1 && matrix[pos[0] + 2][pos[1] - 2] == null)
                            {// can eat from top to bottom -- left 
                                player.SetActive(true);
                                var csharp = players["red " + (pos[0] + 2) + (pos[1] - 2)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "red " + (pos[0] + 2) + (pos[1] - 2);
                                nextArray.Add("red " + (pos[0] + 2) + (pos[1] - 2));
                                somthingToEat.Add(player.name);
                                somthingToEat.Add("black " + (pos[0] + 1) + (pos[1] - 1));
                               
                            }
                            first++;
                        }
                        else if (first == 1)
                        {
                            if (matrix[pos[0] - 1][pos[1] - 1] == null)
                            {
                                player.SetActive(true);
                                var csharp = players["red " + (pos[0] - 1) + (pos[1] - 1)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "red " + (pos[0] - 1) + (pos[1] - 1);
                                nextArray.Add("red " + (pos[0] - 1) + (pos[1] - 1));
                            }
                            else if (matrix[pos[0] - 1][pos[1] - 1].Contains("black") && pos[0] > 1 && pos[1] > 1 && matrix[pos[0] - 2][pos[1] - 2] == null)
                            {// can eat from bottom to top -- left 
                                player.SetActive(true);
                                var csharp = players["red " + (pos[0] - 2) + (pos[1] - 2)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "red " + (pos[0] - 2) + (pos[1] - 2);
                                nextArray.Add("red " + (pos[0] - 2) + (pos[1] - 2));
                                somthingToEat.Add(player.name);
                                somthingToEat.Add("black " + (pos[0] - 1) + (pos[1] - 1));
                                
                            }
                            break;
                        }
                    }
                }
            }
            else if (pos[0] != 7 && pos[0] != 0 && pos[1] == 0)
            {// two options from rigth
                int first = 0;
                foreach (GameObject player in nextMoveD.Values)
                {
                    if (player.name.Contains("red"))
                    {
                        if (first == 0)
                        {
                            if (matrix[pos[0] + 1][pos[1] + 1] == null)
                            {
                                player.SetActive(true);
                                var csharp = players["red " + (pos[0] + 1) + (pos[1] + 1)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "red " + (pos[0] + 1) + (pos[1] + 1);
                                nextArray.Add("red " + (pos[0] + 1) + (pos[1] + 1));
                            }
                            else if (matrix[pos[0] + 1][pos[1] + 1].Contains("black") && pos[0] < 6 && pos[1] < 6 && matrix[pos[0] + 2][pos[1] + 2] == null)
                            {// can eat from top to bottom -- right 
                                player.SetActive(true);
                                var csharp = players["red " + (pos[0] + 2) + (pos[1] + 2)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "red " + (pos[0] + 2) + (pos[1] + 2);
                                nextArray.Add("red " + (pos[0] + 2) + (pos[1] + 2));
                                somthingToEat.Add(player.name);
                                somthingToEat.Add("black " + (pos[0] + 1) + (pos[1] + 1));
                               
                            }
                            first++;
                        }
                        else if (first == 1)
                        {
                            if (matrix[pos[0] - 1][pos[1] + 1] == null)
                            {
                                player.SetActive(true);
                                var csharp = players["red " + (pos[0] - 1) + (pos[1] + 1)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "red " + (pos[0] - 1) + (pos[1] + 1);
                                nextArray.Add("red " + (pos[0] - 1) + (pos[1] + 1));
                            }
                            else if (matrix[pos[0] - 1][pos[1] + 1].Contains("black") && pos[0] > 1 && pos[1] < 6 && matrix[pos[0] - 2][pos[1] + 2] == null)
                            {// can eat from bottom to top -- right 
                                player.SetActive(true);
                                var csharp = players["red " + (pos[0] - 2) + (pos[1] + 2)].GetComponent("Transform");
                                player.transform.position = new Vector3(csharp.transform.position.x, csharp.transform.position.y, 0);
                                player.name = "red " + (pos[0] - 2) + (pos[1] + 2);
                                nextArray.Add("red " + (pos[0] - 2) + (pos[1] + 2));
                                somthingToEat.Add(player.name);
                                somthingToEat.Add("black " + (pos[0] - 1) + (pos[1] + 1));
                            }
                            break;
                        }
                    }
                }
            }
        }
    }
    public void reloadGame(){
        matrix = Enumerable.Range(0, 8).Select(i => new string[8]).ToArray();
        pressedPlayer = null;
        somthingToEat.Clear();
        lastComputerChosen = "";
        nextArray.Clear();
        next = null;
        computer = false;
        blackTurn = true;
        //sc_multiplayer.setIsMyTurn(true);
        foreach (var gameObj in players.Values)
        {
            gameObj.SetActive(true);
            backPlayerToNormal(gameObj.name);
            gameObj.GetComponent<UICheckbox>().isChecked = false;
            if (initGamePlayers(gameObj.name))
            {
                players[gameObj.name].SetActive(false);
            }
            else
            {
                initMatrix(gameObj.name);
            }
        }
        foreach (var gameObj in nextMoveD.Values)
        {
            gameObj.SetActive(false);
        }
    }
    public void setHuman(bool option)
    {
        aginstHumen = option;
        aginstComputer = !option;
        Debug.Log("against humen");
    }
    public void setComputer(bool option)
    {
        aginstHumen = !option;
        aginstComputer = option;
        Debug.Log("against computer");
    }
}