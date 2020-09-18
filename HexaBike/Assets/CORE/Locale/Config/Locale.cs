using UnityEngine;
using System.Collections;

public class Locale : ScriptableObject {

	public System.Collections.Generic.List<Keyword> dictionary;

	private static string languageID = "NONE";
	private const string DEFAULT_LANGUAGE="english";
	private const string WORD_NOT_FOUND="Language not Found.";

	public string getText(string key) {
		if (languageID.Equals("NONE")) { languageID = getLanguageID(); Debug.Log("Language:"+languageID); }
		Keyword word = Locale.getWord(key, this.dictionary);
		if(word==null){return WORD_NOT_FOUND;}
		return word.findWord(languageID);
	}

	private static Keyword getWord(string keyId,System.Collections.Generic.List<Keyword> p_dictionary){
		System.Predicate<Keyword> wordFinder = (Keyword word) => { return word.keywordId.Equals(keyId); };
		Keyword fKeyword=p_dictionary.Find(wordFinder);
		if(fKeyword==null){return null;}
		return fKeyword;
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
		//return Languages.Russian.ToString();
		return systemLanguage.ToLower();
	}
}
