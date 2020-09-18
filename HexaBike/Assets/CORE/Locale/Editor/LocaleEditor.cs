using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(Locale))]
public class LocaleEditor : Editor{
	
	private Locale locale;
	private Vector2 scrollPos;
	private static string subGroupStyle = "ObjectFieldThumb";
	private static string rootGroupStyle = "GroupBox";

	public void OnEnable(){ locale = (Locale)target; }

	public override void OnInspectorGUI(){
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);{
			if (locale != null) {
				EditorGUILayout.BeginVertical(subGroupStyle);

				EditorGUILayout.BeginHorizontal(rootGroupStyle);
				GUILayout.FlexibleSpace(); EditorGUILayout.LabelField("LANGUAGE EDITOR",EditorStyles.boldLabel); GUILayout.FlexibleSpace(); 
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();

				if (locale.dictionary != null) {
					EditorGUILayout.BeginVertical(rootGroupStyle);
					EditorGUILayout.BeginHorizontal(subGroupStyle);
					GUILayout.FlexibleSpace(); EditorGUILayout.LabelField("Words");
					GUILayout.FlexibleSpace(); if(StyledButton("Add Word")){ addWord(); } GUILayout.FlexibleSpace();
					EditorGUILayout.EndHorizontal();

					if (locale.dictionary.Count == 0) {
						EditorGUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace(); GUILayout.Label ("FILE EMPTY"); GUILayout.FlexibleSpace(); 
						EditorGUILayout.EndHorizontal();
					}else{
						for(int k=0; k<locale.dictionary.Count; k++){
							EditorGUILayout.BeginVertical(rootGroupStyle);
							EditorGUILayout.BeginHorizontal(subGroupStyle);
							Keyword key = locale.dictionary[k];
							key.keywordId = EditorGUILayout.TextField(key.keywordId); if(StyledButton("Remove Word")){ removeWord(k); }
							EditorGUILayout.EndHorizontal();
							key.showTranslations = EditorGUILayout.Foldout(key.showTranslations, "Translations", EditorStyles.foldout);
							if (key.showTranslations) {
								for(int t=0; t<key.translations.Count; t++) {
									Translation trans = key.translations[t];
									EditorGUILayout.BeginHorizontal(subGroupStyle);
									trans.text = EditorGUILayout.TextField (trans.text);
									trans.language = (Languages)EditorGUILayout.EnumPopup (trans.language);
									if(StyledButton("Remove Translation")){ removTranslation(k,t); }
									EditorGUILayout.EndHorizontal();
								}
								if(StyledButton("Add Translation")){ addTranslation(k); }
							}
							EditorGUILayout.EndVertical();
						}
					}
				}else{ locale.dictionary = new System.Collections.Generic.List<Keyword>(); }
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndVertical();
			}
			else {
				GUILayout.Label("No locale selected.");
			}
		}EditorGUILayout.EndScrollView();
	}

	private void addWord(){
		Keyword kw = new Keyword(); kw.keywordId="NEW WORD";
		kw.translations = new System.Collections.Generic.List<Translation>();
		locale.dictionary.Add(kw);
	}

	private void removeWord(int index){ locale.dictionary.RemoveAt(index); }

	private void addTranslation(int k){
		if (locale.dictionary[k].translations == null) { locale.dictionary[k].translations = new System.Collections.Generic.List<Translation>(); }
		Translation tr = new Translation(); tr.text="NEW TRANSLATION"; locale.dictionary[k].translations.Add(tr);
	}

	private void removTranslation(int index, int trIndex){ locale.dictionary[index].translations.RemoveAt (trIndex); }

	public bool StyledButton (string label) {
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		bool clickResult = GUILayout.Button(label, "CN CountBadge");
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		return clickResult;
	}
}
