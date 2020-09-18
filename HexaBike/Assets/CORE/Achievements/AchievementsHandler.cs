using UnityEngine;
using System.Collections;

public class AchievementsHandler : MonoBehaviour {

	public AchievmentsConfig config;
	public static AchievmentsConfig achievements;

	void Awake () { achievements = config; }

}
