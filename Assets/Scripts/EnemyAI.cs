//Unity course Summer 2015 - David Faizulaev
using UnityEngine;
using System.Collections;

//the class operates the computer AI.
//making a move to ship the player's ship when it's turn.
public class EnemyAI : MonoBehaviour 
{
	public  int[,] enemyShips;
	public PlayerBoardManager myboard;

	private Vector2 vc;

	void Start(){

		myboard = myboard.GetComponent<PlayerBoardManager> ();
	}

	void Update ()
	{	
		if (Turn.AIturn)
		{
			Debug.Log ("AI turn");
			MakeMove ();
			myboard.PlayerMove ();
		}
	}
	private void MakeMove ()
	{
		getRandomLocation ();
		myboard.EnemyMove(vc);
		Turn.EndTurn (true, false);
	}

	private void getRandomLocation ()
	{
		vc = new Vector2 (Random.Range (0, 10), Random.Range (0, 10));
		Debug.Log ("random loc " + vc.x + "   " + vc.y);
	}
}