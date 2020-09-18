using UnityEngine;
using UnityEditor;

public class PowerUpsCreator {
	
	[MenuItem ("Assets/Create/Mountsix/UI/PowerUps")]
	public static void createConfig () {
		PowerUps config = ScriptableObject.CreateInstance<PowerUps>();
		ProjectWindowUtil.CreateAsset(config, "powerUps.asset");
	}
}
