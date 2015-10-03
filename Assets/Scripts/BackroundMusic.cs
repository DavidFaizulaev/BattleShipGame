using UnityEngine;
using System.Collections;

public class BackroundMusic : MonoBehaviour {

	Object[] myMusic; // declare this as Object array, basically contains all music used in game.
	
	void Start ()
    {
        myMusic = Resources.LoadAll("Music", typeof(AudioClip));
        GetComponent<AudioSource>().clip = myMusic[0] as AudioClip;
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
