using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;


[CustomEditor (typeof(Translator))]
public class TranslatorEditor : Editor {

	private Translator translator;
	private Vector2 scrollPos;
	private static string subGroupStyle = "ObjectFieldThumb";
	private static string rootGroupStyle = "GroupBox";

	public void OnEnable(){ translator = (Translator)target; }

	public override void OnInspectorGUI(){
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);{
			EditorGUILayout.BeginVertical(rootGroupStyle);
				if (translator != null) {
				EditorGUILayout.BeginHorizontal(subGroupStyle);
				GUILayout.Label("Locale File:"); translator.locale = (Locale)EditorGUILayout.ObjectField(translator.locale,typeof(Locale),false);
				if(StyledButton("Add Translation")){ addTranslation(); }
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginVertical(subGroupStyle);
				if (translator.translations != null && translator.translations.Count > 0) {
					for (int i = 0; i < translator.translations.Count; i++) {
						EditorGUILayout.BeginHorizontal (subGroupStyle);
						TDictionary trans = translator.translations[i];
						trans.id = EditorGUILayout.TextField (trans.id);
						trans.label = (Text)EditorGUILayout.ObjectField (trans.label, typeof(Text),true);
						if (StyledButton ("Remove")) {
							removeTranslation (i);
						}
						EditorGUILayout.EndHorizontal ();
					}
				}
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndVertical();

				} else {
					GUILayout.Label("No translator selected.");
				}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndVertical();
		}EditorGUILayout.EndScrollView();

	}

	private void addTranslation(){ 
		if(translator.translations==null){translator.translations = new List<TDictionary>(); }
		translator.translations.Add (new TDictionary());
	}

	private void removeTranslation(int index){ translator.translations.RemoveAt(index); }

	public bool StyledButton (string label) {
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		bool clickResult = GUILayout.Button(label, "CN CountBadge");
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		return clickResult;
	}

}
