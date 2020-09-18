using UnityEngine;
using System.Collections;

[System.Serializable]
public class PowerUp {
	private static string languageID = "NONE";
	private const string DEFAULT_LANGUAGE="english";
	private const string WORD_NOT_FOUND="Keyword not Found.";

	public string powerUpName;
	public System.Collections.Generic.List<UpgradeTranslation> titles;
	public System.Collections.Generic.List<UpgradeTranslation> descriptions;
	public string pwpID;
	public int price;
	public Sprite icon;
	public UpgradeCategory category;
	public UpgradeType type;
	//VALUES
	public GameObject prefab;
	public float value1;
	public float maxUpgrade;
	public string upgradeAchievment;
	public string useAchievment;
	public bool active;
	public bool showTitles;
	public bool showDescriptions;
	private static System.Predicate<UpgradeTranslation> DEFAULT_FINDER = (UpgradeTranslation trans) => { return trans.language.ToString().ToLower() == DEFAULT_LANGUAGE.ToLower(); };

	public float createEffect(){
		float destroyTime=((PlayerPrefs.GetInt(this.pwpID)+1) * this.value1);
		if(prefab!=null){
			GameObject nEffect = (GameObject)GameObject.Instantiate(prefab);
			nEffect.transform.parent = Camera.main.transform; nEffect.transform.localPosition = Vector3.zero;
			nEffect.GetComponent<mountsix.util.DestroyAfter>().DESTROY_TIME = destroyTime;
		}
		return destroyTime;
	}

	public GameObject createEffect(Transform parent){
		GameObject nEffect = null;
		if(prefab!=null){
			nEffect = (GameObject)GameObject.Instantiate(prefab);
			nEffect.transform.parent = parent; nEffect.transform.localPosition = Vector3.zero;
			nEffect.GetComponent<mountsix.util.DestroyAfter>().DESTROY_TIME=15;
		}
		return nEffect;
	}
		
	public string getTitle() { return this.getText(this.titles); }

	public string getDescription() { return this.getText(this.descriptions); }

	private string getText( System.Collections.Generic.List<UpgradeTranslation> texts){
		if (languageID.Equals("NONE")) { languageID = getLanguageID(); Debug.Log("Language:"+languageID); }
		System.Predicate<UpgradeTranslation> transFinder = (UpgradeTranslation trans) => { return trans.language.ToString().ToLower() == languageID.ToLower(); };
		UpgradeTranslation fTranslation=texts.Find(transFinder);
		if(fTranslation==null || fTranslation.text.Trim().Equals("")){
			fTranslation=texts.Find(DEFAULT_FINDER);
			if(fTranslation==null || fTranslation.text.Trim().Equals("")){
				return WORD_NOT_FOUND;
			}
		}
		return fTranslation.text;
	}

	private static string getLanguageID(){
		string systemLanguage=DEFAULT_LANGUAGE;
		if (SystemInfo.operatingSystem.ToLower ().Contains ("android")) {
			AndroidJavaClass localeClass = new AndroidJavaClass("java/util/Locale");
			AndroidJavaObject defaultLocale = localeClass.CallStatic<AndroidJavaObject>("getDefault");
			AndroidJavaObject usLocale = localeClass.GetStatic<AndroidJavaObject>("US");
			systemLanguage = defaultLocale.Call<string>("getDisplayLanguage", usLocale);
		}else{
			systemLanguage = Application.systemLanguage.ToString();
		}
		//return UpgradeLanguages.Russian.ToString();
		return systemLanguage.ToLower();
	}
}
