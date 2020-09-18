using UnityEngine;
using System.Collections;

public class GameData {

	public const string SENSITIVITY="sensitivity";
	public const string MUSIC="music";
	public const string SOUND="sound";
	//SCORES
	public const string TOTAL_SCORE="totalScore";
	public const string TOTAL_MILES="totalMiles";
	public const string BEST_SCORE="bestScore";
	public const string BEST_MILES="bestMiles";

	//ACHIEVMENTS
	public const string UNLOCKED="unlocked";

	public static void init(){
		if(!PlayerPrefs.HasKey(SENSITIVITY)){PlayerPrefs.SetFloat(SENSITIVITY,360f); }
		if(!PlayerPrefs.HasKey(MUSIC)){PlayerPrefs.SetFloat(MUSIC,1); }
		if(!PlayerPrefs.HasKey(SOUND)){PlayerPrefs.SetFloat(SOUND,0.8f); }
		//SCORES.
		if(!PlayerPrefs.HasKey(TOTAL_SCORE)){PlayerPrefs.SetInt(TOTAL_SCORE,0); }
		if(!PlayerPrefs.HasKey(BEST_SCORE)){PlayerPrefs.SetInt(BEST_SCORE,0); }
		if(!PlayerPrefs.HasKey(BEST_MILES)){PlayerPrefs.SetFloat(BEST_MILES,0); }
		if(!PlayerPrefs.HasKey(TOTAL_MILES)){PlayerPrefs.SetFloat(TOTAL_MILES,0); }
	}

	public static void updateCoins(int coins){
		//TOTAL
		int prevTotal = PlayerPrefs.GetInt(GameData.TOTAL_SCORE);
		PlayerPrefs.SetInt(GameData.TOTAL_SCORE,(prevTotal+coins));
		//BEST
		int prevCoins = PlayerPrefs.GetInt(GameData.BEST_SCORE);
		if(coins>prevCoins){ PlayerPrefs.SetInt(GameData.BEST_SCORE,coins); }
	}

	public static void updateMiles(float miles){
		//TOTAL
		float prevTotal = PlayerPrefs.GetFloat(GameData.TOTAL_MILES);
		PlayerPrefs.SetFloat(GameData.TOTAL_MILES,(prevTotal+miles));
		//BEST
		float prevMiles = PlayerPrefs.GetFloat(GameData.BEST_MILES);
		if(miles>prevMiles){ PlayerPrefs.SetFloat(GameData.BEST_MILES,miles); }
	}

	public static bool unlockAchievment(Achievment achievement){
		if (GooglePlayUI.instance.isConnected()) {
			GooglePlayUI.instance.unlockAchievement(achievement.gpid);
		}
		if(!PlayerPrefs.HasKey(achievement.aid)){ PlayerPrefs.SetString(achievement.aid,UNLOCKED); return true; }
		return false;
	}

	public static bool achievmentUnlocked(string achID){ return PlayerPrefs.HasKey(achID); }

	public static void checkAchievements(AchievmentsConfig achievements, int coins,float miles){
		foreach(Achievment ach in achievements.achievmentsList){
			if(ach.category.Equals(Category.CATEGORY_1)){
				if(coins>=ach.unlockValueInt){GameData.unlockAchievment(ach);}
			}
			if(ach.category.Equals(Category.CATEGORY_2)){
				if(miles>=ach.unlockValueFloat){GameData.unlockAchievment(ach);}
			}
			if(ach.category.Equals(Category.CATEGORY_3)){
				if(miles>=ach.unlockValueFloat && coins==0){GameData.unlockAchievment(ach);}
			}
		}
	}

}
