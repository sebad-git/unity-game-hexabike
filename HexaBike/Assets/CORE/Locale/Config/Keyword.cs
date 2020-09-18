using UnityEngine;

[System.Serializable]
public class Keyword {

	public string keywordId="NEW WORD";
	public System.Collections.Generic.List<Translation> translations;
	public bool showTranslations;
	private const string DEFAULT_LANGUAGE="english";
	private const string TEXT_NOT_FOUND="Language not Found.";

	public string findWord(string language){
		System.Predicate<Translation> transFinder = (Translation trans) => { return trans.language.ToString().ToLower() == language.ToLower(); };
		System.Predicate<Translation> englishFinder = (Translation trans) => { return trans.language.ToString().ToLower() == DEFAULT_LANGUAGE.ToLower(); };
		Translation fTranslation=this.translations.Find(transFinder);
		if(!this.translationExists(fTranslation)){
			fTranslation = this.translations.Find(englishFinder);
			if (!this.translationExists(fTranslation)) { return TEXT_NOT_FOUND; }
		}
		return fTranslation.text;
	}

	private bool translationExists(Translation p_translation){
		return (p_translation != null && !p_translation.text.Trim().Equals(""));
	}
}
