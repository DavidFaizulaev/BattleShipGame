using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SC_slider : MonoBehaviour {

    GameObject slider;
    public Dictionary<string, int> levelDict = new Dictionary<string, int>();
	// Use this for initialization
	void Awake () {
        slider = GameObject.Find("Slider");
       // UIEventListener.Get(slider).onPress += onSlideLevel;
        levelDict.Add("level", 5);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
   /* public void onSlideLevel(GameObject sender, bool state)
    {
        float selection = System.Convert.ToInt32((sender.GetComponent<UISlider>().sliderValue * 10));
        //print(sender.name + " : " + selection);
        GameObject.Find("LabelFaceBookLevel").GetComponent<UILabel>().text = "Level: " + selection ;
        levelDict.Remove("level");
        levelDict.Add("level", selection);
    }*/
    public void OnSliderChange()
    {
        int selection = System.Convert.ToInt32((slider.GetComponent<UISlider>().sliderValue * 10));
        //print(sender.name + " : " + selection);
        GameObject.Find("LabelFaceBookLevel").GetComponent<UILabel>().text = "Level: " + selection;
        levelDict.Remove("level");
        levelDict.Add("level", selection);
    }
    public Dictionary<string, int> getDict()
    {
        return levelDict;
    }
}
