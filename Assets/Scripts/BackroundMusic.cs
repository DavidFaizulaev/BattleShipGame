using UnityEngine;
using System.Collections;

public class BackroundMusic : MonoBehaviour {

	private Object[] myMusic; // declare this as Object array, basically contains all music used in game.
	
	void Start ()
    {
        myMusic = Resources.LoadAll("Music", typeof(AudioClip));
        GetComponent<AudioSource>().clip = myMusic[0] as AudioClip;
		GetComponent<AudioSource>().Play(); 
	}
	
	// Method is called once per frame
	public void CheckifPlaying () 
    {
        if (!GetComponent<AudioSource>().isPlaying)
                playRandomMusic();
	}
	
	private void playRandomMusic() {
		GetComponent<AudioSource>().clip = myMusic[Random.Range(0,(myMusic.Length-1))] as AudioClip;
		GetComponent<AudioSource>().Play();
	}
}
