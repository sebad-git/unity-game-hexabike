using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(AchievmentsConfig))]
public class AchievmentsEditor : Editor {
	
	private AchievmentsConfig achievmentInfo;
	private Vector2 scrollPos;
	private static string subGroupStyle = "ObjectFieldThumb";
	private static string titleStyle = "MeTransOffRight";
	private static string rootGroupStyle = "GroupBox";
	private static int achCounter;

	public void OnEnable(){ achievmentInfo = (AchievmentsConfig)target;
		
		if (achievmentInfo != null && achievmentInfo.achievmentsList != null) {
			achCounter = achievmentInfo.achievmentsList.Count;
		}
	}

	public override void OnInspectorGUI(){
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
		if (achievmentInfo != null) {
			EditorGUILayout.BeginVertical(subGroupStyle);
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal(rootGroupStyle);
			GUILayout.FlexibleSpace();  EditorGUILayout.LabelField("ACHIEVEMENTS EDITOR",EditorStyles.boldLabel); GUILayout.FlexibleSpace(); 
			EditorGUILayout.EndHorizontal();

			if (achievmentInfo.achievmentsList != null) {
				EditorGUILayout.BeginVertical(rootGroupStyle);
				EditorGUILayout.BeginHorizontal(subGroupStyle);
				GUILayout.FlexibleSpace(); EditorGUILayout.LabelField("Achievements");
				GUILayout.FlexibleSpace(); if(StyledButton("Add Achievement")){ Add(); } GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();

				if (achievmentInfo.achievmentsList.Count == 0) {
					EditorGUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace(); GUILayout.Label ("FILE EMPTY"); GUILayout.FlexibleSpace(); 
					EditorGUILayout.EndHorizontal();
				}else{
					for(int a=0; a<achievmentInfo.achievmentsList.Count;a++) {
						Achievment ach = achievmentInfo.achievmentsList[a];
						EditorGUILayout.Space();
						EditorGUILayout.BeginHorizontal(subGroupStyle);
						ach.active = EditorGUILayout.Foldout (ach.active, ach.name, EditorStyles.foldout); if (GUILayout.Button ("Remove")) { remove(a); }
						EditorGUILayout.EndHorizontal();
						if (ach.active) {
							EditorGUILayout.BeginHorizontal(subGroupStyle);
							ach.name = EditorGUILayout.TextField ("Achievement:", ach.name.Trim());
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginVertical(subGroupStyle);
							ach.aid = EditorGUILayout.TextField ("Achievement ID:", ach.aid.Trim());
							ach.gpid = EditorGUILayout.TextField ("Google Play ID:", ach.gpid.Trim());
							ach.category = (Category)EditorGUILayout.EnumPopup ("Category:", ach.category);
							EditorGUIUtility.labelWidth = 180;
							EditorGUILayout.LabelField ("Icon:");
							ach.icon = (Sprite)EditorGUILayout.ObjectField (ach.icon, typeof(Sprite), false);
							//UNLOCK VALUES.
							ach.unlockValueType = (UnlockValueType)EditorGUILayout.EnumPopup ("Unlock Value Type:", ach.unlockValueType);
							if (ach.unlockValueType == UnlockValueType.INTEGER) {
								ach.unlockValueInt = EditorGUILayout.IntField ("Unlock Value:", ach.unlockValueInt);
							}
							if (ach.unlockValueType == UnlockValueType.STRING) {
								ach.unlockValueString = EditorGUILayout.TextField ("Unlock Value:", ach.unlockValueString);
							}
							if (ach.unlockValueType == UnlockValueType.FLOAT) {
								ach.unlockValueFloat = EditorGUILayout.FloatField ("Unlock Value:", ach.unlockValueFloat);
							}
							if (ach.unlockValueType == UnlockValueType.BOOLEAN) {
								ach.unlockValueBoolean = EditorGUILayout.Toggle ("Unlock Value:", ach.unlockValueBoolean);
							}
							EditorGUILayout.BeginVertical(rootGroupStyle);
							EditorGUILayout.BeginHorizontal(subGroupStyle);
							EditorGUILayout.Space();
							ach.showTitles = EditorGUILayout.Foldout (ach.showTitles, "Titles", EditorStyles.foldout); 
							if (GUILayout.Button ("Add")) { addTitle(a); }
							EditorGUILayout.EndHorizontal();
							if (ach.showTitles) {
								if(ach.titles!=null){
									for(int t=0; t<ach.titles.Count; t++){
										EditorGUILayout.BeginHorizontal(subGroupStyle);
										AchTranslation trans=ach.titles[t];
										trans.text = EditorGUILayout.TextField(trans.text.Trim());
										trans.language = (AchLanguages)EditorGUILayout.EnumPopup(trans.language);
										if (GUILayout.Button ("Remove")) { removeTitle(a,t); }
										EditorGUILayout.EndHorizontal();
									}
								}
							}
							EditorGUILayout.EndVertical();
							EditorGUILayout.BeginVertical(rootGroupStyle);
							EditorGUILayout.BeginHorizontal(subGroupStyle);
							EditorGUILayout.Space();
							ach.showDescriptions = EditorGUILayout.Foldout(ach.showDescriptions, "Descriptions", EditorStyles.foldout); 
							if (GUILayout.Button ("Add")) { addDescription(a); }
							EditorGUILayout.EndHorizontal();
							if (ach.showDescriptions) {
								if(ach.descriptions!=null){
									for(int d=0; d<ach.descriptions.Count; d++){
										EditorGUILayout.BeginHorizontal(subGroupStyle);
										AchTranslation trans=ach.descriptions[d];
										trans.text = EditorGUILayout.TextField(trans.text.Trim());
										trans.language = (AchLanguages)EditorGUILayout.EnumPopup(trans.language);
										if (GUILayout.Button ("Remove")) { removeDescription(a,d); }
										EditorGUILayout.EndHorizontal();
									}
								}
							}
							EditorGUILayout.EndVertical();
							EditorGUILayout.BeginHorizontal(titleStyle);
							if (GUILayout.Button ("Duplicate")) { duplicate(a); } 
							if (GUILayout.Button ("Move Up")) { moveUp(a); } if (GUILayout.Button ("Move Down")) { moveDown(a); }
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.EndVertical();
						}
					}
					GUILayout.FlexibleSpace(); 	GUILayout.FlexibleSpace();
					EditorGUILayout.EndVertical();
				}
			}else{ achievmentInfo.achievmentsList = new List<Achievment>(); }
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndVertical();
		} 
		else { GUILayout.Label("No file selected."); }
		EditorGUILayout.EndScrollView();
		EditorUtility.SetDirty(achievmentInfo);
	}

