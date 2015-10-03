//Unity course Summer 2015 - David Faizulaev
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// class used to disaply end game message, used by Turn.cs
public class Endgamecanvas : MonoBehaviour
{
	public Text gameresult_text;

	void Start ()
	{
		Debug.Log ("init Endgamecanvas");
	}

	public void DisplayCanvas(int win_code)
	{
		//Debug.Log ("DisplayCanvas");

		string winner_entity = null;
		if (win_code == 1)
						winner_entity = "You Won!";
		if(win_code==2)
						winner_entity = "You Lost!";

		gameresult_text.text =  gameresult_text.text.ToString() + "\n" +  winner_entity;
	}
}