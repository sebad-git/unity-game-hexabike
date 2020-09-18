using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Translator : MonoBehaviour {

	public Locale locale;
	public System.Collections.Generic.List<TDictionary> translations;

	void Awake(){this.translate(); }

	public void translate(){
		foreach(TDictionary trans in this.translations){
			trans.label.text=this.locale.getText(trans.id);
		}
	}
}
