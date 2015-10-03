//Unity course Summer 2015 - David Faizulaev
using UnityEngine;
using System.Collections;
using UnityEngine.UI; // we need This namespace in order to access UI elements within our script

//This class is in charge of regulating the turns between the AI and the player.
//variables indicate whose turn is it and valid x,y coordinates for last successful AI attack.
public class Turn : MonoBehaviour
{
	public static bool Pturn;
	public static bool AIturn;
	public static bool Pwon;
	public static bool AIwon;
	private static Turn instance;
	private static GameObject endgamecanvas;

    public static int last_valid_x;
    public static int last_valid_y;

	// Use this for initialization
	void Start ()
	{
		Pturn = true;
		AIturn = false;
		instance = this;

        last_valid_x = (-1);
        last_valid_y = (-1);

		endgamecanvas = GameObject.Find ("WinnerCanvas");
		endgamecanvas.GetComponent<Canvas> ().enabled=false;
	}

	//method used to switch turn between player and AI
	public static void EndTurn (bool x,bool y)
	{
		Pturn = x;
		AIturn = y;
	}

	//Game over - restart level for a new match
	//winner code - 1 - player won
	//winner code - 2 - AI won
	public static void RestartLevel (int winner_code)
	{
		//This will re-load our single player scene.
		//after waiting for 10 seconds
		endgamecanvas.GetComponent<Canvas> ().enabled=true;
		endgamecanvas.GetComponent<Endgamecanvas> ().DisplayCanvas (winner_code);
		instance.StartCoroutine(Wait());
	}

	//coroutine to wait 10 seconds before restarting the level
	private static IEnumerator Wait(){
	
		Debug.Log ("waiting for 10 seconds before restarting level");
		yield return new WaitForSeconds (10.0f);
		Application.LoadLevel ("Singleplayer");
	}

    public static void set_LastValidAttackCrods  (Vector2 v)
    {
        last_valid_x = (int)v.x;
        last_valid_y = (int)v.y;
    }
}

