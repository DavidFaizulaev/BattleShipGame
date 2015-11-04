//Unity course Summer 2015 - David Faizulaev
using UnityEngine;
using UnityEngine.UI; // we need This namespace in order to access UI elements within our script
using System.Collections;

//This class is responsible for operating the main menu UI
//All the buttons and text within are displayed using this class as well as transition between levels
public class UI_Script : MonoBehaviour 
{
	//All GUI panels
	public GameObject ParentObject;
	public GameObject MainPanel;
	public GameObject OptionsPanel;
	public GameObject MultiPanel;
	public GameObject InfoPanel;

    void Start ()
    {
        Debug.Log("load UI_Script");
    }

	public void singlePlayerPress()
	{
		Debug.Log("singlePressed");
		StartSinglePlayerLevel ();
	}
	public void MultiPlayerPress() //This function will be used on our Multiplayer button
	{
		Debug.Log ("MultiPlayerPress");
        StartMultiPlayerLevel();
		
	}

	public void StudentInfoPress() //This function will be used on our Student Info button
	{
		Debug.Log ("StudentInfoPress");
		MainPanel.SetActive (false);
		InfoPanel.SetActive (true);
	}

	public void BackPress(GameObject backFromPanel) //This function will be used on our Back button
	{
		Debug.Log ("BackPress");
		backFromPanel.SetActive (false);
		MainPanel.SetActive (true);
	}

	public void OptionsPress() //This function will be used on our Options Info button
	{
		Debug.Log ("OptionsPress");
		MainPanel.SetActive (false);
		InfoPanel.SetActive (false);
		OptionsPanel.SetActive (true);
	}

    public void StartSinglePlayerLevel () //This function will be used on our Play button
    {
		//This will load our single player scene.
	 	Application.LoadLevel ("Singleplayer");
    }
 
    public void StartMultiPlayerLevel () //This function will be used on our Play button
    {
		//This will load our multi player scene.
	 	Application.LoadLevel ("Multiplayer");
    }
 
    public void QuitGamePress () //This function will be used on our "Exit Game" button
    {
		Debug.Log ("ExitGamePress");
  		Application.Quit(); //This will quit our game.  
    }
}