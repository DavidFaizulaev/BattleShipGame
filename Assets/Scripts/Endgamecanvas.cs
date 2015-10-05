//Unity course Summer 2015 - David Faizulaev
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//The class used to disaply end game message, used by HandleEndGame
public class Endgamecanvas : MonoBehaviour
{
	public Text gameresult_text;

	void Start ()
	{
		Debug.Log ("init Endgamecanvas");
	}

	public void DisplayCanvas(int win_code)
	{
        string str = null;
		if (win_code == 1)
			str = "You Won!";
		if(win_code==2)
            str = "You Lost!";

        gameresult_text.text = gameresult_text.text.ToString() + "\n" + str;
	}
}