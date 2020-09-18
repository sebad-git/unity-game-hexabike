using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace mountsix.ui{

	public class OptionsMenu : MonoBehaviour {

		public Slider sensitivity;
		public Slider sound;
		public Slider music;

		void Start () {
			sensitivity.value = PlayerPrefs.GetFloat(GameData.SENSITIVITY);
			sound.value = PlayerPrefs.GetFloat(GameData.SOUND);
			music.value = PlayerPrefs.GetFloat(GameData.MUSIC);
		}

		public void close(){
			PlayerPrefs.SetFloat(GameData.SENSITIVITY,sensitivity.value);
			PlayerPrefs.SetFloat(GameData.SOUND,sound.value);
			PlayerPrefs.SetFloat(GameData.MUSIC,music.value);
			SoundManager.updateSound();
			GameObject.FindGameObjectWithTag (Tags.MENU_SOUND_TAG).GetComponent<AudioSource> ().Play ();
			Destroy (gameObject);
		}

	}
}
