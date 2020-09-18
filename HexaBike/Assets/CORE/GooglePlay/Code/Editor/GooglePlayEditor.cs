using UnityEngine;
using UnityEditor;

public class GooglePlayEditor : MonoBehaviour {

	[MenuItem ("Assets/Create/Google Play/Configuration")]
	public static void createConfig () {
		GooglePlayConfig config = ScriptableObject.CreateInstance<GooglePlayConfig> ();
		ProjectWindowUtil.CreateAsset(config, "GPSConfig.asset");
	}
}
