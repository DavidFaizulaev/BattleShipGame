//Unity course Summer 2015 - David Faizulaev
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//Operation of the Options and SinglePlayer slider button
public class Slider : MonoBehaviour {

	string sliderTextString = "0";
	public Text MscsliderText; // public is needed to ensure it's available to be assigned in the inspector.
	public Text SFXsliderText;
	public Text MsliderText;

	public void MusictextUpdate (float textUpdateNumber)
	{
		sliderTextString = textUpdateNumber.ToString();
		MscsliderText.text = sliderTextString;
	}

	public void SFXtextUpdate (float textUpdateNumber)
	{
		sliderTextString = textUpdateNumber.ToString();
		SFXsliderText.text = sliderTextString;
	}

	public void MtextUpdate (float textUpdateNumber)
	{
		sliderTextString = textUpdateNumber.ToString();
		MsliderText.text = sliderTextString+" $";
	}
}
