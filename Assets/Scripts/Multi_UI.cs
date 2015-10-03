﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // we need This namespace in order to access UI elements within our script
using System.Collections;


//This class is responsible for operating the multiplayer UI
//As APPWARP requets a username in order to access the game lobby & room
public class Multi_UI : MonoBehaviour
{
    public GameObject MultiPanel;
    public GameObject MsgPanel;
    public InputField username;
    
    public Button connect_btn;
    public Text response_msg;

    void Start()
    {
        Debug.Log("load multi username UI");
    }

    public void connectPressed()
    {
        Debug.Log("ConnectPressed");
        string var_username = username.text.ToString();

        if ((var_username.Length < 5) || (var_username.Length > 50))
        {
            Debug.Log("no good");
            response_msg.text = "Invalid Input -\n Username must be at least 5 charachters long \nUsername can be maximum of 50 charachters";
            MsgPanel.SetActive(true);
        }
        else
        {
            Debug.Log("all good");
            response_msg.text = "Connecting to game server";
            MsgPanel.SetActive(true);
        }
    }
}
