using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SC_controller : MonoBehaviour {
  
    public Dictionary<string, GameObject> panelDictionary = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> buttonDictionary = new Dictionary<string, GameObject>();
    public static Dictionary<string, GameObject> photoDictionary = new Dictionary<string, GameObject>();
	// Use this for initialization
	public bool facebook=false;
	public bool guest=false;

	void Start () {
        
		foreach (var gameObj in FindObjectsOfType(typeof(GameObject)) as GameObject[]) {
			if(gameObj.tag=="GUIPanel"){
				//Debug.Log ("panel: "+gameObj.name);
				panelDictionary.Add(gameObj.name, gameObj);
                if (gameObj.name.Contains("loading"))
                    panelDictionary[gameObj.name].SetActive(false);
			}
            if (gameObj.tag == "OutSourcePhoto")
            {
                photoDictionary.Add(gameObj.name, gameObj);
            }
			if(gameObj.tag=="GUIButton"){
				//Debug.Log ("button: "+gameObj.name);
				buttonDictionary.Add(gameObj.name, gameObj);
			}
            if (gameObj.tag == "GameController")
            {
                //Debug.Log ("button: "+gameObj.name);
                buttonDictionary.Add(gameObj.name, gameObj);
            }
		}
		Screen.SetResolution (640, 480, true);
        panelDictionary["pause-background"].SetActive(false);
        buttonDictionary["backToMainMenu"].SetActive(false);
        buttonDictionary["reloadGame"].SetActive(false);

		panelDictionary["PanelFacebook"].SetActive(false);
		panelDictionary["PanelGame"].SetActive(false);
		panelDictionary["PanelBottomMenu"].SetActive(false);
		panelDictionary["PanelGuest"].SetActive(false);

       
           
	}
	
	public void facebookBtn(){ 	
		//user decides to login with facebook
		//if (panelDictionary["PanelLogin"].activeSelf) {						//need to remove quickplay
			panelDictionary["PanelLogin"].SetActive (false);
			panelDictionary["PanelGuest"].SetActive (true);
			panelDictionary["PanelBottomMenu"].SetActive (true);
            panelDictionary["PanelGame"].SetActive(false);
			facebook = true;
		//} 
	}
	public void guestBtn(){ //user decides to login as guest
   
		//if (panelDictionary["PanelLogin"].activeSelf) {						//need to remove the option of singleplayer 
			panelDictionary["PanelLogin"].SetActive (false);
			panelDictionary["PanelGuest"].SetActive (true);
			panelDictionary["PanelBottomMenu"].SetActive (true);
            panelDictionary["PanelGame"].SetActive(false);
			guest=true;
		//}
	}
	public void singlePlayerBtn()
	{
		//if(panelDictionary["PanelGuest"].activeSelf) {
            panelDictionary["PanelLogin"].SetActive(false);
			panelDictionary["PanelGuest"].SetActive(false);
            panelDictionary["PanelBottomMenu"].SetActive(false);
			panelDictionary["PanelGame"].SetActive(true);
		//}
	}
    public void ButtonMultiplayer()
	{
        if ( facebook){
            panelDictionary["PanelLogin"].SetActive(false);
            panelDictionary["PanelGuest"].SetActive(false);
            panelDictionary["PanelGuest"].SetActive(false);
            panelDictionary["PanelFacebook"].SetActive(true);
            
        }
		
	}
	public void playNowbtn()
	{
		//if(panelDictionary["PanelFacebook"].activeSelf) {
            panelDictionary["PanelLogin"].SetActive(false);
            panelDictionary["PanelGuest"].SetActive(false);
			panelDictionary["PanelFacebook"].SetActive(false);
            panelDictionary["PanelBottomMenu"].SetActive(false);
			panelDictionary["PanelGame"].SetActive(true);
			SC_Multiplayer multi = GameObject.Find ("SC_Multiplayer").AddComponent<SC_Multiplayer>() as SC_Multiplayer;
		//}
		
	}
	public void backBtn()
	{
		if (panelDictionary["PanelGuest"].activeSelf) {
			panelDictionary["PanelGuest"].SetActive (false);
			panelDictionary["PanelLogin"].SetActive (true);
			panelDictionary["PanelBottomMenu"].SetActive(false);
            facebook = false;
			guest=false;
		}
		if(panelDictionary["PanelFacebook"].activeSelf){
			panelDictionary["PanelFacebook"].SetActive(false);
			panelDictionary["PanelGuest"].SetActive(true);
		}
	}
    public void ButtonQuickGame()
	{
		//Debug.Log ("virtualCurrencyBtn");
       // if (panelDictionary["PanelGuest"].activeSelf){
            panelDictionary["PanelLogin"].SetActive(false);
            panelDictionary["PanelFacebook"].SetActive(false);
            panelDictionary["PanelGuest"].SetActive(false);
            panelDictionary["PanelBottomMenu"].SetActive(false);
            panelDictionary["PanelGame"].SetActive(true);
       // }
	}
	public void inviteFriendsBtn()
	{
		Debug.Log ("inviteFriendsBtn");
	}
	
	public void addChipBtn()
	{
		Debug.Log ("addChipBtn");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public bool getFacebookState()
    {
        return facebook;
    }
    public bool getGuestState()
    {
        return guest;
    }
    public void whenGameEnd()
    {
        if (facebook)
        {
            panelDictionary["PanelLogin"].SetActive(false);
            panelDictionary["PanelGuest"].SetActive(false);
            panelDictionary["PanelBottomMenu"].SetActive(true);
            panelDictionary["PanelFacebook"].SetActive(true);
            panelDictionary["PanelGame"].SetActive(false);
        }
        else
        {
            panelDictionary["PanelLogin"].SetActive(false);
            panelDictionary["PanelGuest"].SetActive(true);
            panelDictionary["PanelBottomMenu"].SetActive(true);
            panelDictionary["PanelFacebook"].SetActive(false);
            panelDictionary["PanelGame"].SetActive(false);
            
        }
        
    }
}
