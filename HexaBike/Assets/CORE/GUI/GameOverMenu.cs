using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace mountsix.ui{

	public class GameOverMenu : MonoBehaviour {
		public Text scoreText;
		public Text distanceText;

		void Start () {
			float miles = PlayerPrefs.GetFloat(GameData.BEST_MILES);
			int coins = PlayerPrefs.GetInt(GameData.BEST_SCORE);
			this.scoreText.text = this.scoreText.text + ": " + coins;
			this.distanceText.text = this.distanceText.text + ": " + miles;
			GameData.checkAchievements(AchievementsHandler.achievements,coins,miles);
			long score = GooglePlayUI.instance.loadScore(GPSIDS.leaderboard_best_scores);
			score += (long)(miles * coins);
			GooglePlayUI.instance.uploadScore(GPSIDS.leaderboard_best_scores, score);
			Time.timeScale = 0;
		}

		public void retry () { Time.timeScale = 1; Application.LoadLevel (Application.loadedLevel); }
		public void quit () { Time.timeScale = 1; Application.LoadLevel(GameScenes.MENU); }

	}
}
