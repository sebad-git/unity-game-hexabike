using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(PowerUps))]
public class UpgradesEditor : Editor {
	
	private PowerUps upgradesFile;
	private Vector2 scrollPos;
	private static string subGroupStyle = "ObjectFieldThumb";
	private static string titleStyle = "MeTransOffRight";
	private static string rootGroupStyle = "GroupBox";
	private static int achCounter;

	public void OnEnable(){ upgradesFile = (PowerUps)target;
		if (upgradesFile != null && upgradesFile.powerUps != null) {
			achCounter = upgradesFile.powerUps.Count;
		}
	}

	public override void OnInspectorGUI(){
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
		if (upgradesFile != null) {
			EditorGUILayout.BeginVertical(subGroupStyle);
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal(rootGroupStyle);
			GUILayout.FlexibleSpace();  EditorGUILayout.LabelField("UPGRADES EDITOR",EditorStyles.boldLabel); GUILayout.FlexibleSpace(); 
			EditorGUILayout.EndHorizontal();

			if (upgradesFile.powerUps != null) {
				EditorGUILayout.BeginVertical(rootGroupStyle);
				EditorGUILayout.BeginHorizontal(subGroupStyle);
				GUILayout.FlexibleSpace(); EditorGUILayout.LabelField("Upgrades");
				GUILayout.FlexibleSpace(); if(StyledButton("Add Upgrade")){ Add(); } GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();

				if (upgradesFile.powerUps.Count == 0) {
					EditorGUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace(); GUILayout.Label ("FILE EMPTY"); GUILayout.FlexibleSpace(); 
					EditorGUILayout.EndHorizontal();
				}else{
					for(int a=0; a<upgradesFile.powerUps.Count;a++) {
						PowerUp pwp = upgradesFile.powerUps[a];
						EditorGUILayout.Space();
						EditorGUILayout.BeginHorizontal(subGroupStyle);
						pwp.active = EditorGUILayout.Foldout (pwp.active, pwp.powerUpName, EditorStyles.foldout); if (GUILayout.Button ("Remove")) { remove(a); }
						EditorGUILayout.EndHorizontal();
						if (pwp.active) {
							EditorGUILayout.BeginHorizontal(subGroupStyle);
							pwp.powerUpName = EditorGUILayout.TextField ("Upgrade name:", pwp.powerUpName.Trim());
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginVertical(subGroupStyle);
							pwp.icon = (Sprite)EditorGUILayout.ObjectField(pwp.icon, typeof(Sprite), false);
							pwp.pwpID = EditorGUILayout.TextField("ID:", pwp.pwpID.Trim());
							pwp.price = EditorGUILayout.IntField("Price:", pwp.price);
							pwp.category = (UpgradeCategory)EditorGUILayout.EnumPopup ("Category:", pwp.category);
							pwp.type = (UpgradeType)EditorGUILayout.EnumPopup ("Type:", pwp.type);
							pwp.prefab = (GameObject)EditorGUILayout.ObjectField(pwp.prefab, typeof(GameObject),false);
							pwp.value1 = EditorGUILayout.FloatField("Value:", pwp.value1);
							pwp.maxUpgrade = EditorGUILayout.FloatField("Max upgrade:", pwp.maxUpgrade);
							if (pwp.type == UpgradeType.ONE_USE) {
								pwp.useAchievment = EditorGUILayout.TextField("One use achievement:", pwp.useAchievment);
							}
							if (pwp.type == UpgradeType.UPGRADABLE) {
								pwp.upgradeAchievment = EditorGUILayout.TextField("Upgrade achievement:", pwp.upgradeAchievment);
							}

							EditorGUILayout.BeginVertical(rootGroupStyle);
							EditorGUILayout.BeginHorizontal(subGroupStyle);
							EditorGUILayout.Space();
							pwp.showTitles = EditorGUILayout.Foldout (pwp.showTitles, "Titles", EditorStyles.foldout); 
							if (GUILayout.Button ("Add")) { addTitle(a); }
							EditorGUILayout.EndHorizontal();
							if (pwp.showTitles) {
								if(pwp.titles!=null){
									for(int t=0; t<pwp.titles.Count; t++){
										EditorGUILayout.BeginHorizontal(subGroupStyle);
										UpgradeTranslation trans=pwp.titles[t];
										trans.text = EditorGUILayout.TextField(trans.text.Trim());
										trans.language = (UpgradeLanguages)EditorGUILayout.EnumPopup(trans.language);
										if (GUILayout.Button ("Remove")) { removeTitle(a,t); }
										EditorGUILayout.EndHorizontal();
									}
								}
							}
							EditorGUILayout.EndVertical();
							EditorGUILayout.BeginVertical(rootGroupStyle);
							EditorGUILayout.BeginHorizontal(subGroupStyle);
							EditorGUILayout.Space();
							pwp.showDescriptions = EditorGUILayout.Foldout(pwp.showDescriptions, "Descriptions", EditorStyles.foldout); 
							if (GUILayout.Button ("Add")) { addDescription(a); }
							EditorGUILayout.EndHorizontal();
							if (pwp.showDescriptions) {
								if(pwp.descriptions!=null){
									for(int d=0; d<pwp.descriptions.Count; d++){
										EditorGUILayout.BeginHorizontal(subGroupStyle);
										UpgradeTranslation trans=pwp.descriptions[d];
										trans.text = EditorGUILayout.TextField(trans.text.Trim());
										trans.language = (UpgradeLanguages)EditorGUILayout.EnumPopup(trans.language);
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
			}else{ upgradesFile.powerUps = new List<PowerUp>(); }
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndVertical();
		} 
		else { GUILayout.Label("No file selected."); }
		EditorGUILayout.EndScrollView();
		EditorUtility.SetDirty(upgradesFile);
	}

	public void duplicate(int index){
		PowerUp pwp = upgradesFile.powerUps[index];
		PowerUp npwp = new PowerUp();
		npwp.pwpID = "UPG"+(achCounter+1);
		npwp.powerUpName = pwp.powerUpName;
		npwp.titles = pwp.titles; npwp.descriptions = pwp.descriptions; 
		npwp.price = pwp.price; npwp.category = pwp.category;
		npwp.type = pwp.type;
		npwp.prefab = pwp.prefab;
		npwp.value1 = pwp.value1;
		npwp.maxUpgrade = pwp.maxUpgrade;
		npwp.useAchievment = pwp.useAchievment;
		npwp.upgradeAchievment = pwp.useAchievment;
		npwp.icon = pwp.icon;
		upgradesFile.powerUps.Add(npwp);
		achCounter=upgradesFile.powerUps.Count;
	}

	public void remove(int index){
		upgradesFile.powerUps.RemoveAt(index);
		achCounter=upgradesFile.powerUps.Count;
	}

	public void Add(){
		achCounter=upgradesFile.powerUps.Count;
		PowerUp pwp = new PowerUp();  pwp.powerUpName = "NEW UPGRADE"; pwp.pwpID = "UPG"+(achCounter+1);
		pwp.descriptions=new List<UpgradeTranslation>(); pwp.titles=new List<UpgradeTranslation>();
		pwp.category = UpgradeCategory.CATEGORY_1; pwp.type= UpgradeType.UPGRADABLE;
		pwp.price = 0; pwp.value1 = 0; pwp.maxUpgrade = 5;
		upgradesFile.powerUps.Add(pwp);
		achCounter=upgradesFile.powerUps.Count;
	}

	public void moveUp(int index){
		if (index > 0) {
			PowerUp pwp = upgradesFile.powerUps[index];
			upgradesFile.powerUps.RemoveAt(index);
			upgradesFile.powerUps.Insert( (index-1), pwp);
		}
		achCounter=upgradesFile.powerUps.Count;
	}

	public void moveDown(int index){
		if (index < upgradesFile.powerUps.Count) {
			PowerUp pwp = upgradesFile.powerUps[index];
			upgradesFile.powerUps.RemoveAt(index);
			upgradesFile.powerUps.Insert ((index + 1), pwp);
		}
	}

	public void addTitle(int index){ upgradesFile.powerUps[index].titles.Add(new UpgradeTranslation()); }
	public void removeTitle(int achIndex,int transIndex){ upgradesFile.powerUps[achIndex].titles.RemoveAt(transIndex); }

	public void addDescription(int index){ upgradesFile.powerUps[index].descriptions.Add(new UpgradeTranslation()); }
	public void removeDescription(int achIndex,int transIndex){ upgradesFile.powerUps[achIndex].descriptions.RemoveAt(transIndex); }

	public bool StyledButton (string label) {
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		bool clickResult = GUILayout.Button(label, "CN CountBadge");
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		return clickResult;
	}
}

