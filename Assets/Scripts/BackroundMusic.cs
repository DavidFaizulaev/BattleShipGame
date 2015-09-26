using UnityEngine;
using System.Collections;

public class BackroundMusic : MonoBehaviour {

	Object[] myMusic; // declare this as Object array
	
	void Awake () {
		myMusic = Resources.LoadAll("Music",typeof(AudioClip));
		GetComponent<AudioSource>().clip = myMusic[0] as AudioClip;
	}
	
	void Start (){
		GetComponent<AudioSource>().Play(); 
	}
	
	// Update is called once per frame
	void Update () {
		if(!GetComponent<AudioSource>().isPlaying)
			playRandomMusic();
	}
	
	void playRandomMusic() {
		GetComponent<AudioSource>().clip = myMusic[Random.Range(0,myMusic.Length)] as AudioClip;
		GetComponent<AudioSource>().Play();
	}
}