	public void duplicate(int index){
		Achievment ach = achievmentInfo.achievmentsList[index];
		Achievment nach = new Achievment();
		nach.aid = "ACHV"+(achCounter+1);
		nach.name = ach.name;
		nach.titles = ach.titles; nach.descriptions = ach.descriptions; nach.gpid = ach.gpid;
		nach.category = ach.category; nach.icon = ach.icon;
		nach.unlockValueBoolean = ach.unlockValueBoolean; nach.unlockValueFloat = ach.unlockValueFloat; 
		nach.unlockValueInt = ach.unlockValueInt; nach.unlockValueString = ach.unlockValueString;
		nach.unlockValueType = ach.unlockValueType; nach.active= true;
		achievmentInfo.achievmentsList.Add(nach);
		achCounter=achievmentInfo.achievmentsList.Count;
	}

	public void remove(int index){
		achievmentInfo.achievmentsList.RemoveAt(index);
		achCounter=achievmentInfo.achievmentsList.Count;
	}

	public void Add(){
		achCounter=achievmentInfo.achievmentsList.Count;
		Achievment ach = new Achievment();  ach.name = "NEW ACHIEVEMENT"; ach.aid = "ACHV"+(achCounter+1);
		ach.descriptions=new List<AchTranslation>(); ach.titles=new List<AchTranslation>(); ach.gpid="";
		ach.category = Category.CATEGORY_1;
		achievmentInfo.achievmentsList.Add(ach);
		achCounter=achievmentInfo.achievmentsList.Count;
	}

	public void moveUp(int index){
		if (index > 0) {
			Achievment ach = achievmentInfo.achievmentsList [index];
			achievmentInfo.achievmentsList.RemoveAt(index);
			achievmentInfo.achievmentsList.Insert( (index-1), ach );
		}
		achCounter=achievmentInfo.achievmentsList.Count;
	}

	public void moveDown(int index){
		if (index < achievmentInfo.achievmentsList.Count) {
			Achievment ach = achievmentInfo.achievmentsList [index];
			achievmentInfo.achievmentsList.RemoveAt(index);
			achievmentInfo.achievmentsList.Insert ((index + 1), ach);
		}
	}

	public void addTitle(int index){ achievmentInfo.achievmentsList[index].titles.Add(new AchTranslation()); }

	public void removeTitle(int achIndex,int transIndex){ achievmentInfo.achievmentsList[achIndex].titles.RemoveAt(transIndex); }

	public void addDescription(int index){ achievmentInfo.achievmentsList[index].descriptions.Add(new AchTranslation()); }

	public void removeDescription(int achIndex,int transIndex){ achievmentInfo.achievmentsList[achIndex].descriptions.RemoveAt(transIndex); }

	public bool StyledButton (string label) {
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		bool clickResult = GUILayout.Button(label, "CN CountBadge");
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		return clickResult;
	}
}

