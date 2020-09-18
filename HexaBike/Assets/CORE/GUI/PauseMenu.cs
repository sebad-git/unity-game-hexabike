using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using mountsix.characters;

namespace mountsix.ui{

	public class PauseMenu : MonoBehaviour {

		public Slider sensitivity;

		void Start () { 
			sensitivity.value = PlayerPrefs.GetFloat(GameData.SENSITIVITY);
			Time.timeScale = 0;
		}

		public void resume() {
			float sens = sensitivity.value;
			PlayerPrefs.SetFloat (GameData.SENSITIVITY, sens);
			BPlayer player = GameObject.FindGameObjectWithTag(Tags.PLAYER_TAG).GetComponent<BPlayer>();
			player.ROTATE_SPEED = sens;
			Time.timeScale = 1; Destroy (gameObject);
		}
		public void quit() { Time.timeScale = 1; Application.LoadLevel(GameScenes.MENU); }

	}

}
