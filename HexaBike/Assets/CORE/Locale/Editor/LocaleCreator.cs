using UnityEngine;
using UnityEditor;

public class LocaleCreator {

	[MenuItem ("Assets/Create/Locale/LocaleConfig")]
	public static void createLocale() {
		Locale config = ScriptableObject.CreateInstance<Locale>();
		ProjectWindowUtil.CreateAsset(config, "locale.asset");
	}
}
