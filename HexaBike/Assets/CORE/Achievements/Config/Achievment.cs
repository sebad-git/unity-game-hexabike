using UnityEngine;
using System.Collections;

[System.Serializable]
public class Achievment {
	public string name;
	public string aid;
	public System.Collections.Generic.List<AchTranslation> titles;
	public System.Collections.Generic.List<AchTranslation> descriptions;
	public string gpid;
	public Category category;
	public Sprite icon;
	public UnlockValueType unlockValueType=UnlockValueType.INTEGER;
	public int unlockValueInt;
	public string unlockValueString;
	public float unlockValueFloat;
	public bool unlockValueBoolean;
	public bool active;
	public bool showDescriptions;
	public bool showTitles;
	private static string languageID = "NONE";
	private const string DEFAULT_LANGUAGE="english";

	private const string WORD_NOT_FOUND="Keyword not Found.";

	public string getTitle() {
		if (languageID.Equals("NONE")) { languageID = getLanguageID(); Debug.Log("Language:"+languageID); }
		System.Predicate<AchTranslation> transFinder = (AchTranslation trans) => { return trans.language.ToString().ToLower() == languageID.ToLower(); };
		System.Predicate<AchTranslation> DEFAULT_FINDER = (AchTranslation trans) => { return trans.language.ToString().ToLower() == DEFAULT_LANGUAGE.ToLower(); };

		AchTranslation fTranslation=this.titles.Find(transFinder);
		if(fTranslation==null || fTranslation.text.Trim().Equals("")){
			fTranslation=this.titles.Find(DEFAULT_FINDER);
			if(fTranslation==null || fTranslation.text.Trim().Equals("")){
				return WORD_NOT_FOUND;
			}
		}
		return fTranslation.text;
	}

	public string getDescription() {
		if (languageID.Equals("NONE")) { languageID = getLanguageID(); Debug.Log("Language:"+languageID); }
		System.Predicate<AchTranslation> transFinder = (AchTranslation trans) => { return trans.language.ToString().ToLower() == languageID.ToLower(); };
		AchTranslation fTranslation=this.descriptions.Find(transFinder);
		if(fTranslation==null || fTranslation.text.Trim().Equals("")){return WORD_NOT_FOUND;}
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
		return systemLanguage.ToLower();
	}
}
