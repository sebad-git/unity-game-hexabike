using UnityEngine;
using System.Collections;

public class SoundManager {

	public static void updateSound() {
		float sound = PlayerPrefs.GetFloat(GameData.SOUND); 
		float music = PlayerPrefs.GetFloat(GameData.MUSIC);
		AudioSource[] audios = GameObject.FindObjectsOfType<AudioSource>();
		foreach(AudioSource asrc in audios){
			if(asrc.gameObject.tag.Equals(Tags.MUSIC_TAG)){ asrc.volume = music; }
			else{ asrc.volume = sound; }
		}
	}

	public static void mute() {
		AudioSource[] audios = GameObject.FindObjectsOfType<AudioSource>();
		foreach(AudioSource asrc in audios){ asrc.volume = 0; } 
	}

}
