using UnityEngine;
using System.Collections;

public class Cheat : MonoBehaviour {

	public int moneyToSet=10000000;

	void Update () {
		if (Input.GetKey (KeyCode.C)) { this.cleanData(); }
		if (Input.GetKey (KeyCode.M)) { this.money(); }
	}

	public void money(){
		PlayerPrefs.SetInt(GameData.TOTAL_SCORE,moneyToSet); Debug.Log("MONEY");
	}

	public void cleanData(){
		PlayerPrefs.DeleteAll(); GameData.init(); Debug.Log("DATA ERASED");
	}
	

}
