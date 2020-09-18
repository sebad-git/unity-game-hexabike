using UnityEngine;
using UnityEditor;

public class AchievmentsCreator {

	[MenuItem ("Assets/Create/Achievements/Achievements File")]
	public static void createConfig () {
		AchievmentsConfig config = ScriptableObject.CreateInstance<AchievmentsConfig>();
		ProjectWindowUtil.CreateAsset(config, "achievments.asset");
	}

}
