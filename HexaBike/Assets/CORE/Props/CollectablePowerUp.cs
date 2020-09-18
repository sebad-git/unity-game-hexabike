using UnityEngine;
using System.Collections;

public class CollectablePowerUp : MonoBehaviour {

	public GameObject collectEffect;
	public UpgradeCategory category;
	
	void OnTriggerEnter(Collider col) {
		if (col.gameObject.tag == Tags.BIKE_TAG) {
			GameObject.FindGameObjectWithTag(Tags.COIN_SOUND_TAG).GetComponent<AudioSource>().Play();
			if(this.collectEffect!=null){ Instantiate(collectEffect,transform.position,Quaternion.identity); }
			if(this.category==UpgradeCategory.CATEGORY_2){ mountsix.ui.Game.getGame().activateCoinMagnet(); }
			if(this.category==UpgradeCategory.CATEGORY_1){ mountsix.ui.Game.getGame().activateTurbo(); }
			Destroy(gameObject);
		}
	}
}
