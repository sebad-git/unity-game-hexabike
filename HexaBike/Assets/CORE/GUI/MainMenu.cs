using UnityEngine;
using System.Collections;

namespace mountsix.ui{

	public class MainMenu : MonoBehaviour {

		public AchievmentsMenu achievmentsMenu;
		public UpgradesMenu upgradesMenu;
		public OptionsMenu optionsMenu;

		void Awake () { 
			AdMobBanner.showBanner(false); SoundManager.updateSound(); 
			GooglePlayUI.OnConnectEvent += this.onGPSConnect;
		}

		void Start () { Screen.sleepTimeout = SleepTimeout.SystemSetting; GameData.init(); }

		public void onGPSConnect(){
			if (GooglePlayUI.instance.isConnected()) {
				foreach(Achievment ach in  AchievementsHandler.achievements.achievmentsList){
					if(GameData.achievmentUnlocked(ach.aid)){
						GooglePlayUI.instance.unlockAchievement(ach.gpid);
					}
				}
			}
		}
		
		public void play(){ Application.LoadLevel (GameScenes.GAME); }

		public void objectives(){ 
			GooglePlayUI.instance.ShowAchievments();
		}

		public void upgrades(){ Instantiate(upgradesMenu); }

		public void options(){ Instantiate(optionsMenu); }

		public void exit(){ Application.Quit(); }

	}
}
