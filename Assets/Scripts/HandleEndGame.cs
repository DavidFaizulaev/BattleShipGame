//Unity course Summer 2015 - David Faizulaev
using UnityEngine;
using System.Collections;
using UnityEngine.UI; // we need This namespace in order to access UI elements within our script

//This class is in charge of activating and displaying the end game canvas which displays the game's result.
public class HandleEndGame : MonoBehaviour
{
    public GameObject endgamecanvas;

    // Use this for initialization
    void Start()
    {
        Debug.Log("Start HandleEndGame");
        //endgamecanvas.SetActive(false);
    }

    public void HandleResult(int winner_code)
    {
        endgamecanvas.SetActive(true);
        //endgamecanvas = GameObject.Find ("WinnerCanvas");
        endgamecanvas.GetComponent<Endgamecanvas>().DisplayCanvas(winner_code);
    }
}
