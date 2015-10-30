//Unity course Summer 2015 - David Faizulaev
using UnityEngine;
using System.Collections;

//This is an utility class, which holds static variables in regards to the connection state, login and so on.
public class ConnStater : MonoBehaviour {

    private static bool login_ready;
    private static bool connection_status;
    private static string usrnm;
    private static GameObject endgamecanvas;
    public static bool attacker; //true - I'm attacking, Else - False

	// Use this for initialization
	void Start () 
    {
        login_ready = false;
        connection_status = false;
        attacker = false;
        usrnm = "";

        endgamecanvas = GameObject.Find("WinnerCanvas");
        endgamecanvas.GetComponent<Canvas>().enabled = false;
	}
	
    public static void Set_login_ready (bool state,string usr_name)
    {
        login_ready = state;
        usrnm = usr_name;
    }

    public static bool Get_login_ready()
    {
        return login_ready;
    }

    public static bool Get_connection_status()
    {
        return connection_status;
    }

    public static void Set__connection_status(bool state)
    {
        connection_status = state;
    }

    public static string get_Username()
    {
        return usrnm;
    }

    //Game over
    //winner code - 1 - player won
    //winner code - 2 - oppponet won
    public static void set_Game_Result (int winner_code)
    {
        if (winner_code == 1)
             Debug.Log("Congratulations, You've won the game!");
         else
             Debug.Log("Too bad, You've lost the game!");
        
         endgamecanvas.GetComponent<Canvas>().enabled = true;
         endgamecanvas.GetComponent<Endgamecanvas>().DisplayCanvas(winner_code);
    }
}
