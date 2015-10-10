//Unity course Summer 2015 - David Faizulaev
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // we need This namespace in order to access UI elements within our script
using System.Collections;
using System.Text.RegularExpressions;

//This class is responsible for operating the multiplayer UI
//As APPWARP requets a username in order to access the game lobby & room
public class Multi_UI : MonoBehaviour
{
    public GameObject MultiPanel;
    public GameObject MsgPanel;
    public GameObject GamePanel;
    public GameObject Intro_panel;
    public InputField username;

    public Button connect_btn;
    public Button exit_btn;

    public Text response_msg;

    private GameObject appwarp_logic;
    private SC_Logic appwarp_logic_sc;

    void Start()
    {
        Debug.Log("load multi username UI");
    }

    public void connectPressed()
    {
        Debug.Log("ConnectPressed");
        
        string var_username = username.text.ToString();
        var regex_sp_chrs = new Regex("^[a-zA-Z0-9 ]*$");

        if ((!regex_sp_chrs.IsMatch(var_username))||(var_username.Length < 5) || (var_username.Length > 50))
        {
            Debug.Log("no good");
            response_msg.text = "Invalid Input -\n Username must be at least 5 charachters long \nUsername can be maximum of 50 charachters \nUsername cannot contain any special characters like !,@,#,$,%,^,&,*,";
            MsgPanel.SetActive(true);
        }
        else
        {
            Debug.Log("all good");
            response_msg.text = "Connecting to game server";
            MsgPanel.SetActive(true);
            ConnStater.Set_login_ready(true, var_username);
            this.StartCoroutine(Wait());
        }
    }

    //coroutine to wait before starting
    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(5.0f);
        if (ConnStater.Get_connection_status())
        {
            MsgPanel.SetActive(false);
            MultiPanel.SetActive(false);
            GamePanel.SetActive(true);
            Intro_panel.SetActive(true);
        }       
    }

    public void QuitGamePress() //This function will be used on our "Exit Game" button
    {
        Debug.Log("ExitGamePress");
        Application.Quit(); //This will quit our game.  
    }
}