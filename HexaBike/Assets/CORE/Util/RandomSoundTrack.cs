using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]

public class RandomSoundTrack : MonoBehaviour {

	public AudioClip[] music;

	void Start () {
		this.setMusic ();
	}

	private void setMusic(){
		this.StopAllCoroutines();
		this.GetComponent<AudioSource>().clip = music [Random.Range (0, this.music.Length)];
		this.GetComponent<AudioSource>().Play(); this.StartCoroutine(this.changeMusic(this.GetComponent<AudioSource>().clip.length));
	}

	private IEnumerator changeMusic(float wait){
		yield return new WaitForSeconds (wait);
		float volume = this.GetComponent<AudioSource>().volume;
		while(this.GetComponent<AudioSource>().volume>0){this.GetComponent<AudioSource>().volume-=0.2f; yield return null;}
		this.GetComponent<AudioSource>().Stop(); this.GetComponent<AudioSource>().volume = volume;
		this.setMusic();
	}
}
